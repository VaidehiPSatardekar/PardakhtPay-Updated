using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Pardakht.UserManagement.Domain.AuditLog;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.StaffUser.Handlers
{
    public class LogoutStaffUserHandler
    {
        private readonly IStaffUserRepository staffUserRepository;
        private readonly IAuditLogManager auditLogManager;

        public LogoutStaffUserHandler(IStaffUserRepository staffUserRepository, IAuditLogManager auditLogManager)
        {
            this.staffUserRepository = staffUserRepository;
            this.auditLogManager = auditLogManager;
        }

        public async Task Handle(DomainModels.StaffUser staffUser, DomainModels.StaffUser actionedByUser, string tenantPlatformMapGuid, string platformGuid)
        {
            var _newPerformance = new DomainModels.AuditLogDTO
            {
                ActiveTime = 0,
                IdleTime = 0,
                LogonTime = 0,
                IsActive = false,
                PlatformGuid = platformGuid,
                TenantPlatformMapGuid = tenantPlatformMapGuid
            };
            // Log out
            await auditLogManager.CreateAuditLogEntry(staffUser, AuditType.User, AuditActionType.UserLogout, "User logout", actionedByUser.Id, _newPerformance);

            //var term = $"%\"{nameof(AuditLogStaffUserTimePerformance.IsActive)}\":true%\"{nameof(AuditLogStaffUserTimePerformance.TenantPlatformMapGuid)}\":\"{tenantPlatformMapGuid}\"" +
            //        $"%\"{nameof(AuditLogStaffUserTimePerformance.PlatformGuid)}\":\"{platformGuid}\"%";
            var auditRecord = (Shared.Models.Infrastructure.AuditLog)null;
            if (string.IsNullOrWhiteSpace(tenantPlatformMapGuid))
            {
                auditRecord = await auditLogManager.GetLastOneByTerm(staffUser.Id, 
                                                                        AuditType.User,
                                                                        AuditActionType.UserPerformanceActivity,
                                                                        platformGuid,
                                                                        null,
                                                                        true);
            }
            else
            {
                auditRecord = await auditLogManager.GetLastOneByTerm(staffUser.Id,
                                                        AuditType.User,
                                                        AuditActionType.UserPerformanceActivity,
                                                        platformGuid,
                                                        tenantPlatformMapGuid,
                                                        true);

            }
            if (auditRecord != null && !string.IsNullOrEmpty(auditRecord.Message) && auditRecord.IsActive)
            {
                var now = DateTime.UtcNow;
                var activeTime = ((int)(now - auditRecord.DateTime).TotalMinutes);
                auditRecord.ActiveTime += activeTime > 0 ? activeTime : 0;
                auditRecord.LogonTime = auditRecord.ActiveTime + auditRecord.IdleTime;
                auditRecord.IsActive = false;
                auditRecord.DateTime = now;

                auditRecord.Message = JsonConvert.SerializeObject(
                                                    new DomainModels.AuditLogStaffUserTimePerformance()
                                                    {
                                                        ActiveTime = auditRecord.ActiveTime,
                                                        IdleTime = auditRecord.IdleTime,
                                                        LogonTime = auditRecord.LogonTime,
                                                        IsActive = auditRecord.IsActive,
                                                        TenantPlatformMapGuid = tenantPlatformMapGuid,
                                                        PlatformGuid = platformGuid
                                                    });
                await auditLogManager.UpdateAuditLogEntry(auditRecord);

                //AuditLogStaffUserTimePerformance performance = JsonConvert.DeserializeObject<AuditLogStaffUserTimePerformance>(auditRecord.Message);

                //if(performance != null && performance.IsActive)
                //{
                //    var now = DateTime.UtcNow;
                //    var activeTime = ((int)(now - auditRecord.DateTime).TotalMinutes);
                //    performance.ActiveTime += activeTime > 0 ? activeTime : 0;
                //    performance.LogonTime = performance.ActiveTime + performance.IdleTime;
                //    performance.IsActive = false;
                //    auditRecord.DateTime = now;
                //    auditRecord.Message = JsonConvert.SerializeObject(performance);
                //    await auditLogManager.UpdateAuditLogEntry(auditRecord);
                //}
            }
        }
    }
}
