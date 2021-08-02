using System.Threading.Tasks;
using Pardakht.UserManagement.Domain.AuditLog;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.StaffUser.Handlers
{
    public class DeleteStaffUserHandler
    {
        private readonly IStaffUserRepository staffUserRepository;
        private readonly IAuditLogManager auditLogManager;

        public DeleteStaffUserHandler(IStaffUserRepository staffUserRepository, IAuditLogManager auditLogManager)
        {
            this.staffUserRepository = staffUserRepository;
            this.auditLogManager = auditLogManager;
        }

        public async Task<DomainModels.StaffUser> Handle(int userId, DomainModels.StaffUser actionedByUser, string tenantPlatformMapGuid, string platformGuid)
        {
            var staffUser = await staffUserRepository.GetById(userId);
               
            if (staffUser.IsDeleted)
            {
                throw new StaffUserValidationException("This user is already deleted");
            }

            staffUser.IsDeleted = true;

            await staffUserRepository.Update(staffUser);
            
            await staffUserRepository.CommitChanges();


            var result = await staffUserRepository.GetByIdWithMappings(userId);
            await auditLogManager.CreateAuditLogEntry(new DomainModels.StaffUser {  Id= userId , Username=""}, AuditType.User, AuditActionType.StaffUserDeleted, "Staff User deleted", actionedByUser.Id);

            return UserMapper.Map(result, tenantPlatformMapGuid, platformGuid);

        }
    }
}
