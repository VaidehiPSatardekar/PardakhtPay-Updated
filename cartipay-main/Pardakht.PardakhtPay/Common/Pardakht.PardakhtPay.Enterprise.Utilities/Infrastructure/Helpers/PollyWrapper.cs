using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
using Polly;
using Polly.Timeout;
using Polly.Wrap;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.Helpers
{
    public class PollyWrapper : IInvoker
    {
        private readonly AsyncPolicyWrap _policyWrapper;
        private readonly TimeoutStrategy _timeoutStrategy;
        private PollyWrapperSettings _settings;

        public PollyWrapper(IOptions<PollyWrapperSettings> settings, TimeoutStrategy timeoutStrategy = TimeoutStrategy.Pessimistic)
        {
            _settings = settings.Value;
            _policyWrapper = Policy.WrapAsync(CreatePolicies());
            _timeoutStrategy = timeoutStrategy;
        }
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            return await _policyWrapper.ExecuteAsync(action);
        }

        private IAsyncPolicy[] CreatePolicies() => new IAsyncPolicy[]   {

            //Policy.TimeoutAsync(Convert.ToInt32(settings.TimeOut), timeoutStrategy),
            Policy.TimeoutAsync(_settings.TimeOut == 0 ? 60 : Math.Min(_settings.TimeOut, 60), _timeoutStrategy),
            Policy.Handle<HttpRequestException>()
                .WaitAndRetryAsync( 
                    // number of retries Convert.ToInt32(5),
                    _settings.MaxRetry == 0 ? 5 : Math.Min(_settings.MaxRetry, 5),
                    // exponential backoff
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    // on retry
                    (exception, timeSpan, retryCount, context) =>
                    {
                        //var msg = $"Retry {retryCount} implemented with Pollys RetryPolicy " +
                        //          $"of {context.PolicyKey} " +
                        //          $"at {context.ExecutionKey}, " +
                        //          $"due to: {exception}.";

                    })
        };
    }
}
