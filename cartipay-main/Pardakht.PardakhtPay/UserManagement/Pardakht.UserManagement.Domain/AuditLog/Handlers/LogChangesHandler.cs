using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.AuditLog.Handlers
{
    public class LogChangesHandler
    {
        private IAuditLogRepository auditLogRepository;
        private ILogger logger;
        public LogChangesHandler(IAuditLogRepository auditLogRepository, ILogger logger)
        {
            this.auditLogRepository = auditLogRepository;
            this.logger = logger;
        }

        public async Task Handle<T>(T updatedRecord, T currentRecord, AuditType auditType, AuditActionType auditActionType, DomainModels.StaffUser staffUser) where T : IEntity
        {
            var auditLogs = new List<Shared.Models.Infrastructure.AuditLog>();
            foreach (var prop in updatedRecord.GetType().GetProperties())
            {
                var currentValue = string.Empty;
                var currentValueObject = currentRecord.GetType().GetProperty(prop.Name).GetValue(currentRecord, null);
                if (currentValueObject != null)
                    currentValue = currentValueObject.ToString();

                 var newValue = prop.GetValue(updatedRecord, null)?.ToString();
                if (newValue != null && currentValue != newValue)
                {
                    if (!currentValue.ToLower().Contains("system.collections")) // hack to prevent different list types
                    {
                        //auditLogs.Add(new Shared.Models.Infrastructure.AuditLog
                        //{
                        //    Message = DisplayCamelCaseString(prop.Name) + " updated from '" + currentValue + "' to '" + newValue + "'",
                        //    Type = auditType,
                        //    ActionType = auditActionType,
                        //    TypeId = updatedRecord.Id,
                        //    DateTime = DateTime.UtcNow,
                        //    UserId = userId
                        //});

                        logger.LogWarning("{@message} {@actionType} {@type} {@typeId} {@userId} {@username} {@createdbyaccountId}",
                                            DisplayCamelCaseString(prop.Name) + " updated from '" + currentValue + "' to '" + newValue + "'",
                                            ((int)auditActionType).ToString(),
                                            ((int)auditType).ToString(),
                                            updatedRecord.Id.ToString(),
                                            staffUser.Id.ToString(),staffUser.Username, staffUser.AccountId);

                    }
                }
            }

            if (auditLogs.Count > 0)
            {
                await auditLogRepository.CreateRange(auditLogs);
                await auditLogRepository.CommitChanges();
            }
        }

        private string DisplayCamelCaseString(string camelCase)
        {
            if (camelCase is null)
                return "";

            var chars = new List<char>
            {
                camelCase[0]
            };
            foreach (char c in camelCase.Skip(1))
            {
                if (char.IsUpper(c))
                {
                    chars.Add(' ');
                    chars.Add(char.ToLower(c));
                }
                else
                    chars.Add(c);
            }

            return new string(chars.ToArray());
        }
    }
}
