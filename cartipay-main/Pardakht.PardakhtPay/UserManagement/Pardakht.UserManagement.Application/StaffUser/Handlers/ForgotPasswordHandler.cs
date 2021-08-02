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
    public class ForgotPasswordHandler : StaffUserHandlerBase
    {
        private readonly IAuditLogManager auditLogManager;
        public ForgotPasswordHandler( IAuditLogManager auditLogManager, StaffUserHandlerArgs handlerArgs) : base( handlerArgs)
        {
            this.auditLogManager = auditLogManager;
        }

        public async Task<WebResponse> Handle(ForgotResetRequest request)
        {
            try
            {
                var staffUser = await staffUserManager.GetByEmail(request.Email);
                if (staffUser == null)
                {
                    throw new Exception($"Staff user {request.Email} not found");
                }

                var identityUser = await identityUserManager.FindByNameAsync(staffUser.Username);
                if (identityUser == null)
                {
                    throw new Exception($"Identity user {staffUser.Username} not found");
                }

                // TODO: from here down is pretty much the same as in reset password - we could refactor into a common parent class
                var newPassword = GenerateRandomPassword();

                // apply the change
                string resetToken = await identityUserManager.GeneratePasswordResetTokenAsync(identityUser);
                IdentityResult passwordChangeResult = await identityUserManager.ResetPasswordAsync(identityUser, resetToken, newPassword);
                if (!passwordChangeResult.Succeeded)
                {
                    return new WebResponse
                    {
                        Message = string.Join('|', passwordChangeResult.Errors.Select(p => p.Code)),
                        Success = false
                    };
                }
                
                

                string platformGuid = request.PlatformGuid;

                if (staffUser.PlatformRoleMappings != null)
                {
                    platformGuid = staffUser.PlatformRoleMappings.Select(p => p.PlatformGuid).FirstOrDefault();
                }

                // send email to user
                var sendMailResult = await SendMail(staffUser, "Password Reset", $"Your password has been reset. Your new password is {newPassword}", platformGuid);
                if (!sendMailResult)
                {
                    logger.LogError($"ForgotPasswordHandler.Handle: sending staff user reset password email failed for {staffUser.AccountId}");
                }

                // add audit log entry
                await auditLogManager.CreateAuditLogEntry(staffUser, AuditType.User, AuditActionType.UserForgotPassword, "Forgot password",staffUser.Id);

                return new WebResponse { Success = true };
            }
            catch (Exception ex)
            {
                logger.LogError($"ForgotPasswordHandler.Handle: an error occurred resetting staff user password {request.Email} - {ex}");
                throw;
            }
        }
    }
}
