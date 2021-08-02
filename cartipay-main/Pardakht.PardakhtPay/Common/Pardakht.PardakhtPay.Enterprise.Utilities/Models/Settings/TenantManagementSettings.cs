namespace Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings
{
    public class TenantManagementSettings : ApiSettings { 
        public int CacheMinutes { get; set; }
        public int? GetTenantBonusListCacheInMinites { get; set; }
        public int? GetTenantPaymentListCacheInMinites { get; set; }
    }
}
