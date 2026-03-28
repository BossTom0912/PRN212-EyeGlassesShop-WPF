using BLL.Services;
using DAL.Models;
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
    public partial class AdminUsersWindow : Window
    {
        private readonly AdminUserService _userService;

        public AdminUsersWindow()
        {
            InitializeComponent();
            _userService = new AdminUserService();
            LoadUsers();
        }

        private void LoadUsers()
        {
            dgUsers.ItemsSource = _userService.GetAllUsers();
        }

        private void BtnToggleActive_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is Account selectedUser)
            {
                if (selectedUser.Id == CurrentSession.LoggedInAccount?.Id)
                {
                    MessageBox.Show("Bạn không thể tự khóa tài khoản của chính mình!");
                    return;
                }

                _userService.ToggleUserStatus(selectedUser.Id);
                LoadUsers();
                MessageBox.Show("Cập nhật trạng thái thành công!");
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một người dùng từ danh sách.");
            }
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }
    }
}
