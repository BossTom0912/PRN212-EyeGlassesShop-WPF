using BLL.Constants;
using BLL.Services;
using System.Windows;
using System.Windows.Input;
using WpfApp_CLassesShop.Admin;
using WpfApp_CLassesShop.Session;

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

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Email và Mật khẩu!",
                    "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var account = _accountService.Login(email, password);

            if (account == null)
            {
                MessageBox.Show("Email hoặc mật khẩu không chính xác. Hoặc tài khoản đã bị khóa!",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CurrentSession.LoggedInAccount = account;

            MessageBox.Show($"Đăng nhập thành công! Xin chào {account.FullName}",
                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

            string roleName = account.Role?.Name ?? "";

            if (AppRoles.IsAdmin(roleName))
            {
                new AdminDashboardWindow().Show();
                this.Close();
                return;
            }

            if (AppRoles.IsSupportStaff(roleName))
            {
                new SupportOrdersWindow().Show();
                this.Close();
                return;
            }

            if (AppRoles.IsCustomer(roleName))
            {
                new MainWindow().Show();
                this.Close();
                return;
            }

            MessageBox.Show($"Role '{roleName}' chưa được map màn hình.",
                "Chưa hỗ trợ", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void txtGoToRegister_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            this.Close();
        }
    }
}