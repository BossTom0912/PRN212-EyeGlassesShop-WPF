using BLL.Services;
using System;
using System.Windows;

namespace WpfApp_CLassesShop
{
    public partial class SupportOrderDetailsWindow : Window
    {
        private readonly SupportStaffService _supportService;
        private readonly long _orderId;

        public SupportOrderDetailsWindow(long orderId)
        {
            InitializeComponent();
            _supportService = new SupportStaffService();
            _orderId = orderId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var order = _supportService.GetOrderById(_orderId);

                txtOrderCode.Text = $"Mã đơn: {order.OrderCode}";
                txtCustomerName.Text = $"Khách hàng: {order.Account?.FullName}";
                txtReceiverName.Text = $"Người nhận: {order.ReceiverName}";
                txtPhone.Text = $"Số điện thoại: {order.Phone}";
                txtAddress.Text = $"Địa chỉ giao: {order.ShippingAddress}";
                txtStatus.Text = $"Trạng thái: {order.Status}";
                txtPaymentMethod.Text = $"Phương thức thanh toán: {order.PaymentMethod}";
                txtTotalAmount.Text = $"Tổng tiền: {order.TotalAmount:N0} VND";

                dgOrderItems.ItemsSource = order.OrderItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi load chi tiết đơn hàng:\n\n" + ex.ToString(), "Lỗi");
                this.Close();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}