using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pardakht.UserManagement.Domain.AuditLog;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;
using InfrastructureModels = Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Domain.Role.Handlers
{
    public class UpdateRoleHandler
    {
        private readonly IRoleRepository roleRepository;
        private readonly IAuditLogManager auditLogManager;
        private ILogger logger;
        public UpdateRoleHandler(IRoleRepository roleRepository, IAuditLogManager auditLogManager, ILogger logger)
        {
            this.roleRepository = roleRepository;
            this.auditLogManager = auditLogManager;
            this.logger = logger;
        }

        public async Task<RoleDto> Handle(RoleDto request, DomainModels.StaffUser staffUser)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var editRecord = await roleRepository.GetByIdWithMappings(request.Id);
            if (editRecord == null)
            {
                throw new Exception($"Role {request.Id} not found");
            }

            var currentRecord = await roleRepository.GetByIdWithMappingsAsNoTracking(request.Id);

            editRecord.IsFixed = request.IsFixed;
            editRecord.Name = request.Name;
            //if (!string.IsNullOrEmpty(editRecord.TenantGuid) && !string.IsNullOrEmpty(request.TenantGuid) && editRecord.TenantGuid != request.TenantGuid)
            //{
            //    // if changing from one tenant to another then then we will have to remove role from applicable users
            //    //RemoveUsersFromRole(role.Id);
            //}
            //if (!string.IsNullOrEmpty(request.TenantGuid))
            //{
            //    editRecord.TenantGuid = request.TenantGuid;
            //}
            //else
            //{
            //    editRecord.TenantGuid = null;
            //}

            // clear any existing mappings
            if (editRecord.RolePermissions != null)
            {
                editRecord.RolePermissions.Clear();
            }

            AddPermissionMappings(request.Permissions, editRecord);

            await roleRepository.Update(editRecord);
            await roleRepository.CommitChanges();


            var newValue = JsonConvert.SerializeObject(editRecord, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            logger.LogWarning("{@message} {@actionType} {@type} {@typeId} {@userId} {@username} {@createdbyaccountId}",
                $"New Role {newValue}",
            ((int)AuditActionType.RoleUpdated).ToString(),
            ((int)AuditType.Role).ToString(),
            editRecord.Id.ToString(),
            staffUser.Id.ToString(), staffUser.Username,staffUser.AccountId);

            await auditLogManager.LogChanges(editRecord, currentRecord, InfrastructureModels.AuditType.Role, InfrastructureModels.AuditActionType.RoleUpdated, staffUser);

            var updated = await roleRepository.GetByIdWithMappingsAsNoTracking(editRecord.Id);

            return RoleMapper.Map(updated);
        }

        public void AddPermissionMappings(IEnumerable<DomainModels.PermissionDto> permissions, InfrastructureModels.Role role)
        {
            if (permissions != null)
            {
                role.RolePermissions = new List<InfrastructureModels.RolePermission>();
                foreach(var permission in permissions)
                {
                    role.RolePermissions.Add(new InfrastructureModels.RolePermission
                    {
                        PermissionId = permission.Id
                    });
                }
            }
        }
    }
}
