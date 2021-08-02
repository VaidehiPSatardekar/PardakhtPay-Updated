using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Domain.Role;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;
using DomainModels = Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public class ConfigureTenantUsersHandler : StaffUserHandlerBase
    {
        private readonly IRoleManager roleManager;
        public ConfigureTenantUsersHandler(IRoleManager roleManager, StaffUserHandlerArgs handlerArgs) : base(handlerArgs)
        {
            this.roleManager = roleManager;
        }

        public async Task<DomainModels.WebResponse<DomainModels.ConfigureTenantUsersResponse>> Handle(DomainModels.ConfigureTenantUsersRequest request)
        {
            try
            {
                // TODO: implement when api-key permissions work done
                if (!userContext.HasRole(PermissionConstants.TenantCreate))
                {
                    return new DomainModels.WebResponse<DomainModels.ConfigureTenantUsersResponse> { Success = false, Message = "User not authorised to create tenant admin user" };
                }

                var actionedByUser = await staffUserManager.GetByAccountId(userContext.AccountId, string.Empty, userContext.PlatformGuid);
                if (actionedByUser == null)
                {
                    throw new Exception("Unable to get logged on user details");
                }

                if (actionedByUser.IsBlocked)
                {
                    throw new Exception("User is blocked");
                }

                if (actionedByUser.IsDeleted)
                {
                    throw new Exception("User is deleted");
                }

                var createAdminUserResponse = await CreateAdminStaffUser(request, actionedByUser);

                if (createAdminUserResponse != null)
                {
                    var apiKey = await CreateApiKey(request, actionedByUser);

                    if (!string.IsNullOrEmpty(apiKey))
                    {
                        createAdminUserResponse.ApiKey = apiKey;
                        InvalidateTenantCache();
                        return new DomainModels.WebResponse<DomainModels.ConfigureTenantUsersResponse> { Success = true, Payload = createAdminUserResponse };
                    }
                }

                return new DomainModels.WebResponse<DomainModels.ConfigureTenantUsersResponse> { Success = false, Message = "Unable to create admin user and/or api key" };
            }
            catch (Exception ex)
            {
                logger.LogError($"ConfigureTenantUsersHandler.Handle: an error occurred configuring tenant admin user and api key - {ex}");
                return new DomainModels.WebResponse<DomainModels.ConfigureTenantUsersResponse> { Success = false, Message = ex.Message };
            }
        }

        private async Task<DomainModels.ConfigureTenantUsersResponse> CreateAdminStaffUser(DomainModels.ConfigureTenantUsersRequest request, DomainModels.StaffUser actionedByUser)
        {
            try
            {
                // TODO: configure
                var adminUsername = $"admin_{request.TenantName}_{request.PlatformName}";
                var adminPassword = $"P@ssword123";

                var username = await GetUniqueUsername(adminUsername);
                var identity = new ApplicationUser { UserName = adminUsername, Email = request.AdminUserEmail, EmailConfirmed = true };
                var identityResult = await identityUserManager.CreateAsync(identity, adminPassword);

                if (!identityResult.Succeeded)
                {
                    var message = string.Join(" | ", identityResult.Errors.Select(i => i.Code + ' ' + i.Description));

                    // just log a warning as we can manually set up a user if required
                    logger.LogWarning($"TenantService.CreateAdminStaffUser: unable to create identity for tenant admin staff user {identity.UserName} " +
                        $"for tenant mapping {request.TenantPlatformMapGuid} - {message}");
                    return null;
                }

                var staffUser = new Shared.Models.WebService.StaffUser
                {
                    TenantGuid = request.TenantPlatformMapGuid,
                    AccountId = identity.Id,
                    Username = username,
                    Email = request.AdminUserEmail,
                    UserType = UserType.StaffUser
                };

                staffUser.PlatformRoleMappings = await CreatePlatformRoleMappings(request);

                var createdUser = await staffUserManager.CreateStaffUser(staffUser, actionedByUser, request.TenantId, userContext.PlatformGuid);

                return new DomainModels.ConfigureTenantUsersResponse { AdminPassword = adminPassword, AdminUsername = username };
            }
            catch (Exception ex)
            {
                // don't re-throw error as we want the overall operation to pass even if there is a problem creating the admin user
                logger.LogError($"TenantService.CreateAdminStaffUser: an error occurred creating tenant admin staff user - {ex}");
                return null;
            }
        }
        private async Task<string> CreateApiKey(DomainModels.ConfigureTenantUsersRequest request, DomainModels.StaffUser actionedByUser)
        {
            try
            {
                var apiKeyUsername = $"api_key_{request.TenantName}_{request.PlatformName}";

                var username = await GetUniqueUsername(apiKeyUsername);
                var identity = new ApplicationUser { UserName = apiKeyUsername };
                var identityResult = await identityUserManager.CreateAsync(identity);

                if (!identityResult.Succeeded)
                {
                    var message = string.Join(" | ", identityResult.Errors.Select(i => i.Code + ' ' + i.Description));

                    // just log a warning as we can manually set up a user if required
                    logger.LogWarning($"TenantService.CreateApiKey: unable to create identity for tenant api key user {identity.UserName} " +
                        $"for tenant mapping {request.TenantPlatformMapGuid} - {message}");
                    return string.Empty;
                }

                var staffUser = new Shared.Models.WebService.StaffUser
                {
                    TenantGuid = request.TenantPlatformMapGuid,
                    AccountId = identity.Id,
                    Username = username,
                    UserType = UserType.ApiUser
                };

                // for now, don't map to any roles
                //staffUser.PlatformRoleMappings = await CreatePlatformRoleMappings(request);
                staffUser.PlatformRoleMappings = new List<DomainModels.StaffUserEditPlatformRoleContainer>
                {
                    new DomainModels.StaffUserEditPlatformRoleContainer
                    {
                        PlatformGuid = request.PlatformGuid
                    }
                };

                var createdUser = await staffUserManager.CreateStaffUser(staffUser, actionedByUser, request.TenantId, userContext.PlatformGuid);

                return staffUser.Username;
            }
            catch (Exception ex)
            {
                // don't re-throw error as we want the overall operation to pass even if there is a problem creating the api key user
                logger.LogError($"TenantService.CreateApiKey: an error occurred creating tenant api key user - {ex}");
                return string.Empty;
            }
        }

        private async Task<string> GetUniqueUsername(string requestedUsername)
        {
            // check if the requested username is unique - if not then try to find one
            var result = string.Empty;
            var username = requestedUsername;
            var counter = 1;
            do
            {
                var existing = await identityUserManager.FindByNameAsync(username);
                if (existing == null)
                {
                    result = username;
                }
                else
                {
                    username = $"{requestedUsername}_{counter}";
                }
                counter++;
                if (counter == 100)
                {
                    // prevent endless loop
                    throw new Exception("Unable to generate a unique username");
                }
            } while (result == string.Empty);

            return result;
        }

        private async Task<List<DomainModels.StaffUserEditPlatformRoleContainer>> CreatePlatformRoleMappings(DomainModels.ConfigureTenantUsersRequest model)
        {
            var userRoles = new List<int>();
            var platformRoles = await roleManager.GetRoles(model.PlatformGuid);
            var adminRole = platformRoles.Where(r => r.RoleHolderTypeId == "T" && r.Name.ToLower() == roleSettings.TenantAdminRoleName.ToLower()).FirstOrDefault();
            if (adminRole == null)
            {
                logger.LogWarning($"TenantService.CreateAdminStaffUser: unable to find admin role to add to tenant admin staff user " +
                    $"for tenant mapping {model.TenantPlatformMapGuid}");
            }
            else
            {
                userRoles.Add(adminRole.Id);
            }

            var umbracoAdminRole = platformRoles.Where(r => r.RoleHolderTypeId == "T" && r.Name.ToLower() == "umbraco-admin").FirstOrDefault();
            if (umbracoAdminRole == null)
            {
                logger.LogWarning($"TenantService.CreateAdminStaffUser: unable to find umbraco admin role to add to tenant admin staff user " +
                    $"for tenant mapping {model.TenantPlatformMapGuid}");
            }
            else
            {
                userRoles.Add(umbracoAdminRole.Id);
            }

            return new List<StaffUserEditPlatformRoleContainer>
            {
                new StaffUserEditPlatformRoleContainer
                {
                    PlatformGuid = model.PlatformGuid,
                    Roles = userRoles
                }
            };
        }
    }
}
