using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Domain.AuditLog;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public class ChangePasswordHandler : StaffUserHandlerBase
    {
        private readonly IAuditLogManager auditLogManager;

        public ChangePasswordHandler( IAuditLogManager auditLogManager, StaffUserHandlerArgs handlerArgs) : base( handlerArgs)
        {
            this.auditLogManager = auditLogManager;
        }

        public async Task<WebResponse<PasswordChangeResponse>> Handle(PasswordChangeRequest request)
        {
            try
            {
                var username = userContext.AccountId;
                var identityUser = await identityUserManager.FindByIdAsync(username);
                if (identityUser == null)
                {
                    logger.LogWarning($"ChangePassword: unable to locate user id {username}");
                    return new WebResponse<PasswordChangeResponse>
                    {
                        Message = "Unable to locate user",
                        Success = false
                    };
                }

                var result = await identityUserManager.ChangePasswordAsync(identityUser, request.OldPassword, request.NewPassword);

                

                if (!result.Succeeded)
                {
                    return new WebResponse<PasswordChangeResponse>
                    {
                        Message = String.Join('|', result.Errors.Select(p => p.Code)),
                        Success = false
                    };
                }

                return new WebResponse<PasswordChangeResponse>
                {
                    Success = true,
                    Payload = new PasswordChangeResponse
                    {
                        UserId = identityUser.Id,
                        Message =  string.Empty
                    }
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"ChangePasswordHandler.Handle: an error occurred changing staff user password, {userContext.AccountId} - {ex}");
                throw;
            }
        }
        
    }
}
