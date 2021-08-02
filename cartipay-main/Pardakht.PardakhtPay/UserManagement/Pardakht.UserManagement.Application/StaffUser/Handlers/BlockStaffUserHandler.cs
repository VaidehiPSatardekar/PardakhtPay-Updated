using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Domain.StaffUser;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public class BlockStaffUserHandler : StaffUserHandlerBase
    {
        public BlockStaffUserHandler( StaffUserHandlerArgs handlerArgs) : base( handlerArgs) {}

        public async Task<WebResponse<Shared.Models.WebService.StaffUser>> Handle(BlockStaffUserRequest request)
        {
            try
            {
                if (!userContext.HasRole(PermissionConstants.StaffUserBlock))
                {
                    return new WebResponse<Shared.Models.WebService.StaffUser> { Success = false, Message = "User not authorised to block users" };
                }

                var loggedOnUser = await staffUserManager.GetByAccountId(userContext.AccountId, string.Empty, userContext.PlatformGuid);

                if (loggedOnUser == null)
                {
                    throw new StaffUserValidationException($"Unable to find user for account {userContext.AccountId}");
                }

                var userToBlock = await staffUserManager.GetById(request.UserId, userContext.PlatformGuid);
                if (userToBlock == null)
                {
                    throw new StaffUserValidationException($"UserId {request.UserId} does not exist");
                }

                // if the user is a tenant user, we need to get the tenantPlatformMapGuid for mapping
                var tenantPlatformMapGuid = string.Empty;
                if (userToBlock.TenantId.HasValue)
                {
                    tenantPlatformMapGuid = (await GetTenantPlatformMapping(userToBlock.TenantId.Value, userContext.PlatformGuid))?.TenantPlatformMapGuid;
                }

                if (!userContext.IsProviderUser())
                {
                    if (!userToBlock.TenantId.HasValue)
                    {
                        throw new StaffUserValidationException($"Tenant users cannot update provider users");
                    }
                    // if the user making the change is a tenant user, make sure they are not updating a user in another tenancy
                    if (userToBlock.TenantId.HasValue && userToBlock.TenantId != loggedOnUser.TenantId)
                    {
                        throw new StaffUserValidationException("Tenant users cannot update users that belong to another tenancy");
                    }
                }

                var result = await staffUserManager.BlockStaffUser(request.UserId, loggedOnUser, request.Block, tenantPlatformMapGuid, userContext.PlatformGuid);
                
                await SendEmailTenantBlockedStaffUserorDeletedorPasswordChange(result,Shared.Models.WebService.Enum.UserAction.BlockingUnBlockinUser);

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
                logger.LogError($"BlockStaffUserHandler.Handle: an error occurred blocking user {request.UserId} - {ex}");
                throw;
            }
        }
    }
}
