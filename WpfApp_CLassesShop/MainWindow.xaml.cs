using BLL.Constants;
using BLL.Services;
using System.Windows;
using System.Windows.Controls;
using WpfApp_CLassesShop.Session;

namespace WpfApp_CLassesShop
{
    public partial class MainWindow : Window
    {
        private readonly ProductService _productService;

        public MainWindow()
        {
            InitializeComponent();
            _productService = new ProductService();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var account = CurrentSession.LoggedInAccount;
            if (account == null)
            {
                MessageBox.Show("Phiên đăng nhập đã hết. Vui lòng đăng nhập lại.");
                new LoginWindow().Show();
                this.Close();
                return;
            }

            if (!AppRoles.IsCustomer(account.Role?.Name))
            {
                MessageBox.Show("Role hiện tại không được phép vào màn hình mua hàng.");
                new LoginWindow().Show();
                this.Close();
                return;
            }

            txtWelcome.Text = $"Xin chào, {account.FullName}! Chúc bạn mua sắm vui vẻ.";
            LoadProductList();
        }

        private void LoadProductList()
        {
            var products = _productService.GetAllProducts();
            dgProducts.ItemsSource = products;
        }

        private void btnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSession.LoggedInAccount == null)
            {
                MessageBox.Show("Vui lòng đăng nhập để thêm hàng vào giỏ!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Button btn = sender as Button;
            var selectedProduct = btn?.DataContext as DAL.Models.VwProductVariantList;

            if (selectedProduct != null && selectedProduct.VariantId.HasValue)
            {
                int variantId = (int)selectedProduct.VariantId.Value;
                int accountId = (int)CurrentSession.LoggedInAccount.Id;
                decimal price = selectedProduct.Price ?? 0;

                try
                {
                    CartService cartService = new CartService();
                    cartService.AddToCart(accountId, variantId, 1, price);

                    MessageBox.Show($"Đã thêm '{selectedProduct.ProductName}' vào giỏ hàng thành công!",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm vào giỏ: {ex.Message}",
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnViewCart_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSession.LoggedInAccount == null)
            {
                MessageBox.Show("Bạn cần đăng nhập để xem giỏ hàng!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            new CartWindow().ShowDialog();
        }

        private void btnOrderHistory_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSession.LoggedInAccount == null)
            {
                MessageBox.Show("Bạn cần đăng nhập để xem lịch sử đơn hàng!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            new OrderHistoryWindow().ShowDialog();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất khỏi hệ thống không?",
                "Xác nhận thoát",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                CurrentSession.LoggedInAccount = null;
                new LoginWindow().Show();
                this.Close();
            }
        }
    }
}