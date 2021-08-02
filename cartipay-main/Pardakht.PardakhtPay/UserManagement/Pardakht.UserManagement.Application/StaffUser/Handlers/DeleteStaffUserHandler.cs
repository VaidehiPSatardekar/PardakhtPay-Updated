using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Domain.StaffUser;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public class DeleteStaffUserHandler : StaffUserHandlerBase
    {
        public DeleteStaffUserHandler( StaffUserHandlerArgs handlerArgs) : base( handlerArgs) {}

        public async Task<WebResponse<Shared.Models.WebService.StaffUser>> Handle(DeleteStaffUserRequest request)
        {
            try
            {
                if (!userContext.HasRole(PermissionConstants.StaffUserDelete))
                {
                    return new WebResponse<Shared.Models.WebService.StaffUser> { Success = false, Message = "User not authorised to delete users" };
                }

                var loggedOnUser = await staffUserManager.GetByAccountId(userContext.AccountId, string.Empty, userContext.PlatformGuid);

                if (loggedOnUser == null)
                {
                    throw new StaffUserValidationException($"Unable to find user for account {userContext.AccountId}");
                }

                var userToDelete = await staffUserManager.GetById(request.UserId, userContext.PlatformGuid);
                if (userToDelete == null)
                {
                    throw new StaffUserValidationException($"UserId {request.UserId} does not exist");
                }

                // if the user is a tenant user, we need to get the tenantPlatformMapGuid for mapping
                var tenantPlatformMapGuid = string.Empty;
                if (userToDelete.TenantId.HasValue)
                {
                    tenantPlatformMapGuid = (await GetTenantPlatformMapping(userToDelete.TenantId.Value, userContext.PlatformGuid))?.TenantPlatformMapGuid;
                }

                if (!userContext.IsProviderUser())
                {
                    if (!userToDelete.TenantId.HasValue)
                    {
                        throw new StaffUserValidationException($"Tenant users cannot update provider users");
                    }
                    // if the user making the change is a tenant user, make sure they are not updating a user in another tenancy
                    if (userToDelete.TenantId.HasValue && userToDelete.TenantId != loggedOnUser.TenantId)
                    {
                        throw new StaffUserValidationException("Tenant users cannot update users that belong to another tenancy");
                    }
                }

                var result = await staffUserManager.DeleteStaffUser(request.UserId, loggedOnUser, tenantPlatformMapGuid, userContext.PlatformGuid);

                await SendEmailTenantBlockedStaffUserorDeletedorPasswordChange(result, Shared.Models.WebService.Enum.UserAction.DeleteUser);
               


                return new WebResponse<Shared.Models.WebService.StaffUser>
                {
                    Success = true,
                    Payload = result,
                    Message = string.Empty
                };
            }
            catch(StaffUserValidationException validationEx)
            {
                return new WebResponse<Shared.Models.WebService.StaffUser>
                {
                    Success = false,
                    Message = validationEx.Message
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"DeleteStaffUserHandler.Handle: an error occurred deleting user {request.UserId} - {ex}");
                throw;
            }
        }
    }
}
