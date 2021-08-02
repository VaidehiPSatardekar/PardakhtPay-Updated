using System.Collections.Generic;
using System.Linq;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Models.User
{
    public class UserContext
    {
        public SecurityContext OriginalCallerContext { get; set; }
        public SecurityContext ProxyCallerContext { get; set; }

        public bool HasEmbeddedUser
        {
            get { return OriginalCallerContext != null; }
        }

        /// <summary>
        /// Gets the user context applicable - if there is an embedded user (user who logged in) then this will take precedence
        /// otherwise the proxy user (usually an api key except when called without using a proxy api in which case it will be the user who logged in)
        /// </summary>
        public SecurityContext ActiveContext
        {
            get { return (OriginalCallerContext != null && OriginalCallerContext.User != null) ? OriginalCallerContext : ProxyCallerContext; }
        }
    }

    public class SecurityContext
    {
        public TokenInfo TokenInfo { get; set; }
        public StaffUser User { get; set; }

        public bool IsGlobalApiKey
        {
            get { return IsProviderUser && User != null && User.UserType == UserType.ApiUser; }
        }

        public bool HasRole(string role)
        {
            if (IsGlobalApiKey)
            {
                // TODO: we may need to review this - perhaps each global api key needs to be added to specific roles / permissions
                return true;
            }

            if (TokenInfo.Roles == null)
            {
                return false;
            }

            return TokenInfo.Roles.Contains(role);
        }

        public bool IsProviderUser
        {
            get { return string.IsNullOrEmpty(TenantGuid); }
        }

        public string TenantGuid
        {
            get { return TokenInfo?.UserData?.TenantGuid; }
        }

        public string PlatformGuid
        {
            get { return TokenInfo?.UserData?.PlatformGuid; }
        }

        public string ParentAccountId
        {
            get { return TokenInfo?.UserData?.ParentAccountId; }
        }

        public string AccountId
        {
            get { return TokenInfo?.AccountId; }
        }

        public string Username
        {
            get { return TokenInfo?.Username; }
        }
    }

    public class TokenInfo
    {
        public string AccountId { get; set; }
        public string Username { get; set; }
        public UserData UserData { get; set; }
        public string[] Roles { get; set; }
    }

    public class UserData
    {
        public string TenantGuid { get; set; }
        public string PlatformGuid { get; set; }
        public string ParentAccountId { get; set; }
        public int UserType { get; set; }
    }

    // copied from UM
    public class StaffUser
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TenantGuid { get; set; } // this is the tenant-platform mapping guid
        public int? TenantId { get; set; } // this is the actual tenant that owns the platform mapping
        public UserType UserType { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsDeleted { get; set; }
        public string ParentAccountId { get; set; }
        public int? BrandId { get; set; }
        public ICollection<StaffUserEditPlatformRoleContainer> PlatformRoleMappings { get; set; }
        public ICollection<string> Permissions { get; set; }

        public bool HasPermissionForTask(string permissionCode)
        {
            if (Permissions != null)
            {
                return (Permissions.Any(p => p == permissionCode));
            }

            return false;
        }
    }

    public class StaffUserEditPlatformRoleContainer
    {
        public string PlatformGuid { get; set; }
        public ICollection<int> Roles { get; set; }
    }

    public enum UserType
    {
        StaffUser = 0,
        ApiUser = 1,
        AffiliateUser = 2
    }
}
