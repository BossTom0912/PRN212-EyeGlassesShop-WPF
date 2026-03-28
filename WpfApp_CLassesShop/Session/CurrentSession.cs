using BLL.Constants;
using DAL.Models;

namespace WpfApp_CLassesShop.Session
{
    public static class CurrentSession
    {
        public static Account? LoggedInAccount { get; set; }

        public static bool IsAdmin => AppRoles.IsAdmin(LoggedInAccount?.Role?.Name);
        public static bool IsSupportStaff => AppRoles.IsSupportStaff(LoggedInAccount?.Role?.Name);
        public static bool IsCustomer => AppRoles.IsCustomer(LoggedInAccount?.Role?.Name);
    }
}