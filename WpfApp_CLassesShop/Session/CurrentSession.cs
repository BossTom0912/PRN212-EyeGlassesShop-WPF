using DAL.Models;

namespace WpfApp_CLassesShop.Session
{
    public static class CurrentSession
    {
        public static Account? LoggedInAccount { get; set; }

        public static bool IsAdmin => LoggedInAccount?.Role?.Name == "Admin";
        public static bool IsSupportStaff => LoggedInAccount?.Role?.Name == "ShopOwner";
        public static bool IsCustomer => LoggedInAccount?.Role?.Name == "User";
    }
}