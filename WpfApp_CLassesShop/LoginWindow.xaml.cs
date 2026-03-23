using BLL.Services;
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
using WpfApp_CLassesShop.Session;

namespace WpfApp_CLassesShop
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly AccountService _accountService;

        public LoginWindow()
        {
            InitializeComponent();
            _accountService = new AccountService();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Email và Mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var account = _accountService.Login(email, password);

            if (account != null)
            {
                // Lưu vào Session giống vậy nè mấy ní 
                Session.Session.LoggedInAccount = account;
                MessageBox.Show($"Đăng nhập thành công! Xin chào {account.FullName}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                MainWindow main = new MainWindow();
                main.Show();
                this.Close(); 
            }
            else
            {
                MessageBox.Show("Email hoặc mật khẩu không chính xác. Hoặc tài khoản đã bị khóa!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtGoToRegister_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Mở cửa sổ Đăng ký
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }
    }
}