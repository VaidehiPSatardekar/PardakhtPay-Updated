using System;

namespace Pardakht.UserManagement.Shared.Models.WebService
{
    public class SessionConfig
    {
        public int TenantId { get; set; }
        public Guid TenantGUID { get; set; }
        public string BrandName { get; set; }
        public string DefaultLanguageCode { get; set; }
        public string DefaultLanguageName { get; set; }
        public string ActiveDomainName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserDescription { get; set; }
        public int SignedInUserId { get; set; }
    }
}
