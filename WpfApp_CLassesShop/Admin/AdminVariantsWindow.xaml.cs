using BLL.Services;
using DAL.Models;
using System.Windows;
using WpfApp_CLassesShop.Session;

namespace WpfApp_CLassesShop.Admin
{
    public partial class AdminVariantsWindow : Window
    {
        private readonly ProductService _productService;

        public AdminVariantsWindow()
        {
            InitializeComponent();
            _productService = new ProductService();

            if (!CurrentSession.IsAdmin)
            {
                MessageBox.Show("Bạn không có quyền vào màn hình quản lý biến thể.");
                Close();
                return;
            }

            LoadVariants();
        }

        private void LoadVariants()
        {
            bool includeInactive = chkShowInactive.IsChecked == true;
            var variants = _productService.GetAllVariantsForAdmin(includeInactive);

            dgVariants.ItemsSource = variants;
            txtSummary.Text = $"Tổng số biến thể: {variants.Count}";
        }

        private VwProductVariantList? GetSelectedRow()
        {
            return dgVariants.SelectedItem as VwProductVariantList;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new AdminVariantEditWindow();
            bool? result = win.ShowDialog();

            if (result == true)
                LoadVariants();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedRow();
            if (selected == null || !selected.VariantId.HasValue)
            {
                MessageBox.Show("Vui lòng chọn một biến thể để sửa.");
                return;
            }

            var win = new AdminVariantEditWindow(selected.VariantId.Value);
            bool? result = win.ShowDialog();

            if (result == true)
                LoadVariants();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedRow();
            if (selected == null || !selected.VariantId.HasValue)
            {
                MessageBox.Show("Vui lòng chọn một biến thể để ẩn.");
                return;
            }

            var confirm = MessageBox.Show(
                $"Bạn có chắc muốn ẩn biến thể SKU '{selected.Sku}' không?",
                "Xác nhận ẩn biến thể",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                _productService.SoftDeleteVariant(selected.VariantId.Value);
                MessageBox.Show("Đã ẩn biến thể thành công.");
                LoadVariants();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi ẩn biến thể: " + ex.Message);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadVariants();
        }

        private void ChkShowInactive_Changed(object sender, RoutedEventArgs e)
        {
            LoadVariants();
        }
    }
}