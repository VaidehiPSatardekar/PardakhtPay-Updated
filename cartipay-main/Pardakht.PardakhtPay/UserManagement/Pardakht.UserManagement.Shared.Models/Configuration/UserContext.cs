using System.Linq;

namespace Pardakht.UserManagement.Shared.Models.Configuration
{
    public class UserContext
    {
        public string[] Roles { get; set; }
        public string AccountId { get; set; }
        public string TenantGuid { get; set; }
        public string PlatformGuid { get; set; }
        public string ParentAccountId { get; set; }
        public string Username { get; set; }
        public dynamic UserData { get; set; }

        public bool HasRole(string role)
        {
            if (Roles == null)
            {
                return false;
            }
            return Roles.Contains(role);
        }

        public bool IsProviderUser()
        {
            var tenantGuid = UserData.TenantGuid == null ? string.Empty : UserData.TenantGuid.ToString();
            return string.IsNullOrEmpty(tenantGuid);
        }

    }
}
