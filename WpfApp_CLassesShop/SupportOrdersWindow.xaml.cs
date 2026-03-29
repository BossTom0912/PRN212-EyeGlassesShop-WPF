using BLL.Constants;
using BLL.Services;
using DAL.Models;
using System.Windows;
using WpfApp_CLassesShop.Session;

namespace WpfApp_CLassesShop
{
    public partial class SupportOrdersWindow : Window
    {
        private readonly SupportStaffService _supportService;

        public SupportOrdersWindow()
        {
            InitializeComponent();
            _supportService = new SupportStaffService();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var account = CurrentSession.LoggedInAccount;
                if (account == null)
                {
                    MessageBox.Show("Phiên đăng nhập đã hết. Vui lòng đăng nhập lại.");
                    new LoginWindow().Show();
                    Close();
                    return;
                }

                if (!AppRoles.IsSupportStaff(account.Role?.Name))
                {
                    MessageBox.Show("Bạn không có quyền vào màn hình Support Staff.");
                    new LoginWindow().Show();
                    Close();
                    return;
                }

                txtWelcome.Text = $"Xin chào, {account.FullName} - Support Staff";
                LoadOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi load màn support:\n\n" + ex, "Lỗi");
            }
        }

        private void LoadOrders()
        {
            try
            {
                dgOrders.ItemsSource = null;
                dgOrders.ItemsSource = _supportService.GetAllOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi load danh sách đơn:\n\n" + ex.Message, "Lỗi");
            }
        }

        private Order? GetSelectedOrder()
        {
            var order = dgOrders.SelectedItem as Order;
            if (order == null)
            {
                MessageBox.Show("Vui lòng chọn một đơn hàng trước.");
                return null;
            }

            return order;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
        }

        private void btnViewDetails_Click(object sender, RoutedEventArgs e)
        {
            var selectedOrder = GetSelectedOrder();
            if (selectedOrder == null) return;

            var detailWindow = new SupportOrderDetailsWindow(selectedOrder.Id);
            detailWindow.ShowDialog();
            LoadOrders();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var currentAccount = CurrentSession.LoggedInAccount;
            var selectedOrder = GetSelectedOrder();

            if (currentAccount == null || selectedOrder == null) return;

            try
            {
                _supportService.ConfirmOrder(selectedOrder.Id, currentAccount.Id);
                MessageBox.Show("Xác nhận đơn thành công.");
                LoadOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xác nhận đơn:\n\n" + ex.Message, "Lỗi");
            }
        }

        private void btnComplete_Click(object sender, RoutedEventArgs e)
        {
            var currentAccount = CurrentSession.LoggedInAccount;
            var selectedOrder = GetSelectedOrder();

            if (currentAccount == null || selectedOrder == null) return;

            try
            {
                _supportService.CompleteOrder(selectedOrder.Id, currentAccount.Id);
                MessageBox.Show("Đã đánh dấu hoàn tất đơn hàng.");
                LoadOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hoàn tất đơn:\n\n" + ex.Message, "Lỗi");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var currentAccount = CurrentSession.LoggedInAccount;
            var selectedOrder = GetSelectedOrder();

            if (currentAccount == null || selectedOrder == null) return;

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn hủy đơn {selectedOrder.OrderCode} không?",
                "Xác nhận hủy đơn",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                string reason = "Support staff hủy đơn vì shop chưa thể xử lý đơn hàng.";
                _supportService.CancelOrder(selectedOrder.Id, currentAccount.Id, reason);

                MessageBox.Show("Hủy đơn thành công.");
                LoadOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hủy đơn:\n\n" + ex.Message, "Lỗi");
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentSession.LoggedInAccount = null;
            new LoginWindow().Show();
            Close();
        }
    }
}