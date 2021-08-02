using System.Threading.Tasks;
using Pardakht.UserManagement.Application.JwtToken;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public class ApiKeyLoginHandler : LoginHandlerBase
    {
        public ApiKeyLoginHandler(IJwtTokenService jwtTokenService, StaffUserHandlerArgs handlerArgs) : base(jwtTokenService, handlerArgs) { }

        public async Task<WebResponse<JsonWebToken>> Handle(ApiKeyLoginRequest request)
        {
            var validationResult = await ValidateUser(request.ApiKey, request.PlatformGuid);

            if (!string.IsNullOrEmpty(validationResult.validationError))
            {
                return new WebResponse<JsonWebToken> { Success = false, Message = validationResult.validationError };
            }

            return await GenerateTokenResponse(validationResult.userDetails, request.PlatformGuid);
        }
    }
}
