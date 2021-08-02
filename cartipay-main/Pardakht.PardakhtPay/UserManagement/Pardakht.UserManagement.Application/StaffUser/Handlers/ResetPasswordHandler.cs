using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Domain.AuditLog;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public class ResetPasswordHandler : StaffUserHandlerBase
    {
        private readonly IAuditLogManager auditLogManager;

        public ResetPasswordHandler(  IAuditLogManager auditLogManager, StaffUserHandlerArgs handlerArgs) : base( handlerArgs)
        {
            this.auditLogManager = auditLogManager;
        }

        public async Task<WebResponse<PasswordResetResponse>> Handle(PasswordResetRequest request)
        {
            try
            {
                if (!userContext.HasRole(PermissionConstants.StaffUserResetPassword))
                {
                    return new WebResponse<PasswordResetResponse> { Success = false, Message = "User not authorised to reset passwords" };
                }

                var actionedByUser = await staffUserManager.GetByAccountId(userContext.AccountId, string.Empty, userContext.PlatformGuid);
                if (actionedByUser == null)
                {
                    throw new Exception("Unable to get logged on user details");
                }

                var staffUser = await staffUserManager.GetByAccountId(request.AccountId, string.Empty, userContext.PlatformGuid);
                if (staffUser == null)
                {
                    throw new Exception($"Staff user {request.AccountId} not found");
                }

                var tenantPlatformMapGuid = string.Empty;

                if (staffUser.TenantId.HasValue)
                {
                    tenantPlatformMapGuid = (await GetTenantPlatformMapping(staffUser.TenantId.Value, userContext.PlatformGuid))?.TenantPlatformMapGuid;
                }

                if (!userContext.IsProviderUser())
                {
                    // if the user making the change is a tenant user, make sure they are only resetting a user's password in their tenancy
                    if (actionedByUser.TenantId != staffUser.TenantId)
                    {
                        return new WebResponse<PasswordResetResponse> { Success = false, Message = "Tenant users cannot reset passwords for users in another tenancy" };
                    }
                    if (!staffUser.TenantId.HasValue)
                    {
                        return new WebResponse<PasswordResetResponse> { Success = false, Message = "Tenant users cannot reset passwords for provider users" };
                    }
                }

                if (staffUser.UserType == UserType.ApiUser)
                {
                    return new WebResponse<PasswordResetResponse> { Success = false, Message = "API keys cannot be reset" };
                }

                var identityUser = await identityUserManager.FindByNameAsync(staffUser.Username);
                if (identityUser == null)
                {
                    throw new Exception($"Identity user {staffUser.Username} not found");
                }

                var newPassword = GenerateRandomPassword();

                // apply the change
                string resetToken = await identityUserManager.GeneratePasswordResetTokenAsync(identityUser);
                IdentityResult passwordChangeResult = await identityUserManager.ResetPasswordAsync(identityUser, resetToken, newPassword);

                if (!passwordChangeResult.Succeeded)
                {
                    return new WebResponse<PasswordResetResponse>
                    {
                        Message = string.Join('|', passwordChangeResult.Errors.Select(p => p.Code)),
                        Success = false
                    };
                }
                staffUser.TenantGuid = tenantPlatformMapGuid;
                await SendEmailTenantBlockedStaffUserorDeletedorPasswordChange(staffUser, Shared.Models.WebService.Enum.UserAction.PasswordChange);

               

                string platformGuid = request.PlatformGuid;

                if (staffUser.PlatformRoleMappings != null)
                {
                    platformGuid = staffUser.PlatformRoleMappings.Select(p => p.PlatformGuid).FirstOrDefault();
                }

                // send email to user
                var sendMailResult = await SendMail(staffUser, "Password Reset", $"Your password has been reset. Your new password is {newPassword}", platformGuid);
                //var sendMailResult = await SendMail(staffUser, newPassword);
                if (!sendMailResult)
                {
                    logger.LogError($"ResetPasswordHandler.Handle: sending staff user reset password email failed for {request.AccountId}");
                }

                // add audit log entry
                await auditLogManager.CreateAuditLogEntry(staffUser, AuditType.User, AuditActionType.UserResetPassword, "Password reset",staffUser.Id);

                // return password
                return new WebResponse<PasswordResetResponse>
                {
                    Success = true,
                    Payload = new PasswordResetResponse { NewPassword = newPassword, Message = !sendMailResult ? " Password changed but email not sent to user." : string.Empty
                    }
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"ResetPasswordHandler.Handle: an error occurred resetting staff user password {request.AccountId} - {ex}");
                throw;
            }
        }
    }
}
