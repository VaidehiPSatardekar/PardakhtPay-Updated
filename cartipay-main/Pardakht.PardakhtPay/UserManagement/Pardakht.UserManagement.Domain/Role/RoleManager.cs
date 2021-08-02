using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Domain.AuditLog;
using Pardakht.UserManagement.Domain.Role.Handlers;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.WebService;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.Role
{
    public class RoleManager : IRoleManager
    {
        private readonly IRoleRepository roleRepository;
        private readonly IPermissionRepository permissionRepository;
        private readonly IAuditLogManager auditLogManager;
        private readonly ILogger<RoleManager> logger;
        public RoleManager(IRoleRepository roleRepository, IPermissionRepository permissionRepository, IAuditLogManager auditLogManager,ILogger<RoleManager> logger)
        {
            this.roleRepository = roleRepository;
            this.permissionRepository = permissionRepository;
            this.auditLogManager = auditLogManager;
            this.logger = logger;
        }

        public async Task<IEnumerable<RoleDto>> GetRoles(string platformGuid)
        {
            var roles = roleRepository.GetRolesWithPermissionsAsNoTracking(platformGuid).ToList();

            return RoleMapper.Map(roles);
        }

        public IEnumerable<PermissionGroupDto> GetPermissionsGroups(string platformGuid)
        {
            var result = permissionRepository.GetPermissionGroupsAsNoTracking(platformGuid).ToList();

            return Mapper.Map<List<PermissionGroupDto>>(result);
        }

        public async Task<RoleDto> EditRole(RoleDto request, Shared.Models.WebService.StaffUser actionedByUser)
        {
            var handler = new UpdateRoleHandler(roleRepository, auditLogManager, logger);

            return await handler.Handle(request, actionedByUser);
        }

        public async Task<Shared.Models.Infrastructure.Role> GetByName(string name, string holderTypeId, string platformGuid)
        {
            var roles = await roleRepository.GetAll();

            return roles.Where(r => r.Name.ToLower() == name.ToLower() && r.RoleHolderTypeId == holderTypeId && r.PlatformGuid == platformGuid)
                .FirstOrDefault();
        }

        public async Task<Shared.Models.Infrastructure.Role> GetById(int id)
        {
            var role = await roleRepository.GetByIdWithMappingsAsNoTracking(id);

            return role;
        }

        //public async Task<IEnumerable<RoleDto>> GetUserRoles(int userId)
        //{
        //    return Mapper.Map<IEnumerable<RoleDto>>(await roleRepository.GetUserRoles(userId)); 
        //}

        //public IEnumerable<PermissionDto> GetAllPermissions(string platformGuid)
        //{
        //    var permissions = permissionRepository.GetAllByPlatformAsNoTracking(platformGuid);
        //    var results = new List<PermissionDto>(Mapper.Map<List<PermissionDto>>(permissions));

        //    return results.OrderBy(o => o..GroupName).ThenBy(t => t.Name);
        //}

        public async Task<RoleDto> AddRole(RoleDto request, DomainModels.StaffUser actionedByUser)
        {
            var handler = new AddRoleHandler(roleRepository, auditLogManager,logger);
            return await handler.Handle(request, actionedByUser);
        }

        //public async Task MoveUsersToRole(int fromRoleId, int toRoleId, int tenantId)
        //{
        //    var fromRole = await roleRepository.GetById(fromRoleId);
        //    var toRole = await roleRepository.GetById(toRoleId);

        //    if (fromRole != null && toRole != null)
        //    {
        //        AddUsersToRole(toRole, fromRole.Users);
        //        RemoveUsersFromRoleForTenant(fromRole, tenantId);
        //    }
        //}

        //public async Task<IEnumerable<RoleDTO>> GetAll(int tenantId = 0)
        //{
        //    var roles = await roleRepository.GetAllWithPermissions();

        //    if (tenantId > 0)
        //    {
        //        roles = roles.Where(r => r.TenantId == tenantId);
        //    }

        //    var results = new List<RoleDTO>(AutoMapper.Mapper.Map<List<RoleDTO>>(roles));

        //    // get users for role - will improve this when ef core adds better support for many-many joins
        //    foreach (var role in results)
        //    {
        //        role.Users = AutoMapper.Mapper.Map<List<StaffUserDTO>>(await roleRepository.GetUsersForRole(role.Id));
        //    }

        //    return results.OrderBy(o => o.Name);
        //}

        //public async Task<IEnumerable<PermissionDTO>> GetAllPermissions()
        //{
        //    var permissions = await permissionRepository.GetAll();
        //    var results = new List<PermissionDTO>(AutoMapper.Mapper.Map<List<PermissionDTO>>(permissions));

        //    return results.OrderBy(o => o.GroupName).ThenBy(t => t.Name);
        //}

        //public async Task<Shared.Models.Infrastructure.Role> GetById(int id)
        //{
        //    return await roleRepository.GetByIdWithNoTracking(id);
        //}

        //public async Task<RoleDTO> GetByIdWithUsers(int id)
        //{
        //    var result = AutoMapper.Mapper.Map<RoleDTO>(await roleRepository.GetByIdWithNoTracking(id));
        //    result.Users = AutoMapper.Mapper.Map<List<StaffUserDTO>>(await roleRepository.GetUsersForRole(id));

        //    return result;
        //}


        //public async Task<Shared.Models.Infrastructure.Role> GetByNameForProvider(string name)
        //{
        //    return await GetByName(name, RoleConstants.RoleHolderTypeProvider);
        //}

        //public async Task<Shared.Models.Infrastructure.Role> GetByNameForTenant(string name)
        //{
        //    return await GetByName(name, RoleConstants.RoleHolderTypeTenant);
        //}

        //public async Task<IEnumerable<Permission>> GetPermissionsByRole(int roleId)
        //{
        //    return await permissionRepository.GetByRoleId(roleId);
        //}

        //public async Task<IEnumerable<PermissionGroupDTO>> GetPermissionsGroups()
        //{
        //    var result = await permissionRepository.GetPermissionGroups();

        //    return AutoMapper.Mapper.Map<IEnumerable<PermissionGroupDTO>>(result);
        //}

        //public static RoleDTO[] ReMapUserRoles(Shared.Models.Infrastructure.User user)
        //{
        //    // temp solution to issue with serializing user roles, where multiple roles aren't visible to the UI
        //    // unable to get AutoMapper to custom map without mapping circular references
        //    var userRoles = new List<RoleDTO>();
        //    foreach (var role in user.Roles)
        //    {
        //        var roleDto = new RoleDTO
        //        {
        //            Id = role.Id,
        //            Name = role.Name,
        //            TenantId = role.TenantId,
        //            IsFixed = role.IsFixed,
        //            RoleHolderTypeId = role.RoleHolderTypeId
        //        };
        //        userRoles.Add(roleDto);
        //    }

        //    return userRoles.ToArray();
        //}

        //private void RemoveUsersFromRole(int roleId)
        //{
        //    var users = roleRepository.GetUsersForRole(roleId).Result.ToList();
        //    if (users != null)
        //    {
        //        foreach (var user in users)
        //        {
        //            var role = user.Roles.Where(r => r.Id == roleId).FirstOrDefault();
        //            if (role != null)
        //            {
        //                user.Roles.Remove(role);
        //                userRepository.Update(user);
        //                userRepository.CommitChanges();
        //            }
        //        }
        //    }
        //}

        //private void RemoveUsersFromRoleForTenant(Shared.Models.Infrastructure.Role role, int tenantId)
        //{
        //    var users = roleRepository.GetUsersForRole(role.Id).Result.ToList();
        //    if (users != null)
        //    {
        //        foreach (var user in users)
        //        {
        //            if (user.TenantId.HasValue && user.TenantId == tenantId)
        //            {
        //                user.Roles.Remove(role);
        //                userRepository.Update(user);
        //            }
        //        }
        //        userRepository.CommitChanges();
        //    }
        //}

        //private void AddUsersToRole(Shared.Models.Infrastructure.Role role, ICollection<Shared.Models.Infrastructure.User> users)
        //{
        //    foreach (var user in users)
        //    {
        //        var existingRole = user.Roles.Where(r => r.Id == role.Id).FirstOrDefault();
        //        if (existingRole == null)
        //        {
        //            user.Roles.Add(role);
        //            userRepository.Update(user);
        //            userRepository.CommitChanges();
        //        }
        //    }
        //}
    }
}

