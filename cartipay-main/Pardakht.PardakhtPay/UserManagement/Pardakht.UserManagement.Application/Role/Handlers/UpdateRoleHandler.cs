using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Domain.Role;
using Pardakht.UserManagement.Domain.StaffUser;
using Pardakht.UserManagement.Shared.Models.Configuration;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.Role.Handlers
{
    public class UpdateRoleHandler
    {
        private readonly IRoleManager roleManager;
        private readonly UserContext userContext;
        private readonly IStaffUserManager staffUserManager;
        private readonly ILogger<RoleService> logger;

        public UpdateRoleHandler(   IRoleManager roleManager, 
                                    UserContext userContext,   
                                    IStaffUserManager staffUserManager, 
                                    ILogger<RoleService> logger)
        {
            this.roleManager = roleManager;
            this.userContext = userContext;
            this.staffUserManager = staffUserManager;
            this.logger = logger;
        }

        public async Task<WebResponse<RoleDto>> Handle(RoleDto request)
        {
            try
            {
                var validateUserMessage = ValidateUserPermissions(request);
                if (!string.IsNullOrWhiteSpace(validateUserMessage))
                {
                    return new WebResponse<RoleDto> { Message = validateUserMessage };
                }

                var actionedByUser = await staffUserManager.GetByAccountId(userContext.AccountId, string.Empty, userContext.PlatformGuid);
                if (actionedByUser == null)
                {
                    throw new Exception("Unable to get logged on user details");
                }

                var validateRoleMessage = await ValidateRole(request);
                if (!string.IsNullOrWhiteSpace(validateRoleMessage))
                {
                    return new WebResponse<RoleDto> { Message = validateRoleMessage };
                }

                var result = await roleManager.EditRole(request, actionedByUser);
                if (result == null)
                {
                    return new WebResponse<RoleDto> { Message = "Unable to edit role" };
                }

                return new WebResponse<RoleDto> { Success = true, Payload = result };
            }
            catch (Exception ex)
            {
                logger.LogError($"UpdateRoleHandler.Handle: an error occurred updating role {request.Name} platform {request.PlatformGuid} - {ex}");
                throw;
            }
        }

        private string ValidateUserPermissions(RoleDto request)
        {
            // permission reqd for 'fixed' roles
            if (request.IsFixed)
            {
                if (!userContext.HasRole(PermissionConstants.RoleEditFixedRoles))
                {
                    return "User not authorised to edit fixed roles";
                }
            }

            // permissions for role types
            if (request.RoleHolderTypeId == RoleConstants.RoleHolderTypeProvider)
            {
                if (!userContext.HasRole(PermissionConstants.RoleEditProviderRoles))
                {
                    return "User not authorised to edit provider roles";
                }
            }

            if (request.RoleHolderTypeId == RoleConstants.RoleHolderTypeTenant)
            {
                if (!userContext.HasRole(PermissionConstants.RoleEditTenantRoles))
                {
                    return "User not authorised to edit tenant roles";
                }
                if (string.IsNullOrEmpty(request.TenantGuid) && !request.IsFixed)
                {
                    return "Tenant role requires a valid tenantId";
                }
                
                if (!userContext.IsProviderUser())
                {
                    if (userContext.TenantGuid != request.TenantGuid)
                    {
                        return "A tenant role can only be edited by a user from the same tenancy, or by a provider user";
                    }
                }
            }

            return string.Empty;
        }

        private async Task<string> ValidateRole(RoleDto role)
        {
            if (role.IsFixed && !string.IsNullOrEmpty(role.TenantGuid))
            {
                return "Unable to edit role - a global role cannot be assigned to a specific tenant";
            }

            var duplicateRole = await roleManager.GetByName(role.Name, role.RoleHolderTypeId, role.PlatformGuid);

            // make sure there is not already one with the same name
            if (duplicateRole != null && duplicateRole.Id != role.Id)
            {
                if (!string.IsNullOrEmpty(role.TenantGuid) && !string.IsNullOrEmpty(duplicateRole.TenantGuid) && role.TenantGuid == duplicateRole.TenantGuid)
                {
                    return $"Unable to edit role - another role already exists with the name {role.Name}";
                }
                if (string.IsNullOrEmpty(role.TenantGuid) && string.IsNullOrEmpty(duplicateRole.TenantGuid))
                {
                    return $"Unable to edit role - another role already exists with the name {role.Name}";
                }
            }

            var existingRole = await roleManager.GetById(role.Id);
            if (existingRole != null)
            {
                if (existingRole.Name.ToLower() == "admin" && role.Name.ToLower() != "admin")
                {
                    return "Unable to edit role - the admin role cannot have its name changed";
                }

                if (existingRole.Name.ToLower() == "umbraco-admin" && role.Name.ToLower() != "umbraco-admin")
                {
                    return "Unable to edit role - the umbraco admin role cannot have its name changed";
                }
            }

            return string.Empty;
        }
    }
}
