using BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp_CLassesShop
{
    /// <summary>
    /// Interaction logic for OrderDetailsWindow.xaml
    /// </summary>
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
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi tải chi tiết đơn hàng: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnReOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int currentUserId = (int)Session.Session.LoggedInAccount.Id;

                OrderService orderService = new OrderService();

                orderService.AddOldOrderToCart(_currentOrderId, currentUserId);

                MessageBoxResult result = MessageBox.Show(
                    "Đã thêm toàn bộ sản phẩm của đơn hàng này vào Giỏ hàng của bạn!\n\nBạn có muốn mở Giỏ hàng để thanh toán ngay không?",
                    "Thêm vào giỏ thành công",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
                {
                    var cartWin = new CartWindow();
                    cartWin.Show();

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi mua lại: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}