using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public class CreateStaffUserHandler : StaffUserHandlerBase
    {
        public CreateStaffUserHandler( StaffUserHandlerArgs handlerArgs) : base( handlerArgs)
        {
        }

        public async Task<WebResponse<CreateStaffUserResponse>> Handle(Shared.Models.WebService.StaffUser request)
        {
            try
            {
                if (!userContext.HasRole(PermissionConstants.StaffUserCreate))
                {
                    return new WebResponse<CreateStaffUserResponse> { Success = false, Message = "User not authorised to create users" };
                }

                var actionedByUser = await staffUserManager.GetByAccountId(userContext.AccountId, string.Empty, userContext.PlatformGuid);
                if (actionedByUser == null)
                {
                    throw new Exception("Unable to get logged on user details");
                }

                if (request.UserType == UserType.StaffUser)
                {
                    if (string.IsNullOrEmpty(request.Email))
                    {
                        return new WebResponse<CreateStaffUserResponse> { Success = false, Message = "Email required for staff users" };
                    }
                }

                if (!userContext.IsProviderUser())
                {
                    var parentAccountId = request.ParentAccountId == null ? string.Empty : request.ParentAccountId.ToString();
                    // if the user making the change is a tenant user, make sure they are only creating a user in their tenancy
                    if (request.TenantGuid != userContext.TenantGuid)
                    {
                        throw new Exception("Tenant users cannot add users to another tenancy");
                    }
                    if (string.IsNullOrEmpty(request.TenantGuid))
                    {
                        //staffUserDto.TenantGuid = userContext.TenantGuid;
                        throw new Exception("Tenant users cannot create provider users");
                    }
                    if (string.IsNullOrEmpty(parentAccountId.ToString()))
                    {
                        request.ParentAccountId = userContext.AccountId;
                    }
                    else
                    {
                        request.ParentAccountId = parentAccountId;
                    }
                }

              var   tenant = await GetTenant(request.TenantGuid);


                var password = GenerateRandomPassword();

                var identity = new ApplicationUser { UserName = request.Username, Email = request.Email, EmailConfirmed = true };
                var identityResult = await identityUserManager.CreateAsync(identity, password);

                if (!identityResult.Succeeded)
                {
                    return new WebResponse<CreateStaffUserResponse>
                    {
                        Success = false,
                        Message = string.Join(" | ", identityResult.Errors.Select(i => i.Code + ' ' + i.Description))
                    };
                }

                request.AccountId = identity.Id;

                var createUserResult = await staffUserManager.CreateStaffUser(request, actionedByUser, tenant?.Id, userContext.PlatformGuid);

                var platformGuid = userContext.PlatformGuid;

                if (request.PlatformRoleMappings != null)
                {
                    platformGuid = request.PlatformRoleMappings.Select(p => p.PlatformGuid).FirstOrDefault();
                }

                if (!await SendMail(createUserResult, "Your new account", $"Your account has been created and your new password is {password}", platformGuid))
                {
                    logger.LogError($"CreateStaffUserHandler.Handle: an error occurred when sending an email to create the staff user {request.Username}");
                }

                await SendEmailTenantCreatedStaffNotification(tenant, request);
                // TODO: what do we do if this fails?? should we do this before sending the email??
                // Dilek said any umbraco error should not stop the system work flow. In case of umbraco failing I will show an message on the user interface
                

                return new WebResponse<CreateStaffUserResponse>
                {
                    Success = true,
                    Payload = new CreateStaffUserResponse { StaffUser = createUserResult, Password = password }
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"CreateStaffUserHandler.Handle: an error occurred creating staff user {request.Username} - {ex}");
                throw;
            }
        }
    }
}
