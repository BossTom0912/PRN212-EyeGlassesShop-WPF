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

namespace WpfApp_CLassesShop.Admin
{
    public partial class AdminDashboardWindow : Window
    {
        public AdminDashboardWindow()
        {
            InitializeComponent();
        }

        private void BtnManageUsers_Click(object sender, RoutedEventArgs e)
        {
            new AdminUsersWindow().ShowDialog();
        }

        private void BtnManageProducts_Click(object sender, RoutedEventArgs e)
        {
            // new AdminProductsWindow().ShowDialog();
        }

        private void BtnManageVariants_Click(object sender, RoutedEventArgs e)
        {
            // new AdminVariantsWindow().ShowDialog();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentSession.LoggedInAccount = null;
            new LoginWindow().Show();
            this.Close();
        }
    }
}
