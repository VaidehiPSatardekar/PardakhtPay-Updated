//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Options;
//using Newtonsoft.Json;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.GenericManagementApi;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;

//namespace Pardakht.PardakhtPay.Enterprise.Utilities.Models.Tenant
//{
//    public class TenantBonus
//    {
//        private readonly TenantManagementSettings _tenantManagementSettings;
//        private readonly IServiceProvider _serviceProvider;
//        private readonly IMemoryCache _memoryCache;

//        public TenantBonus(IServiceProvider serviceProvider, IMemoryCache memoryCache, IOptions<TenantManagementSettings> tenantManagementSettings)
//        {
//            _memoryCache = memoryCache;
//            _serviceProvider = serviceProvider;
//            _tenantManagementSettings = tenantManagementSettings.Value;
//        }

//        public IEnumerable<MappedBonus> BonusSettings
//        {
//            get
//            {
//                var tenantConfig = _serviceProvider.GetRequiredService<TenantConfig>();

//                if (_memoryCache.TryGetValue($"GetTenantBonusList-{tenantConfig.Id}", out IEnumerable<MappedBonus> cacheOut))
//                {
//                    return cacheOut;
//                }

//                var body = new
//                {
//                    TenantPlatformMapId = tenantConfig.Id
//                };

//                var managementFunctions = _serviceProvider.GetRequiredService<IGenericManagementFunctions<TenantManagementSettings>>();
//                var url = $"/api/tenantplatform/getTenantBonusList";
//                var response = managementFunctions.MakeRequest(body, null, url, HttpMethod.Post).Result;
//                var content = response.Content.ReadAsStringAsync().Result;

//                if (response.StatusCode != System.Net.HttpStatusCode.OK)
//                    throw new Exception($"getTenantBonusSettings error {content}");

//                var responseObj = JsonConvert.DeserializeObject<IEnumerable<MappedBonus>>(content);

//                var isEnableCache = _tenantManagementSettings.GetTenantBonusListCacheInMinites.HasValue;
//                if (isEnableCache)
//                {
//                    var cacheEntryOptions = new MemoryCacheEntryOptions()
//                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(_tenantManagementSettings.GetTenantBonusListCacheInMinites.Value));
//                    _memoryCache.Set($"GetTenantBonusList-{tenantConfig.Id}", responseObj, cacheEntryOptions);
//                }

//                return responseObj;
//            }
//        }
//    }
//}

