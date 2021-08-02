using System.Collections.Generic;
using System.Linq;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.StaffUser
{
    public class UserMapper
    {
        public static IEnumerable<DomainModels.StaffUser> Map(IEnumerable<Shared.Models.Infrastructure.StaffUser> staffUsers, string tenantPlatformMapGuid, string platformGuid)
        {
            return staffUsers.Select(u => Map(u, tenantPlatformMapGuid, platformGuid)).ToList();
        }

        public static DomainModels.StaffUser Map(Shared.Models.Infrastructure.StaffUser staffUser, string tenantPlatformMapGuid, string platformGuid)
        {
            var platformRoleMappings = new List<DomainModels.StaffUserEditPlatformRoleContainer>();
            foreach (var staffUserUserPlatform in staffUser.UserPlatforms.Where(p => p.PlatformGuid == platformGuid))
            {
                platformRoleMappings.Add(new DomainModels.StaffUserEditPlatformRoleContainer
                {
                    Roles = staffUserUserPlatform.UserPlatformRoles.Select(p=>p.RoleId).ToArray(),
                    PlatformGuid = staffUserUserPlatform.PlatformGuid
                });
            }

            var permissions = staffUser.UserPlatforms
                    .Where(p => p.PlatformGuid == platformGuid)
                    .SelectMany(upr => upr.UserPlatformRoles)
                    .SelectMany(rp => rp.Role.RolePermissions)
                    .Select(p => p.Permission.Code).ToList();

            return new DomainModels.StaffUser
            {
                Id = staffUser.Id,
                PlatformRoleMappings = platformRoleMappings,
                AccountId = staffUser.AccountId,
                Email = staffUser.Email,
                FirstName = staffUser.FirstName,
                LastName = staffUser.LastName,
                TenantGuid = tenantPlatformMapGuid,
                TenantId = staffUser.TenantId,
                UserType = staffUser.UserType,
                Username = staffUser.Username,
                ParentAccountId = staffUser.ParentAccountId,
                IsBlocked = (staffUser.UserSuspensions != null && staffUser.UserSuspensions.Where(s => s.EndDate == null).Count() > 0),
                IsDeleted = staffUser.IsDeleted,
                Permissions = permissions,
                BrandId = staffUser.BrandId
            };
        }
    }
}
