//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
//using System;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.ApiKey;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
//using Pardakht.PardakhtPay.Shared.Interfaces;
//using Pardakht.PardakhtPay.Shared.Models.Configuration;
//using Pardakht.PardakhtPay.Shared.Models.WebService.Sms;

//namespace Pardakht.PardakhtPay.Shared.Services
//{
//    public class SmsService : ISmsService
//    {
//        IApplicationSettingService _ApplicationSettingService = null;
//        static object _LockObject = new object();
//        ILogger _Logger = null;
//        IHttpClientFactory _HttpClientFactory = null;
//        GenericManagementTokenGenerator<TenantManagementSettings> _TokenGenerator = null;

//        public SmsService(
//            IApplicationSettingService applicationSettingService,
//            ILogger<SmsService> logger,
//            IHttpClientFactory httpClientFactory,
//            GenericManagementTokenGenerator<TenantManagementSettings> tokenGenerator)
//        {
//            _ApplicationSettingService = applicationSettingService;
//            _Logger = logger;
//            _HttpClientFactory = httpClientFactory;
//            _TokenGenerator = tokenGenerator;
//        }

//        public async Task<SmsServiceSendResponse> SendSms(SmsServiceRequest request)
//        {
//            var configuration = await _ApplicationSettingService.Get<SmsServiceConfiguration>();
//            request.SecretKey = configuration.SmsSecretKey;
//            request.UserApiKey = configuration.SmsApiKey;
//            request.TemplateId = configuration.TemplateId;

//            var client = _HttpClientFactory.CreateClient();

//            string url = configuration.Url + "/api/sms/send-sms";

//            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _TokenGenerator.Token);

//            using (var response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")))
//            {
//                if (response.IsSuccessStatusCode)
//                {
//                    var content = await response.Content.ReadAsStringAsync();

//                    var smsResponse = JsonConvert.DeserializeObject<SmsServiceSendResponse>(content);
//                    smsResponse.ValidationEndDate = DateTime.UtcNow.Add(configuration.ExpireTime);

//                    return smsResponse;
//                }
//                else
//                {
//                    var content = await response.Content.ReadAsStringAsync();

//                    _Logger.LogError($"Sms service return error code {response.StatusCode}. Message : {content}");

//                    return new SmsServiceSendResponse()
//                    {
//                        IsSent = false
//                    };
//                }
//            }
//        }

//        public Task<SmsServiceBalanceResponse> CheckBalance(SmsServiceBalanceRequest request)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
