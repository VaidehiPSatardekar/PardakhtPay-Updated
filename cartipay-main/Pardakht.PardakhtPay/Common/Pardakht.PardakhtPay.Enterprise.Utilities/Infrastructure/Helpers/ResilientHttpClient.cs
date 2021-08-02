using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.Helpers
{
    public class ResilientHttpClient : IHttpClient
    {
        private readonly HttpClient _client;
        private readonly IInvoker _invoker;
        private string _dependencyName;
        private string _dependencyFunction;

        public ResilientHttpClient(IOptions<PollyWrapperSettings> settings, HttpClient httpClient, string dependencyName, string dependencyFunction)
        {
            _client = httpClient;
            _client.Timeout = TimeSpan.FromSeconds(settings.Value.TimeOut == 0 ? 60 : settings.Value.TimeOut);
            _dependencyName = dependencyName;
            _dependencyFunction = dependencyFunction;
            _invoker = new PollyWrapper(settings);
        }

        private async Task<T> HttpInvokerAsync<T>(Func<Task<T>> action)
        {
            T myTask = default(T);
            myTask = await _invoker.ExecuteAsync(action);
            return myTask;
        }

        public Task<HttpResponseMessage> GetAsync(string uri)
        {
            return HttpInvokerAsync(async () =>
            {
                var requestMessage = new HttpRequestMessage { RequestUri = new Uri(uri), Method = HttpMethod.Get };
                return await _client.SendAsync(requestMessage);
            });
        }
        public Task<HttpResponseMessage> PostAsync(string uri, StringContent content)
        {
            return HttpInvokerAsync(async () =>
            {
                var requestMessage = new HttpRequestMessage { RequestUri = new Uri(uri), Content = content, Method = HttpMethod.Post };
                return await _client.SendAsync(requestMessage);
            });
        }

        public Task<HttpResponseMessage> PostFileAsync(string uri, MultipartFormDataContent content)
        {
            return HttpInvokerAsync(async () =>
            {
                var requestMessage = new HttpRequestMessage { RequestUri = new Uri(uri), Content = content, Method = HttpMethod.Post };
                return await _client.SendAsync(requestMessage);
            });
        }

        public Task<HttpResponseMessage> PutAsync(string uri, StringContent content)
        {
            return HttpInvokerAsync(async () =>
            {
                var requestMessage = new HttpRequestMessage { RequestUri = new Uri(uri), Content = content, Method = HttpMethod.Put };
                return await _client.SendAsync(requestMessage);
            });
        }
        public Task<HttpResponseMessage> PatchAsync(string uri, StringContent content)
        {
            return HttpInvokerAsync(async () =>
            {
                var requestMessage = new HttpRequestMessage { RequestUri = new Uri(uri), Content = content, Method = new HttpMethod("PATCH") };
                return await _client.SendAsync(requestMessage);
            });
        }

        public Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            return HttpInvokerAsync(async () =>
            {
                var requestMessage = new HttpRequestMessage { RequestUri = new Uri(uri), Method = HttpMethod.Delete };
                return await _client.SendAsync(requestMessage);
            });
        }

    }
}
