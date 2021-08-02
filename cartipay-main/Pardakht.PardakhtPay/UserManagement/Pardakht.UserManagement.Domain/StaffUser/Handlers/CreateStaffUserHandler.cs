using System;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.UserManagement.Domain.AuditLog;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;
using InfrastructureModels = Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Domain.StaffUser.Handlers
{
    public class CreateStaffUserHandler : StaffUserEditBase
    {
        private readonly IStaffUserRepository staffUserRepository;
        private readonly IAuditLogManager auditLogManager;

        public CreateStaffUserHandler(  IStaffUserRepository staffUserRepository, 
                                        IRoleRepository roleRepository,     
                                        IAuditLogManager auditLogManager) : base(staffUserRepository, roleRepository)
        {
            this.staffUserRepository = staffUserRepository;
            this.auditLogManager = auditLogManager;
        }

        public async Task<DomainModels.StaffUser> Handle(DomainModels.StaffUser request, DomainModels.StaffUser staffUser, int? tenantId, string platformGuid)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var existing = (await staffUserRepository.Find(u => u.AccountId == request.AccountId || u.Username == request.Username)).FirstOrDefault();
            if (existing != null)
            {
                throw new Exception($"Staff user already exists - account {request.AccountId}, username {request.Username}");
            }

            var newUser = new InfrastructureModels.StaffUser
            {
                AccountId = request.AccountId,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                TenantId = tenantId,
                Username = request.Username,
                UserType = request.UserType,
                ParentAccountId = request.ParentAccountId,
                BrandId = request.BrandId
            };

            await AddPlatformAndRoleMappings(request.PlatformRoleMappings, newUser);

            await staffUserRepository.Create(newUser);
            await staffUserRepository.CommitChanges();


            newUser = await staffUserRepository.GetByAccountWithMappingsAsNoTracking(newUser.AccountId);

            await auditLogManager.CreateAuditLogEntry(new DomainModels.StaffUser { Id=newUser.Id, Username= newUser.Username }, 
                                                    InfrastructureModels.AuditType.User,
                                                    InfrastructureModels.AuditActionType.StaffUserCreated, 
                                                    "StaffUser created",
                                                    staffUser.Id);

            return UserMapper.Map(newUser, request.TenantGuid, platformGuid);
        }
    }
}
