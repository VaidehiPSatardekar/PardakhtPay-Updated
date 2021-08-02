using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.SqlRepository.Repositories
{
    public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(ParadakhtUserManagementDbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<AuditLog>> CreateRange(IEnumerable<AuditLog> entities)
        {
            var result = new List<AuditLog>();
            foreach (var entity in entities)
            {
                result.Add(await Create(entity));
            }
            return result;
        }

        public async Task<IEnumerable<AuditLog>> GetByTypeId(AuditType type, int typeId)
        {
            return await Task.Run(() => DbContext.AuditLogs.Where(q => q.Type == type && q.TypeId == typeId).AsEnumerable());
        }

        public async Task<AuditLog> GetLastOneByTerm(int typeId, AuditType auditType, AuditActionType auditActionType, string term)
        {
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            return await DbContext.AuditLogs.Where(m => m.Type == auditType && m.ActionType == auditActionType && m.TypeId == typeId && DbF.Like(m.Message, term))
                .OrderByDescending(m => m.Id).FirstOrDefaultAsync();
        }

        public async Task<AuditLog> GetLastOneByPlatformGuidTenantPlatformMapGuid(int typeId, AuditType auditType, AuditActionType auditActionType, string platformGuid,string tenantPlatformMapGuid,bool isActive)
        {
            if (string.IsNullOrWhiteSpace(tenantPlatformMapGuid))
            {
                return await DbContext.AuditLogs.Where(m => m.Type == auditType &&
                                                                        m.ActionType == auditActionType &&
                                                                        m.TypeId == typeId &&
                                                                        m.PlatformGuid == platformGuid &&
                                                                        m.IsActive == isActive)
                                                            .OrderByDescending(m => m.Id)
                                                            .FirstOrDefaultAsync();
            }
            else
            {
                return await DbContext.AuditLogs.Where(m => m.Type == auditType &&
                                                            m.ActionType == auditActionType &&
                                                            m.TypeId == typeId &&
                                                            m.PlatformGuid == platformGuid &&
                                                            m.TenantPlatformMapGuid == tenantPlatformMapGuid &&
                                                            m.IsActive == isActive)
                                                .OrderByDescending(m => m.Id)
                                                .FirstOrDefaultAsync();
            }
            


        }

        public async Task<IEnumerable<AuditLog>> GetStatusLastLoginByTermAndStaffUserIds(AuditType auditType, AuditActionType auditActionType, string term, IEnumerable<int> staffUserIds)
        {
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            return await DbContext.AuditLogs
                .Where(m => m.Type == auditType && m.ActionType == auditActionType && staffUserIds.Any(m1 => m1 == m.TypeId) && DbF.Like(m.Message, term))
                .OrderByDescending(m => m.Id)
                .GroupBy(m => m.TypeId)
                .Select(g => new AuditLog
                {
                    Id = g.First().Id,
                    TypeId = g.Key,
                    DateTime = g.First().DateTime,
                    Message = g.First().Message
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetStatusLastLoginByPlatformTenantGuidAndStaffUserIds(AuditType auditType, AuditActionType auditActionType, IEnumerable<int> staffUserIds,string platformGuid,string tenantPlatformGuid)
        {
            if (string.IsNullOrWhiteSpace(tenantPlatformGuid))
            {
                return await DbContext.AuditLogs
                    .Where(m => DbContext.AuditLogs
                   .Where(m => m.Type == auditType &&
                               m.ActionType == auditActionType &&
                               staffUserIds.Any(m1 => m1 == m.TypeId) &&
                               m.PlatformGuid == platformGuid)
                   .GroupBy(m => m.TypeId)
                   .Select(t => t.Max(p => p.Id)).Contains(m.Id)).ToListAsync();
            }
            else
            {
                return await DbContext.AuditLogs
                    .Where(m => DbContext.AuditLogs
                   .Where(m => m.Type == auditType &&
                               m.ActionType == auditActionType &&
                               staffUserIds.Any(m1 => m1 == m.TypeId) &&
                               m.PlatformGuid == platformGuid &&
                                m.TenantPlatformMapGuid == tenantPlatformGuid)
                   .GroupBy(m => m.TypeId)
                   .Select(t => t.Max(p => p.Id)).Contains(m.Id)).ToListAsync();
            }
        }

        public async Task<IEnumerable<AuditLog>> GetByTermAndDateRange(AuditType auditType, AuditActionType auditActionType, string term, DateTime dateFrom, DateTime dateTo)
        {
            if (dateFrom == null || dateTo == null || dateFrom == DateTime.MinValue || dateTo <= dateFrom)
            {
                dateFrom = DateTime.Today;
                dateTo = dateFrom.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            return await DbContext.AuditLogs
                .Where(m => m.DateTime >= dateFrom && m.DateTime <= dateTo && m.Type == auditType && m.ActionType == auditActionType && DbF.Like(m.Message, term))
                .ToListAsync();
        }
        public async Task<IEnumerable<AuditLog>> GetByPlatformTenantPlatformMapGuidAndDateRange(AuditType auditType, AuditActionType auditActionType, DateTime dateFrom, DateTime dateTo,string platformGuid, string tenantPlatformMapGuid,bool isActive)
        {
            if (dateFrom == null || dateTo == null || dateFrom == DateTime.MinValue || dateTo <= dateFrom)
            {
                dateFrom = DateTime.Today;
                dateTo = dateFrom.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            var response = (IEnumerable<AuditLog>)null;
            if (string.IsNullOrWhiteSpace(tenantPlatformMapGuid))
            {
                response = await DbContext.AuditLogs
                    .Where(m => m.DateTime >= dateFrom &&
                                              m.DateTime <= dateTo &&
                                              m.Type == auditType &&
                                              m.ActionType == auditActionType &&
                                              m.PlatformGuid == platformGuid)
                    .ToListAsync();
            }
            else
            {
                response = await DbContext.AuditLogs
                    .Where(m => m.DateTime >= dateFrom &&
                                              m.DateTime <= dateTo &&
                                              m.Type == auditType &&
                                              m.ActionType == auditActionType &&
                                              m.PlatformGuid == platformGuid &&
                                              m.TenantPlatformMapGuid == tenantPlatformMapGuid)
                    .ToListAsync();
            }
            return response.Where(x => x.IsActive == isActive);
            
        }
    }
}