using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pardakht.UserManagement.Shared.Models.WebService
{
    public class ConfigureTenantUsersRequest
    {
        [Required]
        public int TenantId { get; set; }
        [Required]
        public string TenantName { get; set; }
        [Required]
        public string PlatformName { get; set; }
        [Required]
        public string TenantPlatformMapGuid { get; set; }
        [Required]
        public string AdminUserEmail { get; set; }
        [Required]
        public string PlatformGuid { get; set; }
    }

    public class ConfigureTenantUsersResponse
    {
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }
        public string ApiKey { get; set; }
    }

    public class Tenant
    {
        public int Id { get; set; }
        public ICollection<TenantPlatformMap> PlatformMappings { get; set; }
        //public string BusinessName { get; set; }
        //public string Description { get; set; }
        public string TenancyName { get; set; }
        //public TenantDto Tenant { get; set; }
        //public string MainContact { get; set; }
        //public string Telephone { get; set; }
        //public string Email { get; set; }
        public TenantPlatformMapLanguages[] Languages { get; set; }
        public string PrimaryDomainName { get; set; }
    }
    public class TenantPlatformMapLanguages
    {
        public bool IsDefault { get; set; }
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class TenantPlatformMap
    {
        //public int Id { get; set; }
        public string TenantPlatformMapGuid { get; set; }
        //public int TenantId { get; set; }
        public TenantDto Tenant { get; set; }
        //public int PlatformId { get; set; }
        public string PlatformGuid { get; set; }
        //public string PlatformName { get; set; }
        public string PrimaryDomainName { get; set; }
        //public string SubDomain { get; set; }
        //public string[] DomainNames { get; set; }
        public string BrandName { get; set; }
        public string PreferenceConfig { get; set; }
        public string TenancyName { get; set; }
        //public ICollection<MappedProduct> Products { get; set; }
        //public ICollection<MappedLookupItem> Currencies { get; set; }
        //public ICollection<MappedLookupItem> Languages { get; set; }
        //public ICollection<MappedLookupItem> Countries { get; set; }
        //public ICollection<MappedLookupItem> TimeZones { get; set; }
    }


    public class TenantDetail
    {
        public int Id { get; set; }
        public ICollection<TenantPlatformMap> PlatformMappings { get; set; }
        //public string BusinessName { get; set; }
        //public string Description { get; set; }
        public TenantDto Tenant { get; set; }
        //public TenantDto Tenant { get; set; }
        //public string MainContact { get; set; }
        //public string Telephone { get; set; }
        //public string Email { get; set; }
        public TenantPlatformMapLanguages[] Languages { get; set; }
        public string PrimaryDomainName { get; set; }
    }

    public class TenantDto
    {
        public string Email { get; set; }
        public string TenancyName { get; set; }
        public string BusinessName { get; set; }

    }


}
