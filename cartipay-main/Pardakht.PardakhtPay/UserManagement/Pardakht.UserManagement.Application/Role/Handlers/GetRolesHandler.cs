using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
using Pardakht.UserManagement.Application.TenantServiceApiAuth;
using Pardakht.UserManagement.Domain.Role;
using Pardakht.UserManagement.Shared.Models.Configuration;
using Pardakht.UserManagement.Shared.Models.Infrastructure;
using Pardakht.UserManagement.Shared.Models.WebService;

namespace Pardakht.UserManagement.Application.Role.Handlers
{
    public class GetRolesHandler
    {
        private readonly IRoleManager roleManager;
        private readonly UserContext userContext;
        private readonly ILogger logger;
        private readonly TenantManagementSettings _tenantManagementSettings;
        private readonly TenantServiceApiTokenGenerator _tenantServiceApiTokenGenerator;
        private readonly IHttpClientFactory _httpClientFatory;

        public GetRolesHandler(IRoleManager roleManager, UserContext userContext, ILogger logger,
                                    TenantManagementSettings tenantManagementSettings,
                                    TenantServiceApiTokenGenerator tenantServiceApiTokenGenerator,
                                    IHttpClientFactory httpClientFactory)
        {
            this.roleManager = roleManager;
            this.userContext = userContext;
            this.logger = logger;
            _tenantManagementSettings = tenantManagementSettings;
            _tenantServiceApiTokenGenerator = tenantServiceApiTokenGenerator;
            _httpClientFatory = httpClientFactory;
        }

        public async Task<WebResponse<IEnumerable<RoleDto>>> Handle(string platformGuid, string tenantGuid)
        {
            try
            {
                //if (!userContext.HasRole(PermissionConstants.RoleAccess))
                //{
                //    return new WebResponse<IEnumerable<RoleDto>>
                //    {
                //        Success = false,
                //        Message = "User is not authorised for this task"
                //    };
                //}

                var allRoles = new List<RoleDto>();
                var result = new List<RoleDto>();

                // get all roles for this platform and then filter out any not applicable - possibly slightly inefficient, though the numbers should never be enough to cause any issues
                allRoles.AddRange(await roleManager.GetRoles(platformGuid));

                if (!userContext.IsProviderUser())
                {
                    // a tenant can never see provider roles
                    // add all fixed tenant roles
                    result.AddRange(allRoles.Where(r => r.RoleHolderTypeId == RoleConstants.RoleHolderTypeTenant && r.IsFixed));
                    // add any tenant specific roles for their own tenancy
                    result.AddRange(allRoles.Where(r => r.RoleHolderTypeId == RoleConstants.RoleHolderTypeTenant && r.TenantGuid == userContext.TenantGuid));
                }
                else
                {
                    //var tenants = await GetTenantPlatformMaps();
                    // all provider roles
                    result.AddRange(allRoles.Where(r => r.RoleHolderTypeId == RoleConstants.RoleHolderTypeProvider));
                    // add all tenant roles
                    result.AddRange(allRoles.Where(r => r.RoleHolderTypeId == RoleConstants.RoleHolderTypeTenant));

                    //result.Where(r => !string.IsNullOrEmpty(r.TenantGuid)).ToList().ForEach(x => {
                    //    x.TenancyName = tenants.FirstOrDefault(t => t.TenantPlatformMapGuid == x.TenantGuid)?.BrandName;
                    //});

                    if (!string.IsNullOrEmpty(tenantGuid))
                    {
                        // add any tenant specific roles
                        result.AddRange(allRoles.Where(r => r.RoleHolderTypeId == RoleConstants.RoleHolderTypeTenant && r.TenantGuid == tenantGuid));
                    }
                }

                result = result
                    .OrderByDescending(r => r.IsFixed)
                    .ThenBy(r => r.Name)
                    .ToList();

                return new WebResponse<IEnumerable<RoleDto>>
                {
                    Success = true,
                    Payload = result
                };
            }
            catch (Exception ex)
            {
                logger.LogError($"GetRolesHandler.Handle: an error occurred getting roles for platform {platformGuid}, tenant {tenantGuid} - {ex}");
                throw;
            }
        }

        private async Task<List<TenantPlatformMap>> GetTenantPlatformMaps()
        {
            //todo
            return new List<TenantPlatformMap>()
            {
                new TenantPlatformMap
                {
                    BrandName = "PardakhtPay",
                    PlatformGuid = "PardakhtPayGuid",
                    PreferenceConfig = string.Empty,
                    PrimaryDomainName = string.Empty,
                    TenancyName = "PardakhtPay",
                    Tenant = new TenantDto()
                    {
                        TenancyName = "PardakhtPay",
                        BusinessName = "PardakhtPay",
                        Email = "PardakhtPay@PardakhtPay"
                    },
                    TenantPlatformMapGuid = "PardakhtPayGuid"
                }
            };
            //var url = $"{_tenantManagementSettings.Url}/api/TenantPlatform/tenant-select-list";

            //try
            //{
            //    var token = await _tenantServiceApiTokenGenerator.Token();

            //    if (string.IsNullOrEmpty(token))
            //    {
            //        logger.LogError("StaffUserHandlerBase: unable to generate authorization token");
            //    }

            //    var client = _httpClientFatory.CreateClient();
            //    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            //    var response = await client.GetAsync(url);
            //    if (response.StatusCode == System.Net.HttpStatusCode.OK)
            //    {
            //        var responseBody = await response.Content.ReadAsStringAsync();
            //        if (responseBody.Length > 2)
            //        {
            //            return JsonConvert.DeserializeObject<IEnumerable<TenantPlatformMap>>(responseBody).ToList();
            //        }
            //    }

            //    var contents = await response.Content.ReadAsStringAsync();
            //    logger.LogWarning($"StaffUserHandlerBase.GetTenantsFromApi: Problem with getting tenants for {url}  response code {response.StatusCode} - {contents}");

            //    return null;
            //}
            //catch (Exception ex)
            //{
            //    logger.LogError($"GetTenantHandler.GetTenantsFromApi: An error occurred getting tenant list from {url} - {ex}");
            //    throw;
            //}
        }
    }
}
