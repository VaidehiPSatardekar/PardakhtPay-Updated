//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Options;
//{
//    public class UniversalApiService : IUniversalApiService
//    {
//        private HttpClient _client;
//        private readonly IOptions<PollyWrapperSettings> _settings;

//        public UniversalApiService(IOptions<PollyWrapperSettings> settings)
//        {
//            _settings = settings;
//        }

//        private void Initialize(HttpHeader httpHeader)
//        {
//            _client = new HttpClient();
//            if (_settings.Value.TimeOut > 0)
//            {
//                _client.Timeout = new TimeSpan(0, 0, 0, _settings.Value.TimeOut);
//            }
//            _client.GenerateHeaders(httpHeader);
//        }

//        private void Initialize(HttpHeader httpHeader, string ipAddress)
//        {
//            _client = new HttpClient();
//            var handler = new HttpClientHandler()
//            {
//                Proxy = new WebProxy(ipAddress),
//                UseProxy = true,
//            };
//            _client = new HttpClient(handler);
//            if (_settings.Value.TimeOut > 0)
//            {
//                _client.Timeout = new TimeSpan(0, 0, 0, _settings.Value.TimeOut);
//            }
//            _client.GenerateHeaders(httpHeader);

//        }

//        private void Initialize(HttpHeader httpHeader, string ipAddress, string username, string password)
//        {
//            // First create a proxy object
//            var proxy = new WebProxy
//            {
//                Address = new Uri($"http://{ipAddress}"),
//                BypassProxyOnLocal = false,
//                UseDefaultCredentials = false,

//                // *** These creds are given to the proxy server, not the web server ***
//                Credentials = new NetworkCredential(
//                    userName: username,
//                    password: password)
//            };

//            // Now create a client handler which uses that proxy
//            var httpClientHandler = new HttpClientHandler
//            {
//                Proxy = proxy,
//            };


//            // Finally, create the HTTP client object
//            _client = new HttpClient(handler: httpClientHandler, disposeHandler: true);
//            _client.GenerateHeaders(httpHeader);
//        }


//        public Task<HttpResponseMessage> GetAsync(string url, HttpHeader httpHeader, string dependencyName,
//            string dependencyFunction, string accept = "application/json")
//        {
//            Initialize(httpHeader);

//            ResilientHttpClient CreateResilientHttpClient() =>
//                new ResilientHttpClient(_settings, _client, dependencyName, dependencyFunction);

//            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
//            return CreateResilientHttpClient().GetAsync(url);
//        }

//        public Task<HttpResponseMessage> PostAsync(string url, HttpHeader httpHeader, string dependencyName,
//            string dependencyFunction, string post, string accept = "application/json",
//            string contentType = "application/json")
//        {
//            Initialize(httpHeader);

//            ResilientHttpClient CreateResilientHttpClient() =>
//                new ResilientHttpClient(_settings, _client, dependencyName, dependencyFunction);

//            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
//            return CreateResilientHttpClient()
//                .PostAsync(url, new StringContent(post, Encoding.UTF8, contentType));

//        }

//        public Task<HttpResponseMessage> PutAsync(string url, HttpHeader httpHeader, string dependencyName,
//            string dependencyFunction, string post, string accept = "application/json",
//            string contentType = "application/json")
//        {
//            Initialize(httpHeader);

//            ResilientHttpClient CreateResilientHttpClient() =>
//                new ResilientHttpClient(_settings, _client, dependencyName, dependencyFunction);

//            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
//            return CreateResilientHttpClient().PutAsync(url, new StringContent(post, Encoding.UTF8, contentType));
//        }

//        public Task<HttpResponseMessage> PatchAsync(string url, HttpHeader httpHeader, string dependencyName,
//            string dependencyFunction, string post, string accept = "application/json",
//            string contentType = "application/json")
//        {
//            Initialize(httpHeader);

//            ResilientHttpClient CreateResilientHttpClient() =>
//                new ResilientHttpClient(_settings, _client, dependencyName, dependencyFunction);

//            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
//            return CreateResilientHttpClient()
//                .PatchAsync(url, new StringContent(post, Encoding.UTF8, contentType));
//        }

//        public Task<HttpResponseMessage> DeleteAsync(string url, HttpHeader httpHeader, string dependencyName,
//            string dependencyFunction, string accept = "application/json")
//        {
//            Initialize(httpHeader);

//            ResilientHttpClient CreateResilientHttpClient() =>
//                new ResilientHttpClient(_settings, _client, dependencyName, dependencyFunction);

//            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
//            return CreateResilientHttpClient().DeleteAsync(url);
//        }

//        public Task<HttpResponseMessage> GetAsync(string url, HttpHeader httpHeader,
//            string accept = "application/json")
//        {
//            return GetAsync(url, httpHeader, string.Empty, string.Empty, accept);
//        }

//        public Task<HttpResponseMessage> PutProxyAsync(string url, HttpHeader httpHeader, string body,
//            string ipAddress, string username, string password,
//            string accept = "application/json", string contentType = "application/json")
//        {
//            Initialize(httpHeader, ipAddress, username, password);

//            ResilientHttpClient CreateResilientHttpClient() =>
//                new ResilientHttpClient(_settings, _client, string.Empty, string.Empty);

//            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
//            return CreateResilientHttpClient().PutAsync(url, new StringContent(body, Encoding.UTF8, contentType));
//        }

//        public Task<HttpResponseMessage> PostAsync(string url, HttpHeader httpHeader, string post,
//            string accept = "application/json", string contentType = "application/json")
//        {
//            return PostAsync(url, httpHeader, string.Empty, string.Empty, post, accept, contentType);
//        }

//        public Task<HttpResponseMessage> PostProxyAsync(string url, HttpHeader httpHeader, string body,
//            string ipAddress, string username, string password,
//            string accept = "application/json", string contentType = "application/json")
//        {
//            Initialize(httpHeader, ipAddress, username, password);

//            ResilientHttpClient CreateResilientHttpClient() =>
//                new ResilientHttpClient(_settings, _client, string.Empty, string.Empty);

//            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
//            return CreateResilientHttpClient()
//                .PostAsync(url, new StringContent(body, Encoding.UTF8, contentType));
//        }

//        public Task<HttpResponseMessage> GetProxyAsync(string url, HttpHeader httpHeader, string ipAddress,
//            string username, string password,
//            string accept = "application/json")
//        {
//            Initialize(httpHeader, ipAddress, username, password);

//            ResilientHttpClient CreateResilientHttpClient() =>
//                new ResilientHttpClient(_settings, _client, string.Empty, string.Empty);

//            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
//            return CreateResilientHttpClient().GetAsync(url);
//        }

//        public Task<HttpResponseMessage> PutAsync(string url, HttpHeader httpHeader, string post,
//            string accept = "application/json", string contentType = "application/json")
//        {
//            return PutAsync(url, httpHeader, string.Empty, string.Empty, post, accept, contentType);
//        }

//        public Task<HttpResponseMessage> PatchAsync(string url, HttpHeader httpHeader, string post,
//            string accept = "application/json", string contentType = "application/json")
//        {
//            return PatchAsync(url, httpHeader, string.Empty, string.Empty, post, accept, contentType);
//        }

//        public Task<HttpResponseMessage> DeleteAsync(string url, HttpHeader httpHeader,
//            string accept = "application/json")
//        {
//            return DeleteAsync(url, httpHeader, string.Empty, string.Empty, accept);
//        }

//        public Task<HttpResponseMessage> DeleteProxyAsync(string url, HttpHeader httpHeader, string ipAddress,
//            string username, string password,
//            string accept = "application/json")
//        {
//            Initialize(httpHeader, ipAddress);

//            ResilientHttpClient CreateResilientHttpClient() =>
//                new ResilientHttpClient(_settings, _client, string.Empty, string.Empty);

//            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
//            return CreateResilientHttpClient().DeleteAsync(url);
//        }

//        public Task<HttpResponseMessage> PosCollectionsAsync(string url, HttpHeader httpHeader,
//             IDictionary<string, StreamContent> streamContents, IDictionary<string, StringContent> stringContents, string accept = "application/json")
//        {
//            string boundary = Guid.NewGuid().ToString();
//            var content = new MultipartFormDataContent(boundary);
//            content.Headers.Remove("Content-Type");
//            content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);

//            if (streamContents != null)
//            {
//                foreach (var key in streamContents.Keys)
//                {
//                    content.Add(streamContents[key]);
//                }
//            }

//            if (stringContents != null)
//            {
//                foreach (var key in stringContents.Keys)
//                {
//                    content.Add(stringContents[key]);
//                }
//            }

//            Initialize(httpHeader);

//            ResilientHttpClient CreateResilientHttpClient() =>
//                new ResilientHttpClient(_settings, _client, string.Empty, string.Empty);

//            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));

//            return CreateResilientHttpClient().PostFileAsync(url, content);
//        }

//        public Task<HttpResponseMessage> PostFileAsync(string url, HttpHeader httpHeader,
//            StreamContent streamContent, string accept = "application/json")
//        {
//            string boundary = Guid.NewGuid().ToString();
//            var content = new MultipartFormDataContent(boundary);
//            content.Headers.Remove("Content-Type");
//            content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);
//            content.Add(streamContent);

//            Initialize(httpHeader);

//            ResilientHttpClient CreateResilientHttpClient() =>
//                new ResilientHttpClient(_settings, _client, string.Empty, string.Empty);

//            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));

//            return CreateResilientHttpClient().PostFileAsync(url, content);
//        }

//        public Task<HttpResponseMessage> PostFileProxyAsync(string url, HttpHeader httpHeader,
//            StreamContent streamContent, string ipAddress,
//            string username, string password, string accept = "application/json")
//        {
//            string boundary = Guid.NewGuid().ToString();
//            var content = new MultipartFormDataContent(boundary);
//            content.Headers.Remove("Content-Type");
//            content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);
//            content.Add(streamContent);

//            Initialize(httpHeader, ipAddress, username, password);

//            ResilientHttpClient CreateResilientHttpClient() =>
//                new ResilientHttpClient(_settings, _client, string.Empty, string.Empty);

//            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));

//            return CreateResilientHttpClient().PostFileAsync(url, content);
//        }

//        public Task<HttpResponseMessage> GetProxyAsync(string url, HttpHeader httpHeader, string ipAddress,
//            string accept = "application/json")
//        {
//            Initialize(httpHeader, ipAddress);

//            ResilientHttpClient CreateResilientHttpClient() =>
//                new ResilientHttpClient(_settings, _client, string.Empty, string.Empty);

//            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));

//            return CreateResilientHttpClient().GetAsync(url);
//        }

//        public Task<HttpResponseMessage> PutProxyAsync(string url, HttpHeader httpHeader, string body,
//            string ipAddress, string accept = "application/json", string contentType = "application/json")
//        {
//            Initialize(httpHeader, ipAddress);

//            ResilientHttpClient CreateResilientHttpClient() =>
//                new ResilientHttpClient(_settings, _client, string.Empty, string.Empty);

//            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
//            return CreateResilientHttpClient().PutAsync(url, new StringContent(body, Encoding.UTF8, contentType));
//        }

//        public Task<HttpResponseMessage> PostProxyAsync(string url, HttpHeader httpHeader, string body,
//            string ipAddress, string accept = "application/json", string contentType = "application/json")
//        {
//            Initialize(httpHeader, ipAddress);

//            ResilientHttpClient CreateResilientHttpClient() =>
//                new ResilientHttpClient(_settings, _client, string.Empty, string.Empty);

//            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
//            return CreateResilientHttpClient()
//                .PostAsync(url, new StringContent(body, Encoding.UTF8, contentType));
//        }

//        public Task<HttpResponseMessage> PostFileProxyAsync(string url, HttpHeader httpHeader,
//            StreamContent streamContent, string ipAddress, string accept = "application/json")
//        {
//            var boundary = Guid.NewGuid().ToString();
//            var content = new MultipartFormDataContent(boundary);
//            content.Headers.Remove("Content-Type");
//            content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);
//            content.Add(streamContent);

//            Initialize(httpHeader, ipAddress);

//            ResilientHttpClient CreateResilientHttpClient() =>
//                new ResilientHttpClient(_settings, _client, string.Empty, string.Empty);

//            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
//            return CreateResilientHttpClient().PostFileAsync(url, content);
//        }

//        public Task<HttpResponseMessage> PatchProxyAsync(string url, HttpHeader httpHeader, string body,
//            string ipAddress, string accept = "application/json", string contentType = "application/json")
//        {
//            Initialize(httpHeader, ipAddress);

//            ResilientHttpClient CreateResilientHttpClient() =>
//                new ResilientHttpClient(_settings, _client, string.Empty, string.Empty);

//            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
//            return CreateResilientHttpClient()
//                .PatchAsync(url, new StringContent(body, Encoding.UTF8, contentType));
//        }

//        public Task<HttpResponseMessage> DeleteProxyAsync(string url, HttpHeader httpHeader, string ipAddress,
//            string accept = "application/json")
//        {
//            Initialize(httpHeader, ipAddress);

//            ResilientHttpClient CreateResilientHttpClient() =>
//                new ResilientHttpClient(_settings, _client, string.Empty, string.Empty);

//            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
//            return CreateResilientHttpClient().DeleteAsync(url);
//        }
//    }
//}

