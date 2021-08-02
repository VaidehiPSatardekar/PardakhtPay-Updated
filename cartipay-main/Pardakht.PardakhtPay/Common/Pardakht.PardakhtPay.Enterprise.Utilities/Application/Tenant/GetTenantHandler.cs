//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Tenant;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Services.Tenant;

//namespace Pardakht.PardakhtPay.Enterprise.Utilities.Application.Tenant
//{
//    public class GetTenantHandler<T> where T : ApiSettings, new()
//    {
//        private const string keyCache = "tenants";
//        private readonly Dictionary<string, string> httpHeader;
//        private readonly T settings;
//        private static MemoryCache tenantCache { get; } = new MemoryCache(new MemoryCacheOptions());
//        private const int cacheTimeInMinutes = 30;
//        private readonly PlatformInformationSettings platformInformationSettings;
//        private readonly ILogger<TenantResolverService<T>> logger;
//        private readonly IHttpClientFactory _httpClientFactory;


//        public GetTenantHandler(IHttpClientFactory  httpClientFactory, T settings, PlatformInformationSettings platformInformationSettings, ILogger<TenantResolverService<T>> logger, Dictionary<string,string> httpHeader)
//        {
//            this.settings = settings;
//            this.platformInformationSettings = platformInformationSettings;
//            this.logger = logger;
//            this.httpHeader = httpHeader;
//            _httpClientFactory = httpClientFactory;
//        }

//        public async Task<List<TenantConfig>> Handle()
//        {
//            // get a list of tenants from the sportsbook api and cache
//            if (!tenantCache.TryGetValue(keyCache, out List<TenantConfig> result))
//            {
//                result = await GetTenantsFromApi().ConfigureAwait(false);

//                if (result != null)
//                {
//                    var cacheEntryOptions = new MemoryCacheEntryOptions()
//                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(cacheTimeInMinutes));

//                    var tenantList = new List<TenantConfig>();
//                    foreach (var tenant in result)
//                    {
//                        SetProductConnectionString(tenant);
//                        tenantList.Add(tenant);
//                    }

//                    tenantCache.Set(keyCache, tenantList, cacheEntryOptions);
//                }
//            }

//            return result;
//        }

//        private TenantConfig SetProductConnectionString(TenantConfig tenantConfig)
//        {
//            if (tenantConfig != null && tenantConfig.Products != null && !string.IsNullOrEmpty(platformInformationSettings.ProductCode))
//            {
//                var product = tenantConfig.Products.FirstOrDefault(p => p.Code == platformInformationSettings.ProductCode);
//                if (product != null)
//                {
//                    tenantConfig.ThisProductConnectionString = product.ConnectionString;
//                }
//            }

//            return tenantConfig;
//        }

//        private async Task<List<TenantConfig>> GetTenantsFromApi()
//        {
//            var url = $"{settings.Url}/api/TenantPlatform/lite";

//            try
//            {
//                var client = _httpClientFactory.CreateClient(typeof(T).Name);
//                foreach (var header in httpHeader)
//                {
//                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
//                }
//                var response = await client.GetAsync(url).ConfigureAwait(false);
//                if (response.StatusCode == System.Net.HttpStatusCode.OK)
//                {
//                    var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
//                    if (responseBody.Length > 2)
//                    {
//                        var tenantConfigs = JsonConvert.DeserializeObject<IEnumerable<TenantConfig>>(responseBody).ToList();
//                        foreach (var tenantConfig in tenantConfigs)
//                        {
//                            if (tenantConfig.TenantPlatformMapBrands.Count() != 1) 
//                                continue;

//                            tenantConfig.BrandName =
//                                tenantConfig.TenantPlatformMapBrands.FirstOrDefault()?.BrandName;
//                            tenantConfig.SubDomain =
//                                tenantConfig.TenantPlatformMapBrands.FirstOrDefault()?.SubDomain;
//                        }

//                        return tenantConfigs;
//                    }
//                }

//                var contents = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
//                logger.LogWarning($"TenantService.GetTenants: Problem with Get for {url} with user data {httpHeader["UserData"]},  response code {response.StatusCode}, {contents}");

//                return null;
//            }
//            catch (Exception ex)
//            {
//                logger.LogError($"TenantService.GetTenants: An error occurred getting tenant list from {url} - {ex}");
//                throw;
//            }
//        }

//        public void ClearCache()
//        {
//            tenantCache.Remove(keyCache);
//        }
//    }
//}
