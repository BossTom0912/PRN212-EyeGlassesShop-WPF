using System.Windows;
using WpfApp_CLassesShop.Session;

namespace WpfApp_CLassesShop.Admin
{
    public partial class AdminDashboardWindow : Window
    {
        public AdminDashboardWindow()
        {
            InitializeComponent();

            if (!CurrentSession.IsAdmin)
            {
                MessageBox.Show("Bạn không có quyền vào màn hình Admin.");
                new LoginWindow().Show();
                this.Close();
            }
        }

        private void BtnManageUsers_Click(object sender, RoutedEventArgs e)
        {
            new AdminUsersWindow().ShowDialog();
        }

        private void BtnManageProducts_Click(object sender, RoutedEventArgs e)
        {
            new AdminProductsWindow().ShowDialog();
        }

        private void BtnManageVariants_Click(object sender, RoutedEventArgs e)
        {
            new AdminVariantsWindow().ShowDialog();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentSession.LoggedInAccount = null;
            new LoginWindow().Show();
            this.Close();
        }
    }
}