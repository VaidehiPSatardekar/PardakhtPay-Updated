using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pardakht.UserManagement.Application.JwtToken;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.StaffUser.Handlers
{
    public class StaffUserLoginAsHandler : LoginHandlerBase
    {
        private readonly ILogger _logger;

        public StaffUserLoginAsHandler(IJwtTokenService jwtTokenService, ILogger logger, StaffUserHandlerArgs handlerArgs) : base(jwtTokenService, handlerArgs) 
        {

            _logger = logger;
        }

        public async Task<WebResponse<LoginResponse>> Handle(LoginAsRequest request)
        {
            if (userContext.HasRole(PermissionConstants.LoginAsAUser))
            {
                var validationResult = await ValidateUser(request.UserName, userContext.PlatformGuid);

                var tokenResponse = await GenerateTokenResponse(validationResult.userDetails, userContext.PlatformGuid);

                _logger.LogWarning("{@message} {@actionType} {@type} {@typeId} {@username} {@createdbyaccountId}",
                                    $"{userContext.Username} started to login as {request.UserName}",
                                    ((int)AuditActionType.StaffUserLoginAs).ToString(),
                                    ((int)AuditType.User).ToString(),
                                    "0", 
                                    userContext.Username, userContext.AccountId);
                if (tokenResponse.Success)
                {


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

            return new WebResponse<LoginResponse>()
            {
                Success = false,
                Message = "Not allowed"
            };

        }
    }
}
