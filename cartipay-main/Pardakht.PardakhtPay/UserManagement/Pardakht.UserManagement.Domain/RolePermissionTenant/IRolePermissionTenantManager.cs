using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.RolePermissionTenant
{
    public interface  IRolePermissionTenantManager : IBasicManager<RolePermissionTenantMapDto>
    {
        Task<IEnumerable<RolePermissionTenantMapDto>> GetTenantRolePermissions(int tenantId);
        Task<IEnumerable<RolePermissionTenantMapDto>> CreateUpdateTenantRolePermissions(int tenantId, RolePermissionTenantMapDto[] model);

        //  Task<IEnumerable<string>> GetPermissionCodes(int tenantId,int[] roleId);
    }
}
