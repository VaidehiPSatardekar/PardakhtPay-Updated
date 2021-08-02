using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pardakht.UserManagement.Domain.AuditLog;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;
using InfrastructureModels = Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Domain.Role.Handlers
{
    public class AddRoleHandler
    {
        private readonly IRoleRepository roleRepository;
        private readonly IAuditLogManager auditLogManager;
        private ILogger logger;

        public AddRoleHandler(IRoleRepository roleRepository, IAuditLogManager auditLogManager,ILogger logger)
        {
            this.roleRepository = roleRepository;
            this.auditLogManager = auditLogManager;
            this.logger = logger;
        }

        public async Task<DomainModels.RoleDto> Handle(DomainModels.RoleDto request, DomainModels.StaffUser actionedByUser)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var currentRecord = new InfrastructureModels.Role
            {
                IsFixed = request.IsFixed,
                Name = request.Name,
                RoleHolderTypeId = request.RoleHolderTypeId,
                PlatformGuid = request.PlatformGuid,
                TenantGuid = request.TenantGuid
            };

            currentRecord = await roleRepository.Create(currentRecord);
            await roleRepository.CommitChanges();

            var newValue = JsonConvert.SerializeObject(currentRecord, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            logger.LogWarning("{@message} {@actionType} {@type} {@typeId} {@userId} {@username} {@createdbyaccountId}",
                                    $"New Role {newValue}",
                                ((int)AuditActionType.RoleCreated).ToString(),
                                ((int)AuditType.Role).ToString(),
                                currentRecord.Id.ToString(),
                                actionedByUser.Id.ToString(), actionedByUser.Username, actionedByUser.AccountId);

            var updated = await roleRepository.GetByIdWithMappingsAsNoTracking(currentRecord.Id);
            updated.RolePermissions = updated.RolePermissions.Count == 0 ? null : updated.RolePermissions;

            await auditLogManager.LogChanges(updated, currentRecord, InfrastructureModels.AuditType.Role, InfrastructureModels.AuditActionType.RoleCreated, actionedByUser);

            return RoleMapper.Map(updated);
        }
    }
}
