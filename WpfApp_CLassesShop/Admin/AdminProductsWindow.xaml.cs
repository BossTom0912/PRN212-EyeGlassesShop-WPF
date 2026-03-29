using BLL.Services;
using DAL.Models;
using System.Windows;
using WpfApp_CLassesShop.Session;

namespace WpfApp_CLassesShop.Admin
{
    public partial class AdminProductsWindow : Window
    {
        private readonly ProductService _productService;

        public AdminProductsWindow()
        {
            InitializeComponent();
            _productService = new ProductService();

            if (!CurrentSession.IsAdmin)
            {
                MessageBox.Show("Bạn không có quyền vào màn hình quản lý sản phẩm.");
                Close();
                return;
            }

            LoadProducts();
        }

        private void LoadProducts()
        {
            bool includeInactive = chkShowInactive.IsChecked == true;
            var products = _productService.GetAllProductsForAdmin(includeInactive);

            dgProducts.ItemsSource = products;
            txtSummary.Text = $"Tổng số bản ghi: {products.Count}";
        }

        private VwProductVariantList? GetSelectedRow()
        {
            return dgProducts.SelectedItem as VwProductVariantList;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new AdminProductEditWindow();
            bool? result = win.ShowDialog();

            if (result == true)
            {
                LoadProducts();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedRow();
            if (selected == null || !selected.ProductId.HasValue || !selected.VariantId.HasValue)
            {
                MessageBox.Show("Vui lòng chọn một dòng sản phẩm để sửa.");
                return;
            }

            var win = new AdminProductEditWindow(selected.ProductId.Value, selected.VariantId.Value);
            bool? result = win.ShowDialog();

            if (result == true)
            {
                LoadProducts();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedRow();
            if (selected == null || !selected.ProductId.HasValue)
            {
                MessageBox.Show("Vui lòng chọn một dòng sản phẩm để ẩn.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn ẩn sản phẩm '{selected.ProductName}' không?",
                "Xác nhận ẩn sản phẩm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                _productService.SoftDeleteProduct(selected.ProductId.Value);
                MessageBox.Show("Đã ẩn sản phẩm thành công.");
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi ẩn sản phẩm: " + ex.Message);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        private void ChkShowInactive_Changed(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }
    }
}