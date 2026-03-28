namespace BLL.Constants
{
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string SupportStaff = "ShopOwner";
        public const string Customer = "User";

        private static string Normalize(string? roleName)
        {
            return string.IsNullOrWhiteSpace(roleName)
                ? string.Empty
                : roleName.Trim()
                          .Replace(" ", "")
                          .Replace("_", "")
                          .ToUpperInvariant();
        }

        public static bool IsAdmin(string? roleName)
        {
            return Normalize(roleName) == "ADMIN";
        }

        public static bool IsSupportStaff(string? roleName)
        {
            var normalized = Normalize(roleName);
            return normalized == "SHOPOWNER"
                || normalized == "SUPPORTSTAFF"
                || normalized == "OWNER";
        }

        public static bool IsCustomer(string? roleName)
        {
            var normalized = Normalize(roleName);
            return normalized == "USER"
                || normalized == "CUSTOMER";
        }
    }
}