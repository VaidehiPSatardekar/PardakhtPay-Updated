using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
   
    public class UpdateStaffUserHandler : StaffUserHandlerBase
    {
        public UpdateStaffUserHandler( StaffUserHandlerArgs handlerArgs) : base( handlerArgs)
        {
        }

        public async Task<WebResponse<Shared.Models.WebService.StaffUser>> Handle(Shared.Models.WebService.StaffUser request)
        {
            try
            {
                if (!userContext.HasRole(PermissionConstants.StaffUserEdit))
                {
                    return new WebResponse<Shared.Models.WebService.StaffUser> { Success = false, Message = "User not authorised to update users" };
                }

                var actionedByUser = await staffUserManager.GetByAccountId(userContext.AccountId, string.Empty, userContext.PlatformGuid);
                if (actionedByUser == null)
                {
                    throw new Exception("Unable to get logged on user details");
                }

                if (request.UserType == UserType.StaffUser || request.UserType == UserType.AffiliateUser)
                {
                    if (string.IsNullOrEmpty(request.Email))
                    {
                        return new WebResponse<Shared.Models.WebService.StaffUser> { Success = false, Message = "Email required for staff users" };
                    }
                }

                if (!userContext.IsProviderUser())
                {
                    // if the user making the change is a tenant user, make sure they are only updating a user in their tenancy
                    if (request.TenantGuid != userContext.TenantGuid)
                    {
                        throw new Exception("Tenant users cannot update users from other tenancies");
                    }
                    // TODO: we should get the existing user record from the db and check the tenant
                    if (string.IsNullOrEmpty(request.TenantGuid))
                    {
                        //staffUserDto.TenantGuid = userContext.TenantGuid;
                        throw new Exception("Tenant users cannot update provider users");
                    }
                }
                var getNewRoles = await GetNewRoleList(request, userContext.PlatformGuid);

                var result = await staffUserManager.UpdateStaffUser(request, actionedByUser, userContext.PlatformGuid);

                if (getNewRoles != null) { 
                    if (getNewRoles.Count > 0)
                        await SendEmailTenantRoleAssignedNotification(request, getNewRoles);
                }

               

                return new WebResponse<Shared.Models.WebService.StaffUser>
                {
                    Success = true,
                    Payload = result
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"UpdateStaffUserHandler.Handle: an error occurred updating staff user {request.Username} - {ex}");
                throw;
            }
        }
        
    }
}
