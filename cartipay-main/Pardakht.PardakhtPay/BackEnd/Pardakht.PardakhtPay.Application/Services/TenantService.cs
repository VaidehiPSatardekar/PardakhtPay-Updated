//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Threading.Tasks;
//using Pardakht.PardakhtPay.Application.Interfaces;
//using Pardakht.PardakhtPay.Domain.Interfaces;
//using Pardakht.PardakhtPay.Shared.Models.Configuration;
//using Pardakht.PardakhtPay.Shared.Models.Entities;
//using Pardakht.PardakhtPay.Shared.Models.WebService;
//using System.Linq;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.Tenant;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;

//namespace Pardakht.PardakhtPay.Application.Services
//{
//    public class TenantService : ITenantService
//    {
//        ITenantResolverService _TenantResolverService = null;
//        PlatformInformationSettings _PlatformInformationsettings = null;
//        ILogger<TenantService> _Logger = null;

//        public TenantService(
//            ILogger<TenantService> logger,
//            ITenantResolverService tenantResolverService,
//            IOptions<PlatformInformationSettings> platformInformationsettings)
//        {
//            _TenantResolverService = tenantResolverService;
//            _PlatformInformationsettings = platformInformationsettings.Value;
//            _Logger = logger;
//        }

//        public async Task<WebResponse<List<TenantSearchDTO>>> Search()
//        {
//            try
//            {
//                var items = await _TenantResolverService.GetTenants(false);

//                if (items == null)
//                {
//                    return new WebResponse<List<TenantSearchDTO>>()
//                    {
//                        Success = false,
//                        Message = "No tenants found"
//                    };
//                }
//                else
//                {
//                    var tenants = items
//                                    .Where(t => t.PlatformGuid == _PlatformInformationsettings.PlatformGuid)
//                                    .Select(t => new TenantSearchDTO()
//                                    {
//                                        Id = t.Id,
//                                        TenantDomainPlatformMapGuid = t.TenantPlatformMapGuid,
//                                        TenantGuid = t.TenantPlatformMapGuid,
//                                        TenantName = t.BrandName
//                                    }).ToList();

//                    return new WebResponse<List<TenantSearchDTO>>(tenants);
//                }
//            }
//            catch (Exception ex)
//            {
//                _Logger.LogError(ex, ex.Message);
//                return new WebResponse<List<TenantSearchDTO>>(ex);
//            }
//        }
//    }
//}
