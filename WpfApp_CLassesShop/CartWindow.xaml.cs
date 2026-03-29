using BLL.Constants;
using BLL.Services;
using System.Windows;
using System.Windows.Controls;
using WpfApp_CLassesShop.Session;

namespace WpfApp_CLassesShop
{
    public partial class CartWindow : Window
    {
        public CartWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentSession.LoggedInAccount == null)
            {
                MessageBox.Show("Phiên đăng nhập đã hết. Vui lòng đăng nhập lại.");
                new LoginWindow().Show();
                Close();
                return;
            }

            LoadCartData();
        }

        private void LoadCartData()
        {
            var currentAccount = CurrentSession.LoggedInAccount;
            if (currentAccount == null) return;

            CartService cartService = new CartService();
            var items = cartService.GetCartSummary((int)currentAccount.Id);

            dgCart.ItemsSource = items;

            decimal totalAmount = items.Sum(x => x.TotalPrice);
            txtTotalAmount.Text = $"{totalAmount:N0} VNĐ";
        }

        private void btnCheckout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentAccount = CurrentSession.LoggedInAccount;
                if (currentAccount == null)
                {
                    MessageBox.Show("Phiên đăng nhập đã hết. Vui lòng đăng nhập lại.");
                    new LoginWindow().Show();
                    Close();
                    return;
                }

                if (dgCart.Items.Count == 0)
                {
                    MessageBox.Show("Giỏ hàng của bạn đang trống!",
                                     "Giỏ hàng trống",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Warning);
                    return;
                }

                var checkoutWin = new CheckoutWindow();

                if (checkoutWin.ShowDialog() == true)
                {
                    string name = checkoutWin.ReceiverName;
                    string phone = checkoutWin.Phone;
                    string address = checkoutWin.ShippingAddress;
                    string paymentMethod = checkoutWin.PaymentMethod;

                    OrderService orderService = new OrderService();
                    string orderCode = "ORD" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");

                    orderService.PlaceOrder((int)currentAccount.Id, name, phone, address, orderCode, paymentMethod);

                    var items = dgCart.ItemsSource as IEnumerable<CartItemDisplay>;
                    decimal totalAmount = items != null ? items.Sum(x => x.TotalPrice) : 0;

                    if (paymentMethod.Equals("VNPAY", StringComparison.OrdinalIgnoreCase))
                    {
                        string vnpayUrl = orderService.GenerateVnPayUrl(totalAmount, orderCode);

                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = vnpayUrl,
                            UseShellExecute = true
                        });

                        MessageBoxResult result = MessageBox.Show(
                            "Trình duyệt đã được mở để bạn thanh toán qua VNPAY.\n\n" +
                            "Nếu bạn đã thanh toán thành công, chọn YES.\n" +
                            "Nếu bạn hủy hoặc giao dịch thất bại, chọn NO.",
                            "Xác nhận thanh toán VNPAY",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            orderService.UpdateOrderStatus(
                                orderCode,
                                "WAITING_CONFIRM",
                                currentAccount.Id,
                                "Khách hàng xác nhận đã thanh toán VNPAY.");

                            MessageBox.Show("Thanh toán thành công. Đơn hàng đang chờ support xác nhận.",
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            orderService.UpdateOrderStatus(
                                orderCode,
                                "CANCELLED",
                                currentAccount.Id,  
                                "Khách hàng hủy hoặc thanh toán thất bại qua VNPAY.");

                            MessageBox.Show("Giao dịch chưa hoàn tất. Đơn hàng đã bị hủy.",
                                "Đã hủy thanh toán", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Đặt hàng thành công. Đơn hàng đang ở trạng thái chờ support xác nhận.",
                            "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    LoadCartData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi thanh toán: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDecrease_Click(object sender, RoutedEventArgs e)
        {
            var currentAccount = CurrentSession.LoggedInAccount;
            if (currentAccount == null) return;

            Button btn = sender as Button;
            var selectedItem = btn?.DataContext as CartItemDisplay;
            if (selectedItem != null)
            {
                CartService cartService = new CartService();
                cartService.UpdateQuantity((int)currentAccount.Id, selectedItem.ProductVariantId, -1);
                LoadCartData();
            }
        }

        private void btnIncrease_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentAccount = CurrentSession.LoggedInAccount;
                if (currentAccount == null) return;

                Button btn = sender as Button;
                var selectedItem = btn?.DataContext as CartItemDisplay;
                if (selectedItem != null)
                {
                    CartService cartService = new CartService();
                    cartService.UpdateQuantity((int)currentAccount.Id, selectedItem.ProductVariantId, 1);
                    LoadCartData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            var currentAccount = CurrentSession.LoggedInAccount;
            if (currentAccount == null) return;

            Button btn = sender as Button;
            var selectedItem = btn?.DataContext as CartItemDisplay;
            if (selectedItem != null)
            {
                var confirm = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa toàn bộ '{selectedItem.ProductName}' khỏi giỏ không?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    CartService cartService = new CartService();
                    cartService.RemoveItemCompletely((int)currentAccount.Id, selectedItem.ProductVariantId);
                    LoadCartData();
                }
            }
        }
    }
}