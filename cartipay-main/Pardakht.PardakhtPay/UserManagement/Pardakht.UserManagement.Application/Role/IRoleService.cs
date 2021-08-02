using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.Role
{
    public interface IRoleService
    {
        Task<WebResponse<IEnumerable<RoleDto>>> GetRoles(string platformGuid, string tenantGuid);
        WebResponse<IEnumerable<PermissionGroupDto>> GetPermissionsGroups(string platformGuid);
        Task<WebResponse<RoleDto>> UpdateRole(RoleDto request);
        Task<WebResponse<RoleDto>> AddRole(RoleDto model);
    }
}