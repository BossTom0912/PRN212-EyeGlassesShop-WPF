using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IOrderRepository
    {
        void Checkout(int accountId, List<CartItem> cartItems, string receiverName, string phone, string shippingAddress, string orderCode);
        void UpdateOrderStatus(string orderCode, string status);

        List<Order> GetOrdersByAccountId(int accountId);
        List<OrderItem> GetOrderItemsByOrderId(int orderId);
    }

}
