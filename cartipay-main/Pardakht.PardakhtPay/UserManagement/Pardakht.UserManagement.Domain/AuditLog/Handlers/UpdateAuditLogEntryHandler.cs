using System.Threading.Tasks;
using Pardakht.UserManagement.Infrastructure.Interfaces;

namespace Pardakht.UserManagement.Domain.AuditLog.Handlers
{
    public class UpdateAuditLogEntryHandler
    {
        private IAuditLogRepository auditLogRepository;

        public UpdateAuditLogEntryHandler(IAuditLogRepository auditLogRepository)
        {
            this.auditLogRepository = auditLogRepository;
        }

        public async Task Handle(Shared.Models.Infrastructure.AuditLog auditLog)
        {
            await auditLogRepository.Update(auditLog);
            await auditLogRepository.CommitChanges();
        }
    }
}
