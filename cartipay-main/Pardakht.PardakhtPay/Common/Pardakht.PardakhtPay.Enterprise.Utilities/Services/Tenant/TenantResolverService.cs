//using System.Collections.Generic;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using Newtonsoft.Json;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Application.Tenant;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.ApiKey;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.Tenant;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Tenant;

//namespace Pardakht.PardakhtPay.Enterprise.Utilities.Services.Tenant
//{
//    public class TenantResolverService<T> : ITenantResolverService where T : ApiSettings, new()
//    {
//        private readonly Dictionary<string,string> httpHeader;
//        private readonly T settings;
//        private readonly PlatformInformationSettings platformInformationSettings;
//        private readonly GenericManagementTokenGenerator<T> genericManagementTokenGenerator;
//        private readonly ILogger<TenantResolverService<T>> logger;
//        private readonly IHttpClientFactory _httpClientFactory;

//        public TenantResolverService(   IOptions<PlatformInformationSettings> platformInformationSettings,
//                                        IHttpClientFactory httpClientFactory,
//                                        IOptions<T> settings,
//                                        GenericManagementTokenGenerator<T> genericManagementTokenGenerator,
//                                        ILogger<TenantResolverService<T>> logger)
//        {
//            this.settings = settings.Value;
//            _httpClientFactory = httpClientFactory;
//            this.platformInformationSettings = platformInformationSettings.Value;
//            this.genericManagementTokenGenerator = genericManagementTokenGenerator;
//            this.logger = logger;
//        }

//        public Task<List<TenantConfig>> GetTenants(string accountContext, bool clearCache = false)
//        {
//            var token = genericManagementTokenGenerator.Token;
//            if (string.IsNullOrEmpty(token))
//            {
//                logger.LogError("TenantService: unable to generate authorization token");
//            }

//            var headers  = new Dictionary<string, string>()
//            {
//                {"Authorization", $"Bearer {token}"}
               
//            };
//            if (!string.IsNullOrEmpty(accountContext))
//            {
//                headers.Add("account-context", accountContext);
//            }

//            var httpHeader = headers;

//            if (!string.IsNullOrEmpty(this.platformInformationSettings?.PlatformGuid))
//            {
//                var userData = JsonConvert.SerializeObject(new
//                {
//                    PlatformGuid = this.platformInformationSettings.PlatformGuid,
//                    TenantGuid = string.Empty
//                });
//                httpHeader.Add("UserData", userData);
//            }

//            var handler = new GetTenantHandler<T>(_httpClientFactory, settings, platformInformationSettings, logger, httpHeader);
//            if (clearCache)
//            {
//                handler.ClearCache();
//            }
//            return handler.Handle();
//        }

//        public Task<List<TenantConfig>> GetTenants(bool clearCache = false)
//        {
//            return GetTenants(string.Empty, clearCache);
//        }
//    }
//}
