using System.Threading.Tasks;
using Pardakht.UserManagement.Application.JwtToken;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public class StaffUserLoginHandler : LoginHandlerBase
    {
        public StaffUserLoginHandler(IJwtTokenService jwtTokenService, StaffUserHandlerArgs handlerArgs) : base(jwtTokenService, handlerArgs) { }

        public async Task<WebResponse<LoginResponse>> Handle(LoginRequest request)
        {
            // TODO: check if specific messages ok, or if we want a generic 'login failed' message
            var validationResult = await ValidateUser(request.UserName, userContext.PlatformGuid);

            if (!string.IsNullOrEmpty(validationResult.validationError))
            {
                return new WebResponse<LoginResponse> { Success = false, Message = validationResult.validationError };
            }

            var passwordCheck = await identityUserManager.CheckPasswordAsync(validationResult.identityUser, request.Password);
            if (!passwordCheck)
            {
                return new WebResponse<LoginResponse> { Success = false, Message = "Incorrect password" };
            }

            if (!string.IsNullOrEmpty(request.LoginAsUsername) && validationResult.userDetails.HasPermissionForTask(PermissionConstants.LoginAsAUser))
            {
                validationResult = await ValidateUser(request.LoginAsUsername, userContext.PlatformGuid);
            }
            if (validationResult.userDetails.UserType == UserType.AffiliateUser)
            {
                if ((request.TenantId != null &&
                    validationResult.userDetails.TenantId != request.TenantId) ||
                    (request.BrandId != null &&
                    validationResult.userDetails.BrandId != request.BrandId))

                {
                    return new WebResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "You don't have a permission to access this page"
                    };
                }
            }

            var tokenResponse = await GenerateTokenResponse(validationResult.userDetails, userContext.PlatformGuid);

            if (tokenResponse.Success)
            {
                await staffUserManager.LoginStaffUser(validationResult.userDetails, validationResult.userDetails, validationResult.userDetails.TenantGuid, userContext.PlatformGuid);
                return new WebResponse<LoginResponse>
                {
                    Success = true,
                    Payload = new LoginResponse { User = validationResult.userDetails, Token = tokenResponse.Payload }
                };
            }
            else
            {
                return new WebResponse<LoginResponse>
                {
                    Success = false,
                    Message = tokenResponse.Message
                };
            }
        }
    }
}
