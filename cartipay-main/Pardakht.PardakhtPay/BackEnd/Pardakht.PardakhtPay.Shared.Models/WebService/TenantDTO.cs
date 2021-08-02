using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class TenantDTO : BaseEntityDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string ContactName { get; set; }

        [Required]
        [MaxLength(100)]
        public string ContactPhoneNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string ContactEmail { get; set; }
    }

    public class TenantSearchDTO
    {
        public int Id { get; set; }

        public string TenantGuid { get; set; }

        public string TenantDomainPlatformMapGuid { get; set; }

        public string TenantName { get; set; }

        public string DomainName { get; set; }

        public string DomainGuid { get; set; }

        public string TenantStatus { get; set; }
    }

    public class TenantCacheDTO
    {
        public int Id { get; set; }

        //public int? TenantDatabaseId { get; set; }

        //public string TenantGuid { get; set; }

        public string TenantPlatformMapGuid { get; set; }

        public int TenantId { get; set; }

        public int PlatformId { get; set; }

        //public string TenantName { get; set; }

        public string PlatformName { get; set; }

        public string PrimaryDomainName { get; set; }

        //public string DomainGuid { get; set; }

        //public string TenantStatus { get; set; }

        public string ConnectionString { get; set; }

        //public string JwtKey { get; set; }

        public string PlatformGuid { get; set; }

        public List<string> DomainNames { get; set; }

        public TenantInformationDTO Tenant { get; set; }
    }

    //public class TenantDatabaseDTO
    //{
    //    public int Id { get; set; }

    //    public string Name { get; set; }

    //    public string ConnectionString { get; set; }

    //    public string TenantDomainPlatformMapGuid { get; set; }
    //}

    public class TenantInformationDTO
    {
        public int Id { get; set; }

        public string BrandName { get; set; }

        public string BusinessName { get; set; }

        public string Description { get; set; }

        public string TenancyName { get; set; }

        public string MainContact { get; set; }

        public string Telephone { get; set; }
    }
}
