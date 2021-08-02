//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Caching.Memory;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Options;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.Domain;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.GenericManagementApi;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Domain;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;

//namespace Pardakht.PardakhtPay.Enterprise.Utilities.Services.Domain
//{
//    public class DomainManagementService : IDomainManagementService
//    {
//        private readonly IServiceProvider _serviceProvider;
//        private readonly IMemoryCache _memoryCache;
//        public DomainManagementService(IMemoryCache memoryCache, IServiceProvider serviceProvider)
//        {
//            _serviceProvider = serviceProvider;
//            _memoryCache = memoryCache;
//        }
//        public async Task<IEnumerable<DomainResponse>> GetAllTenantDomain()
//        {
//            if (_memoryCache.TryGetValue("GetAllTenantDomain", out IEnumerable<DomainResponse> outCache))
//            {
//                return outCache;
//            }

//            var managementFunctions = _serviceProvider.GetRequiredService<IGenericManagementFunctions<DomainManagementSettings>>();
//            var httpResponseMessage = await managementFunctions.MakeRequest(null, null, $"/api/Domain/all-tenant-domains", System.Net.Http.HttpMethod.Get);
//            var content = await httpResponseMessage.Content.ReadAsStringAsync();
//            var objectContent = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<DomainResponse>>(content);

//            var options = _serviceProvider.GetRequiredService<IOptions<DomainManagementSettings>>().Value;
//            var isEnableCache = options.CacheTimeInMinutes.HasValue;
//            if (isEnableCache)
//            {
//                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(options.CacheTimeInMinutes.Value));
//                _memoryCache.Set("GetAllTenantDomain", objectContent, cacheEntryOptions);
//            }

//            return objectContent;
//        }
//    }
//}
