//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using Pardakht.PardakhtPay.Application.Interfaces;
//using Pardakht.PardakhtPay.Shared.Interfaces;
//using Pardakht.PardakhtPay.Shared.Models.Configuration;
//using Pardakht.PardakhtPay.Shared.Models.WebService;
//using System.Linq;
//using System.Threading;
//using Pardakht.PardakhtPay.Infrastructure.Interfaces;
//using Pardakht.PardakhtPay.Shared.Models.Entities;
//using System.Security.Claims;

//namespace Pardakht.PardakhtPay.Application.Services
//{
//    public class TenantCacheService : ITenantCacheService
//    {
//        ICacheService _CacheService = null;
//        ILogger<TenantCacheService> _Logger = null;
//        const string CacheKey = "Tenants";
//        static SemaphoreSlim _Semaphore = new SemaphoreSlim(1, 1);
//        ITenantApiRepository _TenantCallbackRepository = null;
//        IGenericManagementFunctions<TenantManagementSettings> _TenantManagementFunctions = null;
//        IHttpClientFactory _HttpClientFactory = null;
//        PlatformInformationSettings _PlatformSettings = null;

//        public TenantCacheService(
//            ILogger<TenantCacheService> logger,
//            ITenantApiRepository tenantCallbackRepository,
//            IGenericManagementFunctions<TenantManagementSettings> tenantManagementFunctions,
//            ICacheService cacheService,
//            IHttpClientFactory httpClientFactory,
//            IOptions<PlatformInformationSettings> platformInformationSettingOptions)
//        {
//            _CacheService = cacheService;
//            _TenantCallbackRepository = tenantCallbackRepository;
//            _Logger = logger;
//            _TenantManagementFunctions = tenantManagementFunctions;
//            _HttpClientFactory = httpClientFactory;
//            _PlatformSettings = platformInformationSettingOptions.Value;
//        }

//        public async Task<string> GetTenantConnectionString(string tenantPlatfromGuid)
//        {
//            var cacheItems = await GetCachedTenants();

//            if (cacheItems != null)
//            {
//                var item = cacheItems.FirstOrDefault(t => t.TenantPlatformMapGuid == tenantPlatfromGuid);

//                if (item == null)
//                {
//                    return null;
//                }

//                return item.ConnectionString;
//            }
//            else
//            {
//                return null;
//            }
//        }

//        public async Task<List<TenantSearchDTO>> GetTenants()
//        {
//            var cacheItems = await GetCachedTenants();

//            if (cacheItems != null)
//            {
//                return cacheItems.ConvertAll(t => new TenantSearchDTO()
//                {
//                    //DomainGuid = t.DomainGuid,
//                    //DomainName = t.DomainName,
//                    Id = t.Id,
//                    TenantDomainPlatformMapGuid = t.TenantPlatformMapGuid,
//                    //TenantGuid = t.TenantGuid,
//                    TenantName = t.Tenant.TenancyName,
//                    //TenantStatus = t.TenantStatus
//                });
//            }

//            return null;
//        }

//        public virtual async Task<List<TenantCacheDTO>> LoadTenants()
//        {
//            object obj = new { PlatformGuid = _PlatformSettings.PlatformGuid, TenantGuid = "" };
//            ClaimsPrincipal p = new ClaimsPrincipal();
//            var claim = new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(obj));
//            var identity = new ClaimsIdentity();
//            identity.AddClaim(claim);
//            p.AddIdentity(identity);

//            var response = await _TenantManagementFunctions.MakeRequest(null, p, "/api/TenantPlatform", HttpMethod.Get);

//            if (!response.IsSuccessStatusCode)
//            {
//                var content = await response.Content.ReadAsStringAsync();
//                _Logger.LogError($"Could not get tenant information from the tenant service. Error Code : {response.StatusCode}");
//                return null;
//            }

//            var responseContent = await response.Content.ReadAsStringAsync();

//            var cacheItems = JsonConvert.DeserializeObject<List<TenantCacheDTO>>(responseContent);
//            cacheItems.ForEach(t =>
//            {
//                t.ConnectionString = null;
//            });

//            cacheItems = cacheItems.Where(t => t.PlatformGuid ==  _PlatformSettings.PlatformGuid).ToList();

//            _CacheService.Set(CacheKey, cacheItems, TimeSpan.FromHours(2));

//            return cacheItems;
//        }

//        private async Task<List<TenantSearchDTO>> GetSearchTenants()
//        {
//            try
//            {
//                var client = _HttpClientFactory.CreateClient();
//                //var url = _TenantManagementFunctions.set.Url + "api/TenantDomainPlatformMap/get-by-platformGuid/" + _AuthenticationConfiguration.PlatformId;

//                using (var response = await _TenantManagementFunctions.MakeRequest(null, null, "api/TenantDomainPlatformMap/get-by-platformGuid/", HttpMethod.Get))
//                {
//                    if (!response.IsSuccessStatusCode)
//                    {
//                        var responseContent = await response.Content.ReadAsStringAsync();
//                    }
//                    else
//                    {
//                        var responseText = await response.Content.ReadAsStringAsync();

//                        var items = JsonConvert.DeserializeObject<List<TenantSearchDTO>>(responseText);
//                        return items;
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                _Logger.LogError(ex, ex.Message);
//            }

//            return null;
//        }

//        private async Task<List<TenantCacheDTO>> GetCachedTenants()
//        {
//            var cacheItems = _CacheService.Get<List<TenantCacheDTO>>(CacheKey);

//            if (cacheItems == null)
//            {

//                try
//                {
//                    await _Semaphore.WaitAsync();

//                    cacheItems = _CacheService.Get<List<TenantCacheDTO>>(CacheKey);

//                    if (cacheItems == null)
//                    {
//                        cacheItems = await LoadTenants();
//                    }
//                }
//                finally
//                {
//                    _Semaphore.Release();
//                }
//            }

//            return cacheItems;
//        }

//        public async Task<string> GetCallbackUrl(string tenantPlatformGuid)
//        {
//            try
//            {
//                var callback = _TenantCallbackRepository.GetCallbackUrl(tenantPlatformGuid);

//                if (!string.IsNullOrEmpty(callback))
//                {
//                    return callback;
//                }

//                var items = await GetCachedTenants();

//                if (items == null)
//                {
//                    return null;
//                }

//                var item = items.FirstOrDefault(t => t.TenantPlatformMapGuid == tenantPlatformGuid);

//                if (item == null)
//                {
//                    return null;
//                }
//                else
//                {
//                    return item.PrimaryDomainName;
//                }
//            }
//            catch (Exception ex)
//            {
//                _Logger.LogError(ex, ex.Message);
//                return null;
//            }
//        }

//        public async Task<string> GetTenantGuid(string url)
//        {
//            try
//            {
//                var guid = _TenantCallbackRepository.GetTenantGuid(url);

//                if (!string.IsNullOrEmpty(guid))
//                {
//                    return guid;
//                }

//                var returnUrl = new Uri(url);

//                var tokens = returnUrl.Host.Split(".");

//                string domainName = returnUrl.Host;

//                if (tokens.Length == 3)
//                {
//                    domainName = $"{tokens[1]}.{tokens[2]}";
//                }

//                var items = await GetCachedTenants();

//                var item = items.FirstOrDefault(t => t.DomainNames.Contains(domainName));

//                if (item == null)
//                {
//                    return null;
//                }
//                else
//                {
//                    return item.TenantPlatformMapGuid;
//                }
//            }
//            catch (Exception ex)
//            {
//                _Logger.LogError(ex, ex.Message);
//                return null;
//            }
//        }

//        public async Task<int> GetMerchantIdFromUrl(string url)
//        {
//            try
//            {
//                return await _TenantCallbackRepository.GetMerchantId(url);
//            }
//            catch (Exception ex)
//            {
//                _Logger.LogError(ex, ex.Message);
//                return 0;
//            }
//        }

//        public async Task<TenantApi> GetTenantApi(string url)
//        {
//            return await _TenantCallbackRepository.GetTenantApi(url);
//        }
//    }
//}
