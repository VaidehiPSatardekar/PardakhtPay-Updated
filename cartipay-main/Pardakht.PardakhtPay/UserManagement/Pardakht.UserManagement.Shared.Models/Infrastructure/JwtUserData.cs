namespace Pardakht.UserManagement.Shared.Models.Infrastructure
{

    public class JwtUserData
    {
        public string TenantGuid { get; set; }
        public string PlatformGuid { get; set; }
        public string ParentAccountId { get; set; }
        public string TenantUid { get; set; }
        public int? BrandId { get; set; }
        public int UserType { get; set; }
    }

}
