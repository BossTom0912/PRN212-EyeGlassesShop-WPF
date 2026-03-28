using BLL.Services;
using System.Windows;
using System.Windows.Input;
using WpfApp_CLassesShop.Session;
using WpfApp_CLassesShop.Admin;

namespace WpfApp_CLassesShop
{
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
                CurrentSession.LoggedInAccount = account;
                MessageBox.Show($"Đăng nhập thành công! Xin chào {account.FullName}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                string roleName = account.Role?.Name ?? "";

                if (roleName == "Admin")
                {
                    AdminDashboardWindow adminWindow = new AdminDashboardWindow();
                    adminWindow.Show();
                }
                else if (roleName == "ShopOwner")
                {
                    MessageBox.Show("Chào mừng bạn đến với giao diện Quản lý Đơn Hàng (Staff) - Đang phát triển", "Staff Dashboard", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MainWindow main = new MainWindow();
                    main.Show();
                }

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