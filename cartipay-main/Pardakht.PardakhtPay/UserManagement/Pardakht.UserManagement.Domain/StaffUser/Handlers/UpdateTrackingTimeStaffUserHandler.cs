using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pardakht.UserManagement.Domain.AuditLog;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.StaffUser.Handlers
{
    public class UpdateTrackingTimeStaffUserHandler
    {
        private readonly IStaffUserRepository staffUserRepository;
        private readonly IAuditLogManager auditLogManager;

        public UpdateTrackingTimeStaffUserHandler(IStaffUserRepository staffUserRepository, IAuditLogManager auditLogManager)
        {
            this.staffUserRepository = staffUserRepository;
            this.auditLogManager = auditLogManager;
        }

        public async Task<StaffUserPerformanceTime> Handle(int userId, string tenantPlatformMapGuid, string platformGuid)
        {
            //var term = $"%\"{nameof(AuditLogStaffUserTimePerformance.IsActive)}\":true%\"{nameof(AuditLogStaffUserTimePerformance.TenantPlatformMapGuid)}\":\"{tenantPlatformMapGuid}\"" +
            //        $"%\"{nameof(AuditLogStaffUserTimePerformance.PlatformGuid)}\":\"{platformGuid}\"%";
            //var auditRecord = await auditLogManager.GetLastOneByTerm(userId, InfrastructureModels.AuditType.User, InfrastructureModels.AuditActionType.UserPerformanceActivity, term);
            var auditRecord = (Shared.Models.Infrastructure.AuditLog)null;
            if (string.IsNullOrWhiteSpace(tenantPlatformMapGuid))
            {
                auditRecord = await auditLogManager.GetLastOneByTerm(userId,
                                                                    AuditType.User,
                                                                    AuditActionType.UserPerformanceActivity,
                                                                    platformGuid,
                                                                    null,
                                                                    true);
            }
            else
            {
                auditRecord = await auditLogManager.GetLastOneByTerm(userId,
                                                                    AuditType.User,
                                                                    AuditActionType.UserPerformanceActivity,
                                                                    platformGuid,
                                                                    tenantPlatformMapGuid,
                                                                    true);
            }
            if (auditRecord != null && !string.IsNullOrEmpty(auditRecord.Message) && auditRecord.IsActive)
            {
                auditRecord.DateTime = DateTime.UtcNow;
                auditRecord.IdleTime = auditRecord.IdleTime;
                auditRecord.ActiveTime = auditRecord.ActiveTime;
                auditRecord.LogonTime = auditRecord.LogonTime;
                auditRecord.IsActive = auditRecord.IsActive;
                auditRecord.Message = JsonConvert.SerializeObject(
                                    new AuditLogStaffUserTimePerformance()
                                    {
                                        ActiveTime = auditRecord.ActiveTime,
                                        IdleTime = auditRecord.IdleTime,
                                        LogonTime = auditRecord.LogonTime,
                                        IsActive = auditRecord.IsActive,
                                        TenantPlatformMapGuid = tenantPlatformMapGuid,
                                        PlatformGuid = platformGuid
                                    });
                await auditLogManager.UpdateAuditLogEntry(auditRecord);
                return new StaffUserPerformanceTime
                {
                    Id = userId,
                    IdleTime = auditRecord.IdleTime,
                    ActiveTime = auditRecord.ActiveTime,
                    LogonTime = auditRecord.LogonTime
                };

                //AuditLogStaffUserTimePerformance performance = JsonConvert.DeserializeObject<AuditLogStaffUserTimePerformance>(auditRecord.Message);

                //if (performance != null && performance.IsActive)
                //{
                //    auditRecord.DateTime = DateTime.UtcNow;
                //    auditRecord.Message = JsonConvert.SerializeObject(performance);
                //    await auditLogManager.UpdateAuditLogEntry(auditRecord);
                //    return new StaffUserPerformanceTime {
                //        Id = userId,
                //        IdleTime = performance.IdleTime,
                //        ActiveTime = performance.ActiveTime,
                //        LogonTime = performance.LogonTime
                //    };
                //}
            }
            return new StaffUserPerformanceTime {
                Id = userId,
                IdleTime = 0,
                ActiveTime = 0,
                LogonTime = 0
            };
        }
    }
}
