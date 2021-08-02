using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pardakht.UserManagement.Domain.AuditLog;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Domain.StaffUser.Handlers
{
    public class BlockAllTenantUsersHandler : StaffUserEditBase
    {
        private readonly IStaffUserRepository _staffUserRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IAuditLogManager _auditLogManager;
        private readonly ILogger _logger;

        public BlockAllTenantUsersHandler(IStaffUserRepository staffUserRepository, IRoleRepository roleRepository, IAuditLogManager auditLogManager, ILogger logger) : base(staffUserRepository, roleRepository)
        {
            _staffUserRepository = staffUserRepository;
            _auditLogManager = auditLogManager;
            _logger = logger;
            _roleRepository = roleRepository;
        }


        public async Task<IEnumerable<DomainModels.StaffUser>> Handle(DomainModels.StaffUser actionedByUser, IEnumerable<DomainModels.StaffUser> staffUsers, DomainModels.BlockAllUsersRequest request)
        {
            foreach (var user in staffUsers)
            {
                var editRecord = await _staffUserRepository.GetByIdWithMappings(user.Id);
                if (editRecord == null)
                {
                    throw new Exception($"Staff user {user.Id} not found");
                }

                var currentRecord = await _staffUserRepository.GetByIdWithMappingsAsNoTracking(user.Id);

                // clear any existing mappings
                editRecord.UserPlatforms.Clear();

                if (request.StaffUsers == null)
                {
                    var roles = await _roleRepository.GetAll();
                    var platformRoleMappings = new List<DomainModels.StaffUserEditPlatformRoleContainer>();
                    var mapping = new DomainModels.StaffUserEditPlatformRoleContainer
                    {
                        PlatformGuid = request.PlatformGuid,
                        Roles = new List<int>()
                    };

                    if (IsValidRoleForBillingOnly(user.Permissions))
                    {
                        mapping.Roles.Add(roles.FirstOrDefault(x => x.Name.ToLower() == "deposit only").Id);
                        platformRoleMappings.Add(mapping);
                    }

                    await AddPlatformAndRoleMappings(platformRoleMappings, editRecord);
                }
                else
                {
                    var platformRoleMappings = request.StaffUsers.FirstOrDefault(x => x.Id == user.Id)?.PlatformRoleMappings;
                    if (platformRoleMappings.Any())
                    {
                        await AddPlatformAndRoleMappings(platformRoleMappings, editRecord);
                    }
                }

                await _staffUserRepository.Update(editRecord);
                await _staffUserRepository.CommitChanges();


                editRecord = await _staffUserRepository.GetByAccountWithMappingsAsNoTracking(editRecord.AccountId);


                var newValue = JsonConvert.SerializeObject(editRecord, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

                _logger.LogWarning("{@message} {@actionType} {@type} {@typeId} {@userId} {@username} {@createdbyaccountId}",
                                    $"Update staff user {newValue}",
                                ((int)AuditActionType.StaffUserUpdated).ToString(),
                                ((int)AuditType.User).ToString(),
                                editRecord.Id.ToString(),
                                actionedByUser.Id.ToString(), actionedByUser.Username, actionedByUser.AccountId);

                await _auditLogManager.LogChanges(editRecord, currentRecord, AuditType.User, AuditActionType.StaffUserUpdated, actionedByUser);

            }
            return staffUsers;
        }

        private bool IsValidRoleForBillingOnly(ICollection<string> permissions)
        {
            return permissions.Any(x => x == PermissionConstants.InvoicePayments || x == PermissionConstants.AddInvoicePayment || x == PermissionConstants.Invoices);
        }
    }
}