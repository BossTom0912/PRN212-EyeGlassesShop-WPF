using System.Windows;
using System.Windows.Controls;

namespace WpfApp_CLassesShop
{
    public partial class CheckoutWindow : Window
    {
        public string ReceiverName { get; private set; } = "";
        public string Phone { get; private set; } = "";
        public string ShippingAddress { get; private set; } = "";
        public string PaymentMethod { get; private set; } = "COD";

        public CheckoutWindow()
        {
            InitializeComponent();
            cboPaymentMethod.SelectedIndex = 0;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReceiverName.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin giao hàng!",
                    "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ReceiverName = txtReceiverName.Text.Trim();
            Phone = txtPhone.Text.Trim();
            ShippingAddress = txtAddress.Text.Trim();

            var selectedPayment = cboPaymentMethod.SelectedItem as ComboBoxItem;
            PaymentMethod = selectedPayment?.Tag?.ToString() ?? "COD";

            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}