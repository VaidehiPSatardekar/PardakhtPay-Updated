//using Newtonsoft.Json;
//using System;
//using System.Net.Http;
//using System.Threading.Tasks;

//{
//    public interface IGenericHttpInvoker
//    {
//        Task<GenericHttpPostResponse<T>> Get<T>(string url);
//        Task<GenericHttpPostResponse<T>> Post<T>(string url, object request);
//    }

//    public class GenericHttpInvoker : IGenericHttpInvoker
//    {
//        private readonly IHttpClientFactory _httpClientFactory;

//        public GenericHttpInvoker(IHttpClientFactory httpClientFactory)
//        {
//            _httpClientFactory = httpClientFactory;
//        }

//        public async Task<GenericHttpPostResponse<T>> Get<T>(string url, HttpHeader httpHeader)
//        {
//            try
//            {

//                serviceProvider.GetRequiredService<IGenericManagementFunctions<TimeZoneCalendarManagementSettings>>();

//                var client = _httpClientFactory.CreateClient(typeof(T).Name);
//                client.DefaultRequestHeaders.Add()
//                var response = await client.GetAsync(url);
//                var contents = await response.Content.ReadAsStringAsync();

//                return await ProcessResponse<T>(response, url);
//            }
//            catch (Exception ex)
//            {
//                return new GenericHttpPostResponse<T> { Success = false, Message = ex.Message };
//            }
//        }

//        public async Task<GenericHttpPostResponse<T>> Post<T>(string url, object request, HttpHeader httpHeader)
//        {
//            try
//            {
//                var body = JsonConvert.SerializeObject(request);

//                var response = await universalApiService.PostAsync(url, httpHeader, body);

//                return await ProcessResponse<T>(response, url);
//            }
//            catch (Exception ex)
//            {
//                return new GenericHttpPostResponse<T> { Success = false, Message = ex.Message };
//            }
//        }

//        private async Task<GenericHttpPostResponse<T>> ProcessResponse<T>(HttpResponseMessage response, string url)
//        {
//            var contents = await response.Content.ReadAsStringAsync();
//            if (!response.IsSuccessStatusCode)
//            {
//                return new GenericHttpPostResponse<T>
//                {
//                    Success = false,
//                    Message = $"Call to {url} unsuccessful - {response.StatusCode} - {contents}",
//                    InnerMessage = contents,
//                    StatusCode = response.StatusCode
//                };
//            }

//            return new GenericHttpPostResponse<T>
//            {
//                Success = true,
//                Message = contents,
//                StatusCode = response.StatusCode,
//                Payload = JsonConvert.DeserializeObject<T>(contents)
//            };
//        }
//    }
//}
