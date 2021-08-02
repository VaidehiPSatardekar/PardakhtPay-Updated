using System;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.UserManagement.Domain.AuditLog;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;
using InfrastructureModels = Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Domain.StaffUser.Handlers
{
    public class BlockStaffUserHandler
    {
        private readonly IStaffUserRepository staffUserRepository;
        private readonly IAuditLogManager auditLogManager;

        public BlockStaffUserHandler(IStaffUserRepository staffUserRepository, IAuditLogManager auditLogManager)
        {
            this.staffUserRepository = staffUserRepository;
            this.auditLogManager = auditLogManager;
        }

        public async Task<DomainModels.StaffUser> Handle(int userId, DomainModels.StaffUser actionedByUser, bool block, string tenantPlatformMapGuid, string platformGuid)
        {
            var existing = staffUserRepository.GetUserSuspensions()
                .Where(s => s.UserId == userId && s.EndDate == null)
                .FirstOrDefault();

            if (block)
            {
                if (existing != null)
                {
                    throw new StaffUserValidationException("This user is already blocked");
                }
                var suspension = new InfrastructureModels.UserSuspension
                {
                    CreatedByUserId = actionedByUser.Id,
                    Reason = "User blocked",
                    StartDate = DateTime.UtcNow,
                    UserId = userId
                };

                await staffUserRepository.AddSuspension(suspension);
            }
            else
            {
                // un blocking user
                existing.EndDate = DateTime.UtcNow;
            }

            await staffUserRepository.CommitChanges();


            var result = await staffUserRepository.GetByIdWithMappings(userId);


            await auditLogManager.CreateAuditLogEntry(actionedByUser, InfrastructureModels.AuditType.User
                , block ? InfrastructureModels.AuditActionType.UserBlocked : InfrastructureModels.AuditActionType.UserUnblocked
                , block ? "User blocked" : "User un-blocked", userId);

            return UserMapper.Map(result, tenantPlatformMapGuid, platformGuid);
        }
    }
}
