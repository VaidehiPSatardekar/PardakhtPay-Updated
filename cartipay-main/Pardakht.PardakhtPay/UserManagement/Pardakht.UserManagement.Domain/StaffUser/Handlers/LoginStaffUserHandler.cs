using System.Threading.Tasks;
using Newtonsoft.Json;
using Pardakht.UserManagement.Domain.AuditLog;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.StaffUser.Handlers
{
    public class LoginStaffUserHandler
    {
        private readonly IStaffUserRepository staffUserRepository;
        private readonly IAuditLogManager auditLogManager;

        public LoginStaffUserHandler(IStaffUserRepository staffUserRepository, IAuditLogManager auditLogManager)
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
            // Log in
            await auditLogManager.CreateAuditLogEntry(staffUser, AuditType.User, AuditActionType.UserLogin, "User login",actionedByUser.Id, _newPerformance);

            // Desactivate old time performance if it exists
            //var term = $"%\"{nameof(AuditLogStaffUserTimePerformance.IsActive)}\":true%\"{nameof(AuditLogStaffUserTimePerformance.TenantPlatformMapGuid)}\":\"{tenantPlatformMapGuid}\"" +
            //        $"%\"{nameof(AuditLogStaffUserTimePerformance.PlatformGuid)}\":\"{platformGuid}\"%";
            //var auditRecord = await auditLogManager.GetLastOneByTerm(staffUser.Id, AuditType.User, AuditActionType.UserPerformanceActivity, term);
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
            if (auditRecord != null && !string.IsNullOrEmpty(auditRecord.Message))
            {
                if (auditRecord.IsActive)
                {
                    auditRecord.IsActive = false;
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
                }
                //AuditLogStaffUserTimePerformance oldPerformance = JsonConvert.DeserializeObject<AuditLogStaffUserTimePerformance>(auditRecord.Message);
                //if (oldPerformance != null && oldPerformance.IsActive)
                //{
                //    oldPerformance.IsActive = false;
                //    auditRecord.Message = JsonConvert.SerializeObject(oldPerformance);
                //    await auditLogManager.UpdateAuditLogEntry(auditRecord);
                //}
            }

            // Initialise time performance
            var newPerformance = JsonConvert.SerializeObject(new DomainModels.AuditLogStaffUserTimePerformance() { IsActive = true, TenantPlatformMapGuid = tenantPlatformMapGuid, PlatformGuid = platformGuid });
            _newPerformance.IsActive = true;
            await auditLogManager.CreateAuditLogEntry(staffUser, AuditType.User, AuditActionType.UserPerformanceActivity, newPerformance, actionedByUser.Id, _newPerformance);
        }
    }
}
