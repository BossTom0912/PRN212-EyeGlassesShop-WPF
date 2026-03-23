using DAL.Models;
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
    /// Interaction logic for OrderHistoryWindow.xaml
    /// </summary>
    public partial class OrderHistoryWindow : Window
    {
        private int _currentAccountId;

        public OrderHistoryWindow()
        {
            InitializeComponent();

            _currentAccountId = (int)Session.Session.LoggedInAccount.Id;

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
            catch (System.Exception ex)
            {
                MessageBox.Show("Lỗi tải lịch sử đơn hàng: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnViewDetails_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var selectedOrder = button.DataContext as Order; 

            if (selectedOrder != null)
            {
                var detailsWin = new OrderDetailsWindow((int)selectedOrder.Id);
                detailsWin.ShowDialog();
            }
        }
    }
}
