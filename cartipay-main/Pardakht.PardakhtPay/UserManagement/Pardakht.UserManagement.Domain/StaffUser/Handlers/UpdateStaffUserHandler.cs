using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pardakht.UserManagement.Domain.AuditLog;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.StaffUser.Handlers
{
    public class UpdateStaffUserHandler : StaffUserEditBase
    {
        private readonly IStaffUserRepository staffUserRepository;
        private readonly IAuditLogManager auditLogManager;
        private ILogger logger;
        public UpdateStaffUserHandler(  IStaffUserRepository staffUserRepository, 
                                        IRoleRepository roleRepository,
                                        IAuditLogManager auditLogManager,
                                        ILogger logger) : base (staffUserRepository, roleRepository)
        {
            this.staffUserRepository = staffUserRepository;
            this.auditLogManager = auditLogManager;
            this.logger = logger;
        }

        public async Task<DomainModels.StaffUser> Handle(DomainModels.StaffUser request, DomainModels.StaffUser staffUser, string platformGuid)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var editRecord = await staffUserRepository.GetByIdWithMappings(request.Id);
            if (editRecord == null)
            {
                throw new Exception($"Staff user {request.Id} not found");
            }

            var currentRecord = await staffUserRepository.GetByIdWithMappingsAsNoTracking(request.Id);

            editRecord.Email = request.Email;
            editRecord.FirstName = request.FirstName;
            editRecord.LastName = request.LastName;
            editRecord.Username = request.Username;
            editRecord.ParentAccountId = request.ParentAccountId;
            editRecord.UserType = request.UserType;
            // clear any existing mappings
            editRecord.UserPlatforms.Clear();

            await AddPlatformAndRoleMappings(request.PlatformRoleMappings, editRecord);

            await staffUserRepository.Update(editRecord);
            await staffUserRepository.CommitChanges();


            editRecord = await staffUserRepository.GetByAccountWithMappingsAsNoTracking(editRecord.AccountId);


            var newValue = JsonConvert.SerializeObject(editRecord, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            logger.LogWarning("{@message} {@actionType} {@type} {@typeId} {@userId} {@username} {@createdbyaccountId}",
                                $"Update staff user {newValue}",
                            ((int)AuditActionType.StaffUserUpdated).ToString(),
                            ((int)AuditType.User).ToString(),
                            editRecord.Id.ToString(),
                            staffUser.Id.ToString(), staffUser.Username, staffUser.AccountId);

            await auditLogManager.LogChanges(editRecord, currentRecord, AuditType.User, AuditActionType.StaffUserUpdated, staffUser);

            return   UserMapper.Map(editRecord, request.TenantGuid, platformGuid);
        }
    }
}
