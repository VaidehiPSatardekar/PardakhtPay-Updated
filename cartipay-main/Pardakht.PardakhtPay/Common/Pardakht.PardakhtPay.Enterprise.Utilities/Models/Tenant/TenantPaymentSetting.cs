//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Options;
//using Newtonsoft.Json;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.GenericManagementApi;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Shared;

//namespace Pardakht.PardakhtPay.Enterprise.Utilities.Models.Tenant
//{

//    public class TenantPaymentSetting
//    {
//        private readonly TenantManagementSettings _tenantManagementSettings;
//        private readonly IServiceProvider _serviceProvider;
//        private readonly IMemoryCache _memoryCache;

//        public TenantPaymentSetting(IMemoryCache memoryCache, 
//            IServiceProvider serviceProvider, IOptions<TenantManagementSettings> tenantManagementSettings)
//        {
//            _serviceProvider = serviceProvider;
//            _memoryCache = memoryCache;
//            _tenantManagementSettings = tenantManagementSettings.Value;
//        }

//        public IEnumerable<TenantPaymentListResponse> PaymentSettings
//        {
//            get
//            {
//                var tenantConfig = _serviceProvider.GetRequiredService<TenantConfig>();
//                if (_memoryCache.TryGetValue($"GetTenantPaymentList-{tenantConfig.TenantPlatformMapGuid}", out IEnumerable<TenantPaymentListResponse> cacheOut))
//                {
//                    return cacheOut;
//                }

//                var managementFunctions = _serviceProvider.GetRequiredService<IGenericManagementFunctions<TenantManagementSettings>>();
//                var body =new
//                {
//                    tenantConfig.TenantPlatformMapGuid
//                };

//                var url = $"/api/tenantplatform/getTenantPaymentList";
//                var response = managementFunctions.MakeRequest(body, null, url, HttpMethod.Post).Result;
//                var content = response.Content.ReadAsStringAsync().Result;
//                if (response.StatusCode != System.Net.HttpStatusCode.OK)
//                    throw new Exception($"getTenantAvailablePaymentSettings error {content}");

//                var responseObj = JsonConvert.DeserializeObject<IEnumerable<TenantPaymentListResponse>>(content);
//                var isEnableCache = _tenantManagementSettings.GetTenantPaymentListCacheInMinites.HasValue;
//                if (isEnableCache)
//                {
//                    var cacheEntryOptions = new MemoryCacheEntryOptions()
//                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(_tenantManagementSettings.GetTenantPaymentListCacheInMinites.Value));
//                    _memoryCache.Set($"GetTenantPaymentList-{tenantConfig.TenantPlatformMapGuid}", responseObj, cacheEntryOptions);
//                }

//                return responseObj;
//            }
//        }
//    }

//    public class TenantPaymentListResponse
//    {
//        public int Id { get; set; }
//        public string PaymentIdentifier { get; set; }
//        public string Name { get; set; }
//        public string IconUrl { get; set; }
//        public string PaymentSystemCurrencyCodes { get; set; }
//        public string GatewayKey { get; set; }
//        public IEnumerable<CustomFieldDto> DepositInputParamaters { get; set; }
//        public CustomFieldDto[] CustomFields { get; set; }
//        public IEnumerable<CustomFieldDto> WithDrawInputParamaters { get; set; }
//        public List<int> PaymentSettingCustomerSegmentationRelations { get; set; }
//        public ICollection<PaymentCustomerSegmentRelationDto> PaymentSettingCustomerSegmentationRelationsObject { get; set; }
//        public int TenantPlatformMapPaymentSettingMapId { get; set; }
//        public TenantPaymentSettingStatus PaymentSettingStatus { get; set; }
//        public TenantPaymentSettingType TenantPaymentSettingType { get; set; }
//        public IEnumerable<OrderDefault> OrderDefaults { get; set; }
//        public string PaymentSystemName { get; set; }
//        public string ExtendedConfigurationJson { get; set; }
//        public int? ProxyIpAddressesId { get; set; }
//    }

//    public class OrderDefault
//    {
//        public TenantPaymentSettingType TenantPaymentSettingType { get; set; }
//        public int Priority { get; set; }
//        public bool IsDefault { get; set; }
//    }

//}


