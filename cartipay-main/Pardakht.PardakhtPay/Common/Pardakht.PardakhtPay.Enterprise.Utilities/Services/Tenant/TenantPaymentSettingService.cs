//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//{
//    public class TenantPaymentSettingService : ITenantPaymentSettingService
//    {
//        private readonly IUniversalApiService _universalApiService;
//        private readonly TenantManagementSettings _tenantManagementSettings;
//        private readonly GenericManagementTokenGenerator<TenantManagementSettings> _genericManagementTokenGenerator;
//        private readonly ILogger<TenantPaymentSettingService> _logger;

//        public TenantPaymentSettingService(IUniversalApiService universalApiService, ILogger<TenantPaymentSettingService> logger,
//             GenericManagementTokenGenerator<TenantManagementSettings> genericManagementTokenGenerator,
//             IOptions<TenantManagementSettings> tenantManagementSettings)
//        {
//            _universalApiService = universalApiService;
//            _genericManagementTokenGenerator = genericManagementTokenGenerator;
//            _tenantManagementSettings = tenantManagementSettings.Value;
//            _logger = logger;
//        }


//        public async Task<IEnumerable<TenantPaymentListResponse>> GetTenantPaymentSettings(string tenantPlatformMapGuid)
//        {
//            throw new NotImplementedException();
//            //var token = _genericManagementTokenGenerator.Token;
//            //if (string.IsNullOrEmpty(token))
//            //{
//            //    _logger.LogError("TenantService: unable to generate authorization token");
//            //}

//            //var httpHeader = new HttpHeader
//            //{
//            //    Headers = new Dictionary<string, string>()
//            //    {
//            //        { "Authorization", $"Bearer {token}" }
//            //    }
//            //};

//            //var body = JsonConvert.SerializeObject(new
//            //{
//            //    TenantPlatformMapGuid = tenantPlatformMapGuid
//            //});
//            //var url = $"{_tenantManagementSettings.Url}/api/tenantplatform/getTenantPaymentList";
//            //var response = await _universalApiService.PostAsync(url, httpHeader, body);
//            //if (response.StatusCode != System.Net.HttpStatusCode.OK)
//            //    throw new Exception("getTenantAvailablePaymentSettings error");

//            //var responseBody = await response.Content.ReadAsStringAsync();
//            //var responseObj = JsonConvert.DeserializeObject<IEnumerable<TenantPaymentListResponse>>(responseBody);
//            //return responseObj;

//        }
//    }
//}


