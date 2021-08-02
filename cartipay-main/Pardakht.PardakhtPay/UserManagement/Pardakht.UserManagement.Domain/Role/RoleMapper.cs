using System.Collections.Generic;
using System.Linq;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.Role
{
    public class RoleMapper
    {
        public static IEnumerable<RoleDto> Map(IEnumerable<Shared.Models.Infrastructure.Role> roles)
        {
            if (roles == null)
            {
                return null;
            }

            return roles.Select(r => Map(r));
        }

        public static RoleDto Map(Shared.Models.Infrastructure.Role role)
        {
            if (role == null)
            {
                return null;
            }

            return new RoleDto
            {
                Id = role.Id,
                IsFixed = role.IsFixed,
                Name = role.Name,
                PlatformGuid = role.PlatformGuid,
                RoleHolderTypeId = role.RoleHolderTypeId,
                TenantGuid = role.TenantGuid,
                Permissions = MapPermissions(role.RolePermissions)
            };

            //var permissions = MapPermissions(role.RolePermissions);
        }

        private static List<PermissionDto> MapPermissions(IEnumerable<Shared.Models.Infrastructure.RolePermission> map)
        {
            if (map != null)
            {
                return map.Where(p => p.Permission != null)
                    .Select(rp => new PermissionDto
                    {
                        Id = rp.Permission.Id,
                        Code = rp.Permission.Code,
                        Name = rp.Permission.Name,
                        IsRestricted = rp.Permission.IsRestricted
                    }).ToList();
            }

            return null;
        }
    }
}
