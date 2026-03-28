using DAL.Models;
using System.Windows;
using WpfApp_CLassesShop.Session;

namespace WpfApp_CLassesShop
{
    public partial class OrderHistoryWindow : Window
    {
        private int _currentAccountId;

        public OrderHistoryWindow()
        {
            InitializeComponent();

            if (CurrentSession.LoggedInAccount == null)
            {
                MessageBox.Show("Phiên đăng nhập đã hết. Vui lòng đăng nhập lại.");
                new LoginWindow().Show();
                this.Close();
                return;
            }

            _currentAccountId = (int)CurrentSession.LoggedInAccount.Id;
            LoadOrderHistory();
        }

        private void LoadOrderHistory()
        {
            try
            {
                BLL.Services.OrderService orderService = new BLL.Services.OrderService();
                var orders = orderService.GetOrderHistory(_currentAccountId);
                dgOrders.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải lịch sử đơn hàng: " + ex.Message,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnViewDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            var selectedOrder = button?.DataContext as Order;

            if (selectedOrder != null)
            {
                new OrderDetailsWindow((int)selectedOrder.Id).ShowDialog();
            }
        }
    }
}