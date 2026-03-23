using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_CLassesShop.Session
{
    public static class Session
    {
        // Biến toàn cục lưu trữ thông tin User/ShopOwner/Admin đang đăng nhập với sau này mấy ní tạo trang nào chỉ cần gọi thằng này 1 lần là được 
        public static Account LoggedInAccount { get; set; }
    }
}
