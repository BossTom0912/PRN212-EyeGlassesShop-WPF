using BLL.Services;
using DAL.Models;
using System.Windows;
using WpfApp_CLassesShop.Session;

namespace WpfApp_CLassesShop.Admin
{
    public partial class AdminUsersWindow : Window
    {
        private readonly AdminUserService _userService;

        public AdminUsersWindow()
        {
            InitializeComponent();

            if (!CurrentSession.IsAdmin)
            {
                MessageBox.Show("Bạn không có quyền vào màn hình quản lý người dùng.");
                this.Close();
                return;
            }

            _userService = new AdminUserService();
            LoadUsers();
        }

        private void LoadUsers()
        {
            dgUsers.ItemsSource = null;
            dgUsers.ItemsSource = _userService.GetAllUsers();
        }

        private void BtnToggleActive_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsers.SelectedItem is not Account selectedUser)
            {
                MessageBox.Show("Vui lòng chọn một người dùng từ danh sách.");
                return;
            }

            if (selectedUser.Id == CurrentSession.LoggedInAccount?.Id)
            {
                MessageBox.Show("Bạn không thể tự khóa tài khoản của chính mình!");
                return;
            }

            try
            {
                _userService.ToggleUserStatus(selectedUser.Id);
                LoadUsers();
                MessageBox.Show("Cập nhật trạng thái thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật trạng thái user:\n\n" + ex.Message);
            }
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
        }
    }
}