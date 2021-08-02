using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Domain.AuditLog.Handlers;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.AuditLog
{
    public class AuditLogManager : IAuditLogManager
    {
        private IAuditLogRepository auditLogRepository;
        private ILogger logger;
        public AuditLogManager(IAuditLogRepository auditLogRepository,ILoggerFactory loggerFactory)
        {
            this.auditLogRepository = auditLogRepository;
            this.logger = loggerFactory.CreateLogger<AuditLogManager>();
        }

        public async Task CreateAuditLogEntry(DomainModels.StaffUser staffUser, AuditType auditType, AuditActionType auditActionType, string text,int userId)
        {
            var handler = new CreateAuditLogEntryHandler(auditLogRepository,logger);
            await handler.Handle(staffUser, auditType, auditActionType, text, userId,null);
        }

        public async Task CreateAuditLogEntry(DomainModels.StaffUser staffUser, AuditType auditType, AuditActionType auditActionType, string text , int userId, DomainModels.AuditLogDTO auditLogDTO)
        {
            var handler = new CreateAuditLogEntryHandler(auditLogRepository, logger);

            await handler.Handle(staffUser, auditType, auditActionType, text, userId, auditLogDTO);
        }

        public async Task UpdateAuditLogEntry(Shared.Models.Infrastructure.AuditLog auditLog)
        {
            var handler = new UpdateAuditLogEntryHandler(auditLogRepository);

            await handler.Handle(auditLog);
        }

        public async Task LogChanges<T>(T updatedRecord, T currentRecord, AuditType auditType, AuditActionType auditActionType, DomainModels.StaffUser staffUser) where T : IEntity
        {
            var handler = new LogChangesHandler(auditLogRepository, logger);

            await handler.Handle(updatedRecord, currentRecord, auditType, auditActionType, staffUser);
        }

        public async Task<Shared.Models.Infrastructure.AuditLog> GetLastOneByTerm(int typeId, AuditType auditType, AuditActionType auditActionType, string term)
        {
            return await auditLogRepository.GetLastOneByTerm(typeId, auditType, auditActionType, term);
        }

        public async Task<Shared.Models.Infrastructure.AuditLog> GetLastOneByTerm(int typeId, AuditType auditType, AuditActionType auditActionType, string platformGuid,string tenantPlatformMapGuid,bool isActive)
        {
            return await auditLogRepository.GetLastOneByPlatformGuidTenantPlatformMapGuid(typeId, auditType, auditActionType, platformGuid, tenantPlatformMapGuid, isActive);
        }

        public async Task<IEnumerable<Shared.Models.Infrastructure.AuditLog>> GetStatusLastLoginByTermAndStaffUserIds(AuditType auditType, AuditActionType auditActionType, string term, IEnumerable<int> staffUserIds)
        {
            return await auditLogRepository.GetStatusLastLoginByTermAndStaffUserIds(auditType, auditActionType, term, staffUserIds);
        }

        public async Task<IEnumerable<Shared.Models.Infrastructure.AuditLog>> GetStatusLastLoginByPlatformTenantGuidAndStaffUserIds(AuditType auditType,AuditActionType auditActionType, IEnumerable<int> staffUserIds,string platformGuid,string tenantPlatformMapGuid)
        {
            return await auditLogRepository.GetStatusLastLoginByPlatformTenantGuidAndStaffUserIds(auditType, auditActionType,  staffUserIds, platformGuid, tenantPlatformMapGuid);
        }


        public async Task<IEnumerable<Shared.Models.Infrastructure.AuditLog>> GetByTermAndDateRange(AuditType auditType, AuditActionType auditActionType, string term, DateTime dateFrom, DateTime dateTo)
        {
            return await auditLogRepository.GetByTermAndDateRange(auditType, auditActionType, term, dateFrom, dateTo);
        }
        public async Task<IEnumerable<Shared.Models.Infrastructure.AuditLog>> GetByTermAndDateRange(AuditType auditType, AuditActionType auditActionType, DateTime dateFrom, DateTime dateTo,string platformGuid,string tenantPlatformMapGuid,bool isActive)
        {
            return await auditLogRepository.GetByPlatformTenantPlatformMapGuidAndDateRange(auditType, auditActionType, dateFrom, dateTo,platformGuid,tenantPlatformMapGuid, isActive);
        }
    }
}
