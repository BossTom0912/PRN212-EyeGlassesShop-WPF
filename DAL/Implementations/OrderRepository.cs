using DAL.DBContext;
using DAL.Interfaces;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        public void Checkout(int accountId, List<CartItem> cartItems, string receiverName, string phone, string shippingAddress, string orderCode)
        {
            using (var context = new GlassesShopContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        decimal totalAmount = cartItems.Sum(item => (decimal)(item.Quantity * item.UnitPrice));

                        var order = new Order
                        {
                            AccountId = accountId,
                            OrderCode = orderCode,
                            ReceiverName = receiverName,    
                            Phone = phone,                 
                            ShippingAddress = shippingAddress, 
                            OrderDate = DateTime.UtcNow,
                            Status = "PENDING",
                            PaymentMethod = "COD",
                            TotalAmount = totalAmount,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        context.Orders.Add(order);
                        context.SaveChanges();

                    
                        foreach (var item in cartItems)
                        {
                            var variant = context.ProductVariants.Find(item.ProductVariantId);

                            var orderItem = new OrderItem
                            {
                                OrderId = order.Id,
                                ProductVariantId = item.ProductVariantId,
                                Quantity = (int)item.Quantity,
                                UnitPrice = (decimal)item.UnitPrice,
                                SubTotal = (decimal)(item.Quantity * item.UnitPrice),
                                ProductNameSnapshot = "Kính thời trang",
                                SkuSnapshot = variant?.Sku ?? "N/A",
                                ColorSnapshot = variant?.Color ?? "N/A",
                                SizeSnapshot = variant?.Size ?? "N/A",
                                CreatedAt = DateTime.UtcNow,
                                UpdatedAt = DateTime.UtcNow
                            };
                            context.OrderItems.Add(orderItem);
                        }

                       
                        var history = new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            NewStatus = "PENDING", 
                            ChangedAt = DateTime.UtcNow,
                            Note = "Khách hàng đặt đơn từ hệ thống"
                        };
                        context.OrderStatusHistories.Add(history);

                    
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
        }
        //Nay la xem lich su 
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

        //Xem chi tiet mon hang
        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            using (var context = new GlassesShopContext())
            {
                return context.OrderItems
                              .Where(oi => oi.OrderId == orderId)
                              .ToList();
            }
        }

        public void UpdateOrderStatus(string orderCode, string status)
        {
            using (var context = new GlassesShopContext())
            {
                var order = context.Orders.FirstOrDefault(o => o.OrderCode == orderCode);
                if (order != null)
                {
                    order.Status = status; 
                    order.UpdatedAt = DateTime.UtcNow;

                    var history = new OrderStatusHistory
                    {
                        OrderId = order.Id,
                        NewStatus = status,
                        ChangedAt = DateTime.UtcNow,
                        Note = "Khách hàng xác nhận " + (status == "PAID" ? "Đã thanh toán" : "Đã hủy") + " qua VNPAY"
                    };
                    context.OrderStatusHistories.Add(history);

                    context.SaveChanges();
                }
            }
        }
    }
}
