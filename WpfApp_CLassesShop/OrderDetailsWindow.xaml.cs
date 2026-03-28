using BLL.Services;
using System.Windows;
using WpfApp_CLassesShop.Session;

namespace WpfApp_CLassesShop
{
    public partial class OrderDetailsWindow : Window
    {
        private int _currentOrderId;

        public OrderDetailsWindow(int orderId)
        {
            InitializeComponent();
            _currentOrderId = orderId;
            LoadDetails(orderId);
        }

        private void LoadDetails(int orderId)
        {
            try
            {
                OrderService orderService = new OrderService();
                var items = orderService.GetOrderDetails(orderId);
                dgOrderDetails.ItemsSource = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải chi tiết đơn hàng: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnReOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentSession.LoggedInAccount == null)
                {
                    MessageBox.Show("Phiên đăng nhập đã hết. Vui lòng đăng nhập lại.");
                    new LoginWindow().Show();
                    this.Close();
                    return;
                }

                int currentUserId = (int)CurrentSession.LoggedInAccount.Id;

                OrderService orderService = new OrderService();
                orderService.AddOldOrderToCart(_currentOrderId, currentUserId);

                MessageBoxResult result = MessageBox.Show(
                    "Đã thêm toàn bộ sản phẩm của đơn hàng này vào Giỏ hàng.\n\nBạn có muốn mở Giỏ hàng ngay không?",
                    "Thêm vào giỏ thành công",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
                {
                    new CartWindow().Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi mua lại: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}