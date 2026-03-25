using BLL.Services;
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
    /// Interaction logic for CartWindow.xaml
    /// </summary>
    public partial class CartWindow : Window
    {
        public CartWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Session.Session.LoggedInAccount != null)
            {
                LoadCartData();
            }
        }

        private void LoadCartData()
        {
            CartService cartService = new CartService();
            var items = cartService.GetCartSummary((int)Session.Session.LoggedInAccount.Id);

            dgCart.ItemsSource = items;

            decimal totalAmount = items.Sum(x => x.TotalPrice);
            txtTotalAmount.Text = $"{totalAmount:N0} VNĐ";
        }

        private void btnCheckout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgCart.Items.Count == 0)
                {
                    MessageBox.Show("Giỏ hàng của bạn đang trống! Hãy chọn món đồ yêu thích trước khi thanh toán nhé.",
                                     "Giỏ hàng trống", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var checkoutWin = new CheckoutWindow();

                if (checkoutWin.ShowDialog() == true)
                {
                    string name = checkoutWin.ReceiverName;
                    string phone = checkoutWin.Phone;
                    string address = checkoutWin.ShippingAddress;

                    OrderService orderService = new OrderService();

                    string orderCode = "ORD" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");

                    orderService.PlaceOrder((int)Session.Session.LoggedInAccount.Id, name, phone, address, orderCode, "COD");

                    var items = dgCart.ItemsSource as IEnumerable<BLL.Services.CartItemDisplay>;
                    decimal totalAmount = items != null ? items.Sum(x => x.TotalPrice) : 100000;

                    string vnpayUrl = orderService.GenerateVnPayUrl(totalAmount, orderCode);

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = vnpayUrl,
                        UseShellExecute = true
                    });

                    MessageBoxResult result = MessageBox.Show(
                        "Trình duyệt đã được mở để bạn thanh toán qua VNPAY.\n\n" +
                        "👉 Nếu bạn ĐÃ thanh toán thành công, vui lòng chọn 'Yes'.\n" +
                        "👉 Nếu bạn HỦY hoặc giao dịch bị lỗi, vui lòng chọn 'No'.",
                        "Xác nhận thanh toán VNPAY",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        orderService.UpdateOrderStatus(orderCode, "APPROVED");

                        MessageBox.Show("🎉 Tuyệt vời! Cảm ơn bạn đã thanh toán. Đơn hàng sẽ sớm được xử lý và giao đến bạn!",
                                         "Thanh toán thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        orderService.UpdateOrderStatus(orderCode, "CANCELLED");

                        MessageBox.Show("Giao dịch chưa hoàn tất hoặc đã bị hủy. Đơn hàng này sẽ bị hủy bỏ.",
                                         "Đã hủy thanh toán", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                    LoadCartData();
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;

                // Móc lỗi sâu bên trong (Inner Exception) ra để xem Database đang chửi gì
                if (ex.InnerException != null)
                {
                    errorMessage += "\n\nCHI TIẾT TỪ DATABASE:\n" + ex.InnerException.Message;
                }

                MessageBox.Show($"Lỗi thanh toán: {errorMessage}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //Xóa:
        // Nút Trừ
        private void btnDecrease_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var selectedItem = btn.DataContext as BLL.Services.CartItemDisplay;
            if (selectedItem != null)
            {
                CartService cartService = new CartService();
                cartService.UpdateQuantity((int)Session.Session.LoggedInAccount.Id, selectedItem.ProductVariantId, -1);
                LoadCartData(); 
            }
        }

        // Nút Cộng
        private void btnIncrease_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var selectedItem = btn.DataContext as BLL.Services.CartItemDisplay;
            if (selectedItem != null)
            {
                CartService cartService = new CartService();
                cartService.UpdateQuantity((int)Session.Session.LoggedInAccount.Id, selectedItem.ProductVariantId, 1);
                LoadCartData(); 
            }
        }

        // Nút Xóa hẳn 
        private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var selectedItem = btn.DataContext as BLL.Services.CartItemDisplay;
            if (selectedItem != null)
            {
                var confirm = MessageBox.Show($"Bạn có chắc chắn muốn xóa toàn bộ '{selectedItem.ProductName}' khỏi giỏ không?",
                                              "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (confirm == MessageBoxResult.Yes)
                {
                    CartService cartService = new CartService();
                    cartService.RemoveItemCompletely((int)Session.Session.LoggedInAccount.Id, selectedItem.ProductVariantId);
                    LoadCartData();
                }
            }
        }


    }
}