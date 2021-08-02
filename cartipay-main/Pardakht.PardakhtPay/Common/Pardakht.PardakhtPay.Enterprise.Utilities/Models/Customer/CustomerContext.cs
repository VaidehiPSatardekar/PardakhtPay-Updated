namespace Pardakht.PardakhtPay.Enterprise.Utilities.Models.Customer
{
    public class CustomerContext
    {
        public string CustomerGuid { get; set; }
        public string Username { get; set; }
        public CustomerData CustomerData { get; set; }
    }

    public class CustomerData
    {
        public string TenantPlatformMapGuid { get; set; }
        public string LanguageCode { get; set; }
        public string CurrencyCode { get; set; }
        public string OddsDisplay { get; set; }
        public string TimeZone { get; set; }
        public string CustomerGuid { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        public bool RememberMe { get; set; }
    }
}
