using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pardakht.PardakhtPay.BankBotWebJobService
{
    public class WebJobHost : IHostedService, IDisposable
    {
        ILogger _Logger = null;

        public WebJobHost(ILogger<WebJobHost> logger)
        {
            _Logger = logger;
        }

        public void Dispose()
        {
            
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _Logger.LogInformation("Service started");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _Logger.LogInformation("Service stopped");

            return Task.CompletedTask;
        }
    }
}
