using BLL.Services;
using System.Windows;

namespace WpfApp_CLassesShop.Admin
{
    public partial class AdminVariantEditWindow : Window
    {
        private readonly ProductService _productService;
        private readonly long? _variantId;
        private readonly bool _isEditMode;

        public AdminVariantEditWindow()
        {
            InitializeComponent();
            _productService = new ProductService();
            _isEditMode = false;

            Loaded += AdminVariantEditWindow_Loaded;
        }

        public AdminVariantEditWindow(long variantId)
        {
            InitializeComponent();
            _productService = new ProductService();
            _variantId = variantId;
            _isEditMode = true;

            Loaded += AdminVariantEditWindow_Loaded;
        }

        private void AdminVariantEditWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isEditMode)
            {
                Title = "Sửa biến thể";
                LoadEditData();
            }
            else
            {
                Title = "Thêm biến thể";
                LoadProductsForCreate();
            }
        }

        private void LoadProductsForCreate()
        {
            cbProduct.ItemsSource = _productService.GetActiveProducts();
            cbProduct.Visibility = Visibility.Visible;
            txtProductReadonly.Visibility = Visibility.Collapsed;
        }

        private void LoadEditData()
        {
            if (!_variantId.HasValue)
                return;

            var variant = _productService.GetVariantById(_variantId.Value);
            if (variant == null)
                throw new Exception("Không tìm thấy biến thể.");

            var product = _productService.GetProductById(variant.ProductId);
            if (product == null)
                throw new Exception("Không tìm thấy sản phẩm của biến thể.");

            cbProduct.Visibility = Visibility.Collapsed;
            txtProductReadonly.Visibility = Visibility.Visible;
            txtProductReadonly.Text = product.Name;

            txtSku.Text = variant.Sku;
            txtColor.Text = variant.Color;
            txtSize.Text = variant.Size;
            txtMaterial.Text = variant.Material ?? "";
            txtPrice.Text = variant.Price.ToString();
            txtStock.Text = variant.StockQuantity.ToString();
            txtImageUrl.Text = variant.ImageUrl ?? "";
            chkVariantActive.IsChecked = variant.IsActive;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!decimal.TryParse(txtPrice.Text.Trim(), out decimal price))
                    throw new Exception("Giá không hợp lệ.");

                if (!int.TryParse(txtStock.Text.Trim(), out int stock))
                    throw new Exception("Tồn kho không hợp lệ.");

                if (_isEditMode)
                {
                    _productService.UpdateVariant(
                        _variantId!.Value,
                        txtSku.Text,
                        txtColor.Text,
                        txtSize.Text,
                        txtMaterial.Text,
                        price,
                        stock,
                        txtImageUrl.Text,
                        chkVariantActive.IsChecked == true
                    );

                    MessageBox.Show("Cập nhật biến thể thành công.");
                }
                else
                {
                    if (cbProduct.SelectedValue == null)
                        throw new Exception("Vui lòng chọn sản phẩm.");

                    long productId = Convert.ToInt64(cbProduct.SelectedValue);

                    _productService.CreateVariant(
                        productId,
                        txtSku.Text,
                        txtColor.Text,
                        txtSize.Text,
                        txtMaterial.Text,
                        price,
                        stock,
                        txtImageUrl.Text
                    );

                    MessageBox.Show("Thêm biến thể thành công.");
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}