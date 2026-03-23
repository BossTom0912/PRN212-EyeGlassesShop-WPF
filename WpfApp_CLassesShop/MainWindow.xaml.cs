using BLL.Services;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp_CLassesShop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
            if (Session.Session.LoggedInAccount != null)
            {
                txtWelcome.Text = $"Xin chào, {Session.Session.LoggedInAccount.FullName}! Chúc bạn mua sắm vui vẻ.";
            }
            else
            {
                txtWelcome.Text = "Xin chào Quý khách!";
            }
            LoadProductList();
        }

        private void LoadProductList()
        {
            var products = _productService.GetAllProducts();

            dgProducts.ItemsSource = products;
        }

        private void btnAddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (Session.Session.LoggedInAccount == null)
            {
                MessageBox.Show("Vui lòng đăng nhập để thêm hàng vào giỏ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Button btn = sender as Button;
            var selectedProduct = btn.DataContext as DAL.Models.VwProductVariantList;

            if (selectedProduct != null && selectedProduct.VariantId.HasValue)
            {
                int variantId = (int)selectedProduct.VariantId.Value;
                int accountId = (int)Session.Session.LoggedInAccount.Id;

                decimal price = (decimal)selectedProduct.Price;

                try
                {
                    BLL.Services.CartService cartService = new BLL.Services.CartService();

                    cartService.AddToCart(accountId, variantId, 1, price);

                    MessageBox.Show($"Đã thêm '{selectedProduct.ProductName}' vào giỏ hàng thành công!",
                                    "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm vào giỏ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnViewCart_Click(object sender, RoutedEventArgs e)
        {
            if (Session.Session.LoggedInAccount == null)
            {
                MessageBox.Show("Bạn cần đăng nhập để xem giỏ hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CartWindow cartWin = new CartWindow();
            cartWin.ShowDialog(); 
        }

        private void btnOrderHistory_Click(object sender, RoutedEventArgs e)
        {
            var historyWin = new OrderHistoryWindow();
            historyWin.ShowDialog();
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
               Session.Session.LoggedInAccount = null;

                LoginWindow loginWin = new LoginWindow();
                loginWin.Show();

                this.Close();
            }
        }
    }
}