//using Microsoft.IdentityModel.JsonWebTokens;

using System.Threading.Tasks;
using Pardakht.UserManagement.Application.JwtToken;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public class RefreshTokenHandler : LoginHandlerBase
    {
        public RefreshTokenHandler(IJwtTokenService jwtTokenService, StaffUserHandlerArgs handlerArgs) : base(jwtTokenService, handlerArgs) { }

        public async Task<WebResponse<JsonWebToken>> Handle()
        {
            var validationResult = await ValidateUser(userContext.Username, userContext.PlatformGuid);

            if (!string.IsNullOrEmpty(validationResult.validationError))
            {
                return new WebResponse<JsonWebToken> { Success = false, Message = validationResult.validationError };
            }

            return await GenerateTokenResponse(validationResult.userDetails, userContext.PlatformGuid);
        }
    }
}
