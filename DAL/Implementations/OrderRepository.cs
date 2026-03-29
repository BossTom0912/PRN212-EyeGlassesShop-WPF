using DAL.Constants;
using DAL.DBContext;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using DAL.Constants;
namespace DAL.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        public void Checkout(int accountId, List<CartItem> cartItems, string receiverName, string phone, string shippingAddress, string orderCode, string paymentMethod)
        {
            paymentMethod = PaymentMethods.Normalize(paymentMethod);
            string initialStatus = paymentMethod == PaymentMethods.VNPAY
     ? OrderStatuses.PendingPayment
        : OrderStatuses.WaitingConfirm;
            using (var context = new GlassesShopContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    if (cartItems == null || cartItems.Count == 0)
                        throw new Exception("Giỏ hàng trống.");

                    var now = DateTime.UtcNow;
                    var variantMap = new Dictionary<long, ProductVariant>();

                    foreach (var item in cartItems)
                    {
                        var variant = context.ProductVariants.FirstOrDefault(v => v.Id == item.ProductVariantId && v.IsActive);
                        if (variant == null)
                            throw new Exception($"Không tìm thấy biến thể sản phẩm ID = {item.ProductVariantId}.");

                        if (variant.StockQuantity < item.Quantity)
                            throw new Exception($"Sản phẩm SKU '{variant.Sku}' không đủ tồn kho.");

                        variantMap[item.ProductVariantId] = variant;
                    }

                    decimal totalAmount = cartItems.Sum(item => item.Quantity * item.UnitPrice);

                    paymentMethod = PaymentMethods.Normalize(paymentMethod);

                    string orderStatus = paymentMethod == PaymentMethods.VNPAY
                        ? OrderStatuses.PendingPayment
                        : OrderStatuses.WaitingConfirm;

                    var order = new Order
                    {
                        AccountId = accountId,
                        OrderCode = orderCode,
                        ReceiverName = receiverName,
                        Phone = phone,
                        ShippingAddress = shippingAddress,
                        OrderDate = now,
                        Status = orderStatus,
                        PaymentMethod = paymentMethod,
                        TotalAmount = totalAmount,
                        CreatedAt = now,
                        UpdatedAt = now
                    };

                    context.Orders.Add(order);
                    context.SaveChanges();

                    foreach (var item in cartItems)
                    {
                        var variant = variantMap[item.ProductVariantId];
                        variant.StockQuantity -= item.Quantity;
                        variant.UpdatedAt = now;

                        var orderItem = new OrderItem
                        {
                            OrderId = order.Id,
                            ProductVariantId = item.ProductVariantId,
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            SubTotal = item.Quantity * item.UnitPrice,
                            ProductNameSnapshot = variant.Product?.Name ?? "Kính thời trang",
                            SkuSnapshot = variant.Sku ?? "N/A",
                            ColorSnapshot = variant.Color ?? "N/A",
                            SizeSnapshot = variant.Size ?? "N/A",
                            CreatedAt = now,
                            UpdatedAt = now
                        };

                        context.OrderItems.Add(orderItem);
                    }

                    AddOrderStatusHistory(
    context,
    order.Id,
    null,
    orderStatus,
    accountId,
    "Khách hàng tạo đơn hàng.");

                    context.CartItems.RemoveRange(cartItems);

                    context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    string realError = ex.Message;
                    if (ex.InnerException != null)
                    {
                        realError += "\n\nCHI TIẾT LỖI TỪ DATABASE:\n" + ex.InnerException.Message;
                    }

                    throw new Exception(realError);
                }
            }
        }

        public List<Order> GetOrdersByAccountId(int accountId)
        {
            using (var context = new GlassesShopContext())
            {
                return context.Orders
                    .Where(o => o.AccountId == accountId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();
            }
        }

        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            using (var context = new GlassesShopContext())
            {
                return context.OrderItems
                    .Where(oi => oi.OrderId == orderId)
                    .OrderBy(oi => oi.Id)
                    .ToList();
            }
        }

        public void UpdateOrderStatus(string orderCode, string status, long? changedByAccountId = null, string? note = null)
        {
            using (var context = new GlassesShopContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                var order = context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefault(o => o.OrderCode == orderCode);

                if (order == null)
                    throw new Exception("Không tìm thấy đơn hàng.");

                var oldStatus = order.Status;

                if (oldStatus == status)
                    return;

                if (status == OrderStatuses.Cancelled && oldStatus != OrderStatuses.Cancelled)
                {
                    RestoreStockForOrder(context, order);
                    order.CancelledAt = DateTime.UtcNow;
                }

                order.Status = status;
                order.UpdatedAt = DateTime.UtcNow;

                AddOrderStatusHistory(
                    context,
                    order.Id,
                    oldStatus,
                    status,
                    changedByAccountId,
                    note ?? $"Cập nhật trạng thái đơn từ {oldStatus} sang {status}");

                context.SaveChanges();
                transaction.Commit();
            }
        }

        public List<Order> GetOrdersForSupport(string? status = null)
        {
            using (var context = new GlassesShopContext())
            {
                var query = context.Orders
                    .Include(o => o.Account)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(status))
                {
                    query = query.Where(o => o.Status == status);
                }
                else
                {
                    query = query.Where(o => o.Status != OrderStatuses.PendingPayment);
                }

                return query.OrderByDescending(o => o.OrderDate).ToList();
            }
        }

        public Order? GetOrderById(long orderId)
        {
            using (var context = new GlassesShopContext())
            {
                return context.Orders
                    .Include(o => o.Account)
                    .Include(o => o.OrderItems)
                    .FirstOrDefault(o => o.Id == orderId);
            }
        }

        public void SupportConfirmOrder(long orderId, long staffId)
        {
            using (var context = new GlassesShopContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                var order = context.Orders.FirstOrDefault(o => o.Id == orderId);
                if (order == null)
                    throw new Exception("Không tìm thấy đơn hàng.");

                if (order.Status != OrderStatuses.WaitingConfirm)
                    throw new Exception("Chỉ đơn ở trạng thái WAITING_CONFIRM mới được xác nhận.");

                var oldStatus = order.Status;
                var now = DateTime.UtcNow;

                order.Status = OrderStatuses.Confirmed;
                order.ApprovedByOwnerId = staffId;
                order.ApprovedAt = now;
                order.UpdatedAt = now;

                AddOrderStatusHistory(
                    context,
                    order.Id,
                    oldStatus,
                    OrderStatuses.Confirmed,
                    staffId,
                    "Support staff xác nhận đơn hàng.");

                context.SaveChanges();
                transaction.Commit();
            }
        }

        public void SupportCompleteOrder(long orderId, long staffId)
        {
            using (var context = new GlassesShopContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                var order = context.Orders.FirstOrDefault(o => o.Id == orderId);
                if (order == null)
                    throw new Exception("Không tìm thấy đơn hàng.");

                if (order.Status != OrderStatuses.Confirmed)
                    throw new Exception("Chỉ đơn đã được support xác nhận mới được hoàn tất.");

                var oldStatus = order.Status;
                var now = DateTime.UtcNow;

                order.Status = OrderStatuses.Completed;
                order.CompletedAt = now;
                order.UpdatedAt = now;

                if (order.ApprovedByOwnerId == null)
                    order.ApprovedByOwnerId = staffId;

                AddOrderStatusHistory(
                    context,
                    order.Id,
                    oldStatus,
                    OrderStatuses.Completed,
                    staffId,
                    "Support staff hoàn tất đơn hàng.");

                context.SaveChanges();
                transaction.Commit();
            }
        }

        public void SupportCancelOrder(long orderId, long staffId, string reason)
        {
            using (var context = new GlassesShopContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                var order = context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefault(o => o.Id == orderId);

                if (order == null)
                    throw new Exception("Không tìm thấy đơn hàng.");

                if (order.Status == OrderStatuses.Cancelled)
                    throw new Exception("Đơn hàng đã bị hủy trước đó.");

                if (order.Status == OrderStatuses.Completed)
                    throw new Exception("Đơn hàng đã hoàn tất, không thể hủy.");

                var oldStatus = order.Status;
                var now = DateTime.UtcNow;

                RestoreStockForOrder(context, order);

                order.Status = OrderStatuses.Cancelled;
                order.CancelledAt = now;
                order.CancelReason = reason;
                order.UpdatedAt = now;

                AddOrderStatusHistory(
                    context,
                    order.Id,
                    oldStatus,
                    OrderStatuses.Cancelled,
                    staffId,
                    string.IsNullOrWhiteSpace(reason)
                        ? "Support staff hủy đơn hàng."
                        : $"Support staff hủy đơn: {reason}");

                context.SaveChanges();
                transaction.Commit();
            }
        }

        private void AddOrderStatusHistory(
            GlassesShopContext context,
            long orderId,
            string? oldStatus,
            string newStatus,
            long? changedByAccountId,
            string? note)
        {
            context.OrderStatusHistories.Add(new OrderStatusHistory
            {
                OrderId = orderId,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ChangedByAccountId = changedByAccountId,
                Note = note,
                ChangedAt = DateTime.UtcNow
            });
        }

        private void RestoreStockForOrder(GlassesShopContext context, Order order)
        {
            if (order.OrderItems == null || order.OrderItems.Count == 0)
            {
                order = context.Orders
                    .Include(o => o.OrderItems)
                    .First(o => o.Id == order.Id);
            }

            foreach (var item in order.OrderItems)
            {
                var variant = context.ProductVariants.FirstOrDefault(v => v.Id == item.ProductVariantId);
                if (variant != null)
                {
                    variant.StockQuantity += item.Quantity;
                    variant.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}