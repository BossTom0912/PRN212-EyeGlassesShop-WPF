using DAL.Models;

namespace DAL.Interfaces
{
    public interface IOrderRepository
    {
        void Checkout(
            int accountId,
            List<CartItem> cartItems,
            string receiverName,
            string phone,
            string shippingAddress,
            string orderCode,
            string paymentMethod);

        void UpdateOrderStatus(
            string orderCode,
            string status,
            long? changedByAccountId = null,
            string? note = null);

        List<Order> GetOrdersByAccountId(int accountId);
        List<OrderItem> GetOrderItemsByOrderId(int orderId);

        List<Order> GetOrdersForSupport(string? status = null);
        Order? GetOrderById(long orderId);
        void SupportConfirmOrder(long orderId, long staffId);
        void SupportCompleteOrder(long orderId, long staffId);
        void SupportCancelOrder(long orderId, long staffId, string reason);
    }
}