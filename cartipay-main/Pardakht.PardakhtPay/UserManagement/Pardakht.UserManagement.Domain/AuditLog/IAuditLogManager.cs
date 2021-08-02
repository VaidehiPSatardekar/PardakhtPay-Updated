using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.AuditLog
{
    public interface IAuditLogManager
    {
        Task LogChanges<T>(T updatedRecord, T currentRecord, AuditType auditType, AuditActionType auditActionType, DomainModels.StaffUser staffUser) where T : IEntity;
        Task CreateAuditLogEntry(DomainModels.StaffUser staffUser, AuditType auditType, AuditActionType auditActionType, string text, int userId);
        Task CreateAuditLogEntry(DomainModels.StaffUser staffUser, AuditType auditType, AuditActionType auditActionType, string text, int userId, AuditLogDTO auditLogDTO);
        Task UpdateAuditLogEntry(Shared.Models.Infrastructure.AuditLog auditLog);
        Task<Shared.Models.Infrastructure.AuditLog> GetLastOneByTerm(int typeId, AuditType auditType, AuditActionType auditActionType, string term);
        Task<Shared.Models.Infrastructure.AuditLog> GetLastOneByTerm(int typeId, AuditType auditType, AuditActionType auditActionType, string platformGuid, string tenantPlatformMapGuid,bool isActive);
        Task<IEnumerable<Shared.Models.Infrastructure.AuditLog>> GetStatusLastLoginByTermAndStaffUserIds(AuditType auditType, AuditActionType auditActionType, string term, IEnumerable<int> staffUserIds);
        Task<IEnumerable<Shared.Models.Infrastructure.AuditLog>> GetStatusLastLoginByPlatformTenantGuidAndStaffUserIds(AuditType auditType, AuditActionType auditActionType, IEnumerable<int> staffUserIds, string platformGuid, string tenantPlatformMapGuid);
        Task<IEnumerable<Shared.Models.Infrastructure.AuditLog>> GetByTermAndDateRange(AuditType auditType, AuditActionType auditActionType, string term, DateTime dateFrom, DateTime dateTo);
        Task<IEnumerable<Shared.Models.Infrastructure.AuditLog>> GetByTermAndDateRange(AuditType auditType, AuditActionType auditActionType, DateTime dateFrom, DateTime dateTo, string platformGuid, string tenantPlatformMapGuid, bool isActive);
    }
}
