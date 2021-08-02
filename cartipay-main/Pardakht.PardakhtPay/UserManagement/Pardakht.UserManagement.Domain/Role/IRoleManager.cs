using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.UserManagement.Shared.Models.WebService;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.Role
{
    public interface IRoleManager 
    {
        //Task<RoleDto> GetAdminRole(string platformGuid);
        Task<IEnumerable<RoleDto>> GetRoles(string platformGuid);
        IEnumerable<PermissionGroupDto> GetPermissionsGroups(string platformGuid);
        Task<RoleDto> EditRole(RoleDto request, DomainModels.StaffUser actionedByUser);
        Task<RoleDto> AddRole(RoleDto request, DomainModels.StaffUser actionedByUser);
        Task<Shared.Models.Infrastructure.Role> GetById(int id);
        Task<Shared.Models.Infrastructure.Role> GetByName(string name, string holderTypeId, string platformGuid);
        //Task<IEnumerable<RoleDto>> GetUserRoles(int userId);
        //IEnumerable<PermissionDto> GetAllPermissions(string platformGuid);





        //Task<IEnumerable<RoleDto>> GetAll(int tenantId = 0);
        //Task<Shared.Models.Infrastructure.Role> GetByName(string name, string holderTypeId);
        //Task<Shared.Models.Infrastructure.Role> GetByNameForTenant(string name);
        //Task<Shared.Models.Infrastructure.Role> GetByNameForProvider(string name);
        //Task<Shared.Models.Infrastructure.Role> GetById(int id);
        //Task<RoleDTO> GetByIdWithUsers(int id);
        //Task<IEnumerable<Shared.Models.Infrastructure.Permission>> GetPermissionsByRole(int roleId);
        //Task<IEnumerable<PermissionDTO>> GetAllPermissions();
        //Task<RoleDTO> CreateRole(RoleDTO role, string actionedByAccountId);
        //Task<IEnumerable<PermissionGroupDTO>> GetPermissionsGroups();
        //Task MoveUsersToRole(int fromRoleId, int toRoleId, int tenantId);
    }
}
