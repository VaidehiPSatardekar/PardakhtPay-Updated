using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
using Pardakht.UserManagement.Application.JwtToken;
using Pardakht.UserManagement.Application.StaffUser;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.TenantServiceApiAuth
{

    public class TenantServiceInfo
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expire { get; set; }
    }
    public class TenantServiceApiAuth
    {

        private readonly TenantManagementSettings tenantManagementApiSettings;
        private readonly IJwtTokenService jwtTokenService;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<TenantServiceApiAuth> logger;
        private readonly TenantServiceInfo tenantServiceInfo;

        public TenantServiceApiAuth(IOptions<TenantManagementSettings> tenantManagementApiSettings,
                                    IJwtTokenService jwtTokenService,
                                    IServiceProvider serviceProvider,
                                    TenantServiceInfo tenantServiceInfo,
                                    ILogger<TenantServiceApiAuth> logger)
        {
            this.tenantManagementApiSettings = tenantManagementApiSettings.Value;
            this.jwtTokenService = jwtTokenService;
            this.serviceProvider = serviceProvider;
            this.tenantServiceInfo = tenantServiceInfo;
            this.logger = logger;

            //GenerateNewToken();
        }

        public async Task GenerateNewToken()
        {
            try
            {
                var userService = serviceProvider.GetRequiredService<IStaffUserService>();
                var request = new ApiKeyLoginRequest { ApiKey = tenantManagementApiSettings.ApiKey, PlatformGuid = tenantManagementApiSettings.PlatformGuid };
                var response = await userService.ApiKeyLogin(request);

                if (!response.Success || response.Payload == null)
                {
                    tenantServiceInfo.Token = string.Empty;
                    tenantServiceInfo.RefreshToken = string.Empty;
                    return;
                }

                tenantServiceInfo.Expire = response.Payload.Expires;
                tenantServiceInfo.RefreshToken = response.Payload.RefreshToken;
                tenantServiceInfo.Token = response.Payload.AccessToken;


            }
            catch (Exception ex)
            {
                logger.LogError($"TenantServiceApiAuth.GenerateNewToken: an error occurred generating token - {ex}");
                throw;
            }
        }
    }
}
