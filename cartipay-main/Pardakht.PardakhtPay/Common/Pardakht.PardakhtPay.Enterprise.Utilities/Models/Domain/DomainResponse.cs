using System;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Models.Domain
{
    public class DomainResponse
    {
        public int Id { get; set; }
        public string DomainGuid { get; set; }
        public string DomainAddress { get; set; }
        public string ZoneId { get; set; }
        public string ServerIpAddress { get; set; }
        public string TenantPlatformMapGuid { get; set; }
        public int DomainStatus { get; set; }
        public string DomainStatusStr { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedAtStr { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedAtStr { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string RegistrationDateStr { get; set; }
        public int SecurityLevel { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsProviderDomain { get; set; }
        public bool IsAutoRenew { get; set; }
        public int PurchasePeriod { get; set; }
        public bool IsPrivateDomain { get; set; }
        public bool IsTenantDomain { get; set; }
        public bool IsTenantContactInfo { get; set; }
        public bool IsAffiliationDomain { get; set; }
        public int? TenantPlatformMapBrandId { get; set; }
        public string PlatformGuid { get; set; }
    }

}
