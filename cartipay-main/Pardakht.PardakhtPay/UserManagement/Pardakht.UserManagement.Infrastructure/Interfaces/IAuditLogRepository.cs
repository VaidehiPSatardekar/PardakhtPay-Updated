using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.Interfaces
{
    public interface IAuditLogRepository : IGenericRepository<AuditLog>
    {
        Task<IEnumerable<AuditLog>> CreateRange(IEnumerable<AuditLog> entities);
        Task<IEnumerable<AuditLog>> GetByTypeId(AuditType type, int typeId);
        Task<AuditLog> GetLastOneByTerm(int typeId, AuditType auditType, AuditActionType auditActionType, string term);
        Task<AuditLog> GetLastOneByPlatformGuidTenantPlatformMapGuid(int typeId, AuditType auditType, AuditActionType auditActionType, string platformGuid, string tenantPlatformMapGuid, bool isActive);
        Task<IEnumerable<AuditLog>> GetStatusLastLoginByTermAndStaffUserIds(AuditType auditType, AuditActionType auditActionType, string term, IEnumerable<int> staffUserIds);
        Task<IEnumerable<AuditLog>> GetStatusLastLoginByPlatformTenantGuidAndStaffUserIds(AuditType auditType, AuditActionType auditActionType, IEnumerable<int> staffUserIds, string platformGuid, string tenantPlatformGuid);
        Task<IEnumerable<AuditLog>> GetByTermAndDateRange(AuditType auditType, AuditActionType auditActionType, string term, DateTime dateFrom, DateTime dateTo);
        Task<IEnumerable<AuditLog>> GetByPlatformTenantPlatformMapGuidAndDateRange(AuditType auditType, AuditActionType auditActionType, DateTime dateFrom, DateTime dateTo, string platformGuid, string tenantPlatformMapGuid,bool isActive);
    }
}