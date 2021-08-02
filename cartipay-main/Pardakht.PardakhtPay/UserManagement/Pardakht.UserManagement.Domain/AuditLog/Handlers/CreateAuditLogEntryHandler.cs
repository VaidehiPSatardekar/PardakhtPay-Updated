using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.AuditLog.Handlers
{
    public class CreateAuditLogEntryHandler
    {
        private IAuditLogRepository auditLogRepository;
        private ILogger logger;
        public CreateAuditLogEntryHandler(IAuditLogRepository auditLogRepository, ILogger logger)
        {
            this.auditLogRepository = auditLogRepository;
            this.logger = logger;
        }

        public async Task Handle(DomainModels.StaffUser staffUser, AuditType auditType, AuditActionType auditActionType,string text , int userId, DomainModels.AuditLogDTO auditLogDTO)
        {
            if (auditActionType == AuditActionType.UserPerformanceActivity || auditActionType == AuditActionType.UserLogin || auditActionType == AuditActionType.UserLogout)
            {
                logger.LogWarning("{@message} {@actionType} {@type} {@typeId} {@userId} {@username}",
                                    text,
                                    ((int)auditActionType).ToString(),
                                    ((int)auditType).ToString(),
                                    staffUser.Id.ToString(),
                                    userId.ToString(),staffUser.Username);

                if (auditLogDTO == null)
                {
                    var auditLog = await auditLogRepository.Create(new Shared.Models.Infrastructure.AuditLog
                    {
                        Message = text,
                        Type = auditType,
                        ActionType = auditActionType,
                        TypeId = staffUser.Id,
                        DateTime = DateTime.UtcNow,
                        UserId = userId,
                    });
                }
                else
                {
                    var auditLog = await auditLogRepository.Create(new Shared.Models.Infrastructure.AuditLog
                    {
                        Message = text,
                        Type = auditType,
                        ActionType = auditActionType,
                        TypeId = staffUser.Id,
                        DateTime = DateTime.UtcNow,
                        UserId = userId,
                        ActiveTime = auditLogDTO.ActiveTime,
                        IdleTime = auditLogDTO.IdleTime,
                        IsActive = auditLogDTO.IsActive,
                        LogonTime = auditLogDTO.LogonTime,
                        PlatformGuid = auditLogDTO.PlatformGuid,
                        TenantPlatformMapGuid = auditLogDTO.TenantPlatformMapGuid
                    });
                }
                await auditLogRepository.CommitChanges();
            }

            if (auditActionType != AuditActionType.UserPerformanceActivity && auditActionType != AuditActionType.UserLogin && auditActionType != AuditActionType.UserLogout)
            {
                logger.LogWarning("{@message} {@actionType} {@type} {@typeId} {@userId} {@username}",
                    text,
                    ((int)auditActionType).ToString(),
                    ((int)auditType).ToString(),
                    staffUser.Id.ToString(),
                    userId.ToString(), staffUser.Username);
            }

        }
    }
}
