using BLL.Utils;
using DAL.Implementations;
using DAL.Interfaces;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;

        public OrderService()
        {
            _orderRepo = new OrderRepository();
            _cartRepo = new CartRepository();
        }

        public void PlaceOrder(int accountId, string receiverName, string phone, string shippingAddress, string orderCode, string paymentMethod)
        {
            var cart = _cartRepo.GetCartByAccountId(accountId);
            if (cart == null) throw new Exception("Không tìm thấy giỏ hàng.");

            var cartItems = _cartRepo.GetAllCartItems((int)cart.Id);
            if (cartItems == null || cartItems.Count == 0)
            {
                throw new Exception("Giỏ hàng của bạn đang trống! Hãy chọn món đồ yêu thích trước nhé.");
            }

            _orderRepo.Checkout(accountId, cartItems, receiverName, phone, shippingAddress, orderCode, paymentMethod);
        }

        public void UpdateOrderStatus(string orderCode, string status)
        {
            _orderRepo.UpdateOrderStatus(orderCode, status);
        }

        public List<Order> GetOrderHistory(int accountId)
        {
            return _orderRepo.GetOrdersByAccountId(accountId);
        }

        public List<OrderItem> GetOrderDetails(int orderId)
        {
            return _orderRepo.GetOrderItemsByOrderId(orderId);
        }

        //Thanh toan VNPAY
        public string GenerateVnPayUrl(decimal totalAmount, string orderCode)
        {
            string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
            string vnp_TmnCode = "QGEPANHQ";
            string vnp_HashSecret = "64HHE50RXQZLQ7R67MA72GNDI8Y2VPMQ";

            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);

            long amount = (long)(totalAmount * 100);
            vnpay.AddRequestData("vnp_Amount", amount.ToString());

            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", "127.0.0.1");
            vnpay.AddRequestData("vnp_Locale", "vn");

            vnpay.AddRequestData("vnp_OrderInfo", "ThanhToan_" + orderCode);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", "http://localhost");
            vnpay.AddRequestData("vnp_TxnRef", orderCode);

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }

        public void AddOldOrderToCart(int oldOrderId, int accountId)
        {
            using (var context = new DAL.DBContext.GlassesShopContext())
            {
                var oldOrderItems = context.OrderItems.Where(oi => oi.OrderId == oldOrderId).ToList();

                if (oldOrderItems == null || oldOrderItems.Count == 0)
                {
                    throw new Exception("Đơn hàng này không có sản phẩm nào để mua lại!");
                }

                var cart = context.Carts.FirstOrDefault(c => c.AccountId == accountId);
                if (cart == null)
                {
                    throw new Exception("Hệ thống chưa tạo giỏ hàng cho tài khoản này.");
                }

                foreach (var oldItem in oldOrderItems)
                {
                    var existingCartItem = context.CartItems.FirstOrDefault(ci => ci.CartId == cart.Id && ci.ProductVariantId == oldItem.ProductVariantId);

                    if (existingCartItem != null)
                    {
                        existingCartItem.Quantity += oldItem.Quantity;
                    }
                    else
                    {
                        var newCartItem = new DAL.Models.CartItem
                        {
                            CartId = cart.Id,
                            ProductVariantId = oldItem.ProductVariantId,
                            Quantity = oldItem.Quantity,
                            UnitPrice = oldItem.UnitPrice
                        };
                        context.CartItems.Add(newCartItem);
                    }
                }

                context.SaveChanges();
            }
        }
    }
}
