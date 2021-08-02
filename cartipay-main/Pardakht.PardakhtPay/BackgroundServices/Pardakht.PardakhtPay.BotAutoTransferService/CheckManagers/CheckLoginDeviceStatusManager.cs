using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.BotAutoTransferService.Interfaces;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.GenericManagementApi;
using Pardakht.PardakhtPay.Shared.Models.Configuration;

namespace Pardakht.PardakhtPay.BotAutoTransferService.CheckManagers
{
    public class CheckLoginDeviceStatusManager : ICheckLoginDeviceStatusManager
    {
        ILogger _Logger = null;
        IServiceProvider _Provider;

        public CheckLoginDeviceStatusManager(ILogger<CheckLoginDeviceStatusManager> logger,
            IServiceProvider provider)
        {
            _Logger = logger;
            _Provider = provider;
        }

        public async Task Run()
        {
            try
            {
                using (var scope = _Provider.CreateScope())
                {
                    var statusCode = await CallPardakhtPay(scope.ServiceProvider);
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }

        private async Task<HttpStatusCode> CallPardakhtPay(IServiceProvider provider)
        {
            var functions = provider.GetRequiredService<IGenericManagementFunctions<PardakhtPayAuthenticationSettings>>();

            using (var response = await functions.MakeRequest(null, null, "/api/banklogin/getloginlistdevicestatus", HttpMethod.Post))
            {
                return response.StatusCode;
            }
        }
    }
}
