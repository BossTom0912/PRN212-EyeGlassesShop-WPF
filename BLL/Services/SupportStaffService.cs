using DAL.Implementations;
using DAL.Interfaces;
using DAL.Models;

namespace BLL.Services
{
    public class SupportStaffService
    {
        private readonly IOrderRepository _orderRepository;

        public SupportStaffService()
        {
            _orderRepository = new OrderRepository();
        }

        public List<Order> GetAllOrders(string? status = null)
        {
            return _orderRepository.GetOrdersForSupport(status);
        }

        public Order GetOrderById(long orderId)
        {
            var order = _orderRepository.GetOrderById(orderId);
            if (order == null)
                throw new Exception("Không tìm thấy đơn hàng.");

            return order;
        }

        public void ConfirmOrder(long orderId, long staffId)
        {
            _orderRepository.SupportConfirmOrder(orderId, staffId);
        }

        public void CompleteOrder(long orderId, long staffId)
        {
            _orderRepository.SupportCompleteOrder(orderId, staffId);
        }

        public void CancelOrder(long orderId, long staffId, string reason)
        {
            _orderRepository.SupportCancelOrder(orderId, staffId, reason);
        }
    }
}