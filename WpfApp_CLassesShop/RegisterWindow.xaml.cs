using BLL.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace WpfApp_CLassesShop
{
    public partial class RegisterWindow : Window
    {
        private readonly AccountService _accountService;

        public RegisterWindow()
        {
            InitializeComponent();
            _accountService = new AccountService();
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string email = txtEmail.Text.Trim();
            string pass = txtPassword.Password;
            string confirmPass = txtConfirmPassword.Password;

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Vui lòng nhập các thông tin bắt buộc (Tên, Email, Mật khẩu)!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (pass != confirmPass)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                bool isSuccess = _accountService.Register(fullName, email, pass, phone);
                if (isSuccess)
                {
                    MessageBox.Show("Đăng ký thành công! Hệ thống đã tạo giỏ hàng cho bạn. Vui lòng đăng nhập.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi đăng ký", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtBackToLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}