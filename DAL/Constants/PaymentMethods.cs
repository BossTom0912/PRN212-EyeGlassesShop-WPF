namespace DAL.Constants
{
    public static class PaymentMethods
    {
        public const string COD = "COD";
        public const string VNPAY = "VNPAY";

        public static string Normalize(string? value)
        {
            var normalized = value?.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(normalized))
                throw new Exception("Phương thức thanh toán không được để trống.");

            if (normalized != COD && normalized != VNPAY)
                throw new Exception("Phương thức thanh toán không hợp lệ.");

            return normalized;
        }
    }
}