using BLL.Services;
using System.Windows;

namespace WpfApp_CLassesShop.Admin
{
    public partial class AdminProductEditWindow : Window
    {
        private readonly ProductService _productService;
        private readonly long? _productId;
        private readonly long? _variantId;
        private readonly bool _isEditMode;

        public AdminProductEditWindow()
        {
            InitializeComponent();
            _productService = new ProductService();
            _isEditMode = false;

            Loaded += AdminProductEditWindow_Loaded;
        }

        public AdminProductEditWindow(long productId, long variantId)
        {
            InitializeComponent();
            _productService = new ProductService();
            _productId = productId;
            _variantId = variantId;
            _isEditMode = true;

            Loaded += AdminProductEditWindow_Loaded;
        }

        private void AdminProductEditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadLookups();

            if (_isEditMode)
            {
                Title = "Sửa sản phẩm";
                LoadEditData();
            }
            else
            {
                Title = "Thêm sản phẩm";
            }
        }

        private void LoadLookups()
        {
            cbBrand.ItemsSource = _productService.GetActiveBrands();
            cbCategory.ItemsSource = _productService.GetActiveCategories();
        }

        private void LoadEditData()
        {
            if (!_productId.HasValue || !_variantId.HasValue)
                return;

            var product = _productService.GetProductById(_productId.Value);
            if (product == null)
                throw new Exception("Không tìm thấy sản phẩm.");

            var variant = _productService.GetVariantById(_variantId.Value);
            if (variant == null)
                throw new Exception("Không tìm thấy biến thể.");

            txtProductName.Text = product.Name;
            txtDescription.Text = product.Description ?? "";
            cbBrand.SelectedValue = product.BrandId;
            cbCategory.SelectedValue = product.CategoryId;
            txtBaseImageUrl.Text = product.BaseImageUrl ?? "";
            chkProductActive.IsChecked = product.IsActive;

            txtSku.Text = variant.Sku;
            txtColor.Text = variant.Color;
            txtSize.Text = variant.Size;
            txtMaterial.Text = variant.Material ?? "";
            txtPrice.Text = variant.Price.ToString();
            txtStock.Text = variant.StockQuantity.ToString();
            txtVariantImageUrl.Text = variant.ImageUrl ?? "";
            chkVariantActive.IsChecked = variant.IsActive;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                long brandId = GetSelectedId(cbBrand, "thương hiệu");
                long categoryId = GetSelectedId(cbCategory, "danh mục");

                if (!decimal.TryParse(txtPrice.Text.Trim(), out decimal price))
                    throw new Exception("Giá không hợp lệ.");

                if (!int.TryParse(txtStock.Text.Trim(), out int stock))
                    throw new Exception("Tồn kho không hợp lệ.");

                if (_isEditMode)
                {
                    _productService.UpdateProductAndVariant(
                        _productId!.Value,
                        _variantId!.Value,
                        txtProductName.Text,
                        txtDescription.Text,
                        categoryId,
                        brandId,
                        txtBaseImageUrl.Text,
                        txtSku.Text,
                        txtColor.Text,
                        txtSize.Text,
                        txtMaterial.Text,
                        price,
                        stock,
                        txtVariantImageUrl.Text,
                        chkProductActive.IsChecked == true,
                        chkVariantActive.IsChecked == true
                    );

                    MessageBox.Show("Cập nhật sản phẩm thành công.");
                }
                else
                {
                    _productService.CreateProductWithDefaultVariant(
                        txtProductName.Text,
                        txtDescription.Text,
                        categoryId,
                        brandId,
                        txtBaseImageUrl.Text,
                        txtSku.Text,
                        txtColor.Text,
                        txtSize.Text,
                        txtMaterial.Text,
                        price,
                        stock,
                        txtVariantImageUrl.Text
                    );

                    MessageBox.Show("Thêm sản phẩm thành công.");
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private long GetSelectedId(System.Windows.Controls.ComboBox comboBox, string displayName)
        {
            if (comboBox.SelectedValue == null)
                throw new Exception($"Vui lòng chọn {displayName}.");

            return Convert.ToInt64(comboBox.SelectedValue);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}