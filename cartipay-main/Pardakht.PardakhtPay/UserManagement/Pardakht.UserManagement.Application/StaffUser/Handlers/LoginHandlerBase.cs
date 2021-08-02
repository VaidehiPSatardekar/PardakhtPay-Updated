using System;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.UserManagement.Application.JwtToken;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public abstract class LoginHandlerBase : StaffUserHandlerBase
    {
        private readonly IJwtTokenService jwtTokenService;

        public LoginHandlerBase(IJwtTokenService jwtTokenService, StaffUserHandlerArgs handlerArgs) : base(handlerArgs)
        {
            this.jwtTokenService = jwtTokenService;
        }

        protected async Task<(string validationError, ApplicationUser identityUser, Shared.Models.WebService.StaffUser userDetails)> ValidateUser(string username, string platformGuid)
        {
            var userToVerify = await identityUserManager.FindByNameAsync(username);
            if (userToVerify == null)
            {
                return (validationError: "Invalid user", identityUser: null, userDetails: null);
            }

            var userDetails = await staffUserManager.GetByAccountId(userToVerify.Id, string.Empty, platformGuid);
            if (userDetails == null)
            {
                return (validationError: "User not found", identityUser: null, userDetails: null);
            }

            if (userDetails.IsBlocked)
            {
                return (validationError: "User is blocked", identityUser: null, userDetails: null);
            }

            if (userDetails.IsDeleted)
            {
                return (validationError: "User is deleted", identityUser: null, userDetails: null);
            }

            if (userDetails.UserType == UserType.StaffUser && // may need to review this
                (userDetails.PlatformRoleMappings == null || userDetails.PlatformRoleMappings.Count == 0 || !userDetails.PlatformRoleMappings.Any(m => m.PlatformGuid == platformGuid)))
            {
                return (validationError: "User is not mapped to this platform", identityUser: null, userDetails: null);
            }

            

            return (validationError: string.Empty, identityUser: userToVerify, userDetails: userDetails);
        }

        protected async Task<WebResponse<JsonWebToken>> GenerateTokenResponse(Shared.Models.WebService.StaffUser userDetails, string platformGuid)
        {
            var token = await jwtTokenService.CreateToken(userDetails, platformGuid);
            if (string.IsNullOrEmpty(token))
            {
                return new WebResponse<JsonWebToken>
                {
                    Success = false,
                    Message = "Token generation error"
                };
            }

            return new WebResponse<JsonWebToken>
            {
                Success = true,
                Payload = new JsonWebToken
                {
                    AccessToken = token,
                    RefreshToken = "not-ready",
                    Expires = DateTime.UtcNow.AddHours(1)
                }
            };
        }
    }
}
