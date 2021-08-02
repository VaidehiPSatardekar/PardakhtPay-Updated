//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.Extensions.DependencyInjection;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.Helpers;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.Domain;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.Tenant;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Tenant;

//namespace Pardakht.PardakhtPay.Enterprise.Utilities.Services.Tenant
//{
//    public class TenantResolver : ITenantResolver
//    {
//        private readonly ITenantResolverService _tenantResolverService;
//        private readonly IServiceProvider _serviceProvider;

//        public TenantResolver(ITenantResolverService tenantResolverService, IServiceProvider serviceProvider)
//        {
//            _tenantResolverService = tenantResolverService;
//            _serviceProvider = serviceProvider;
//        }

//        public async Task<TenantConfig> ResolveByDomain(string domain, string accountContext)
//        {
//            var tenants = await _tenantResolverService.GetTenants(accountContext).ConfigureAwait(false);
//            var tenant = await ResolveByDomain(domain, tenants);
//            if (tenant != null)
//            {
//                return tenant;
//            }

//            tenants = await _tenantResolverService.GetTenants(accountContext, true).ConfigureAwait(false);
//            tenant = await ResolveByDomain(domain, tenants);
//            return tenant;
//        }
//        public async Task<TenantConfig> ResolveByDomain(string domain, List<TenantConfig> allTenants)
//        {
//            var domainManagementService = _serviceProvider.GetRequiredService<IDomainManagementService>();
//            var allTenantDomains = await domainManagementService.GetAllTenantDomain();
//            var tenantDomain = allTenantDomains.FirstOrDefault(q =>
//            q.DomainAddress != null &&
//            q.DomainAddress.Equals(domain, StringComparison.InvariantCultureIgnoreCase)
//            );

//            if (tenantDomain != null)
//            {
//                var resolvedTenant = allTenants.FirstOrDefault(q => q.TenantPlatformMapGuid.Equals(tenantDomain.TenantPlatformMapGuid, StringComparison.OrdinalIgnoreCase));
//                if (resolvedTenant != null)
//                {
//                    var tenantBrand = resolvedTenant.TenantPlatformMapBrands.FirstOrDefault(q => q.Id == tenantDomain.TenantPlatformMapBrandId);
//                    resolvedTenant.ActiveTenantPlatformMapBrand = tenantBrand;
//                    resolvedTenant.BrandName = tenantBrand?.BrandName;
//                    resolvedTenant.SubDomain = tenantBrand?.SubDomain;
//                    return resolvedTenant;
//                }
//            }

//            var allTenantsBrands = allTenants.SelectMany(q => q.TenantPlatformMapBrands);
//            foreach (var tenantBrand in allTenantsBrands)
//            {
//                var foundTenantDomain = allTenantDomains.FirstOrDefault(q => q.TenantPlatformMapBrandId == tenantBrand.Id);
//                if (foundTenantDomain != null)
//                {
//                    var searchAddress = $"{tenantBrand.SubDomain}.{foundTenantDomain.DomainAddress}";
//                    if (domain.Equals(searchAddress, StringComparison.OrdinalIgnoreCase))
//                    {
//                        var resolvedTenant = allTenants.FirstOrDefault(q => q.Id == tenantBrand.TenantPlatformMapId);
//                        if (resolvedTenant != null)
//                        {
//                            resolvedTenant.ActiveTenantPlatformMapBrand = tenantBrand;
//                            resolvedTenant.BrandName = tenantBrand?.BrandName;
//                            resolvedTenant.SubDomain = tenantBrand?.SubDomain;
//                            return resolvedTenant;
//                        }
//                    }
//                }
//            }




//            var domainName = HttpContextHelper.GetDomainWithoutSubDomain(domain);
//            tenantDomain = allTenantDomains.FirstOrDefault(q =>
//             q.DomainAddress != null && !q.IsProviderDomain &&
//             q.DomainAddress.Equals(domainName, StringComparison.InvariantCultureIgnoreCase)
//             );

//            if (tenantDomain != null)
//            {
//                var resolvedTenant = allTenants.FirstOrDefault(q => q.TenantPlatformMapGuid.Equals(tenantDomain.TenantPlatformMapGuid, StringComparison.OrdinalIgnoreCase));
//                if (resolvedTenant != null)
//                {
//                    var tenantBrand = resolvedTenant.TenantPlatformMapBrands.FirstOrDefault(q => q.Id == tenantDomain.TenantPlatformMapBrandId);
//                    resolvedTenant.ActiveTenantPlatformMapBrand = tenantBrand;
//                    resolvedTenant.BrandName = tenantBrand?.BrandName;
//                    resolvedTenant.SubDomain = tenantBrand?.SubDomain;
//                    return resolvedTenant;
//                }
//            }

//            foreach (var tenantBrand in allTenantsBrands)
//            {
//                var searchAddress = $"{tenantBrand.SubDomain}.{tenantBrand.Domain}";
//                if (domain.StartsWith("affiliate-", StringComparison.OrdinalIgnoreCase))
//                {
//                    searchAddress = $"affiliate-{searchAddress}";
//                }
//                if (domain.StartsWith("survey-", StringComparison.OrdinalIgnoreCase))
//                {
//                    searchAddress = $"survey-{searchAddress}";
//                }

//                if (domain.Equals(searchAddress, StringComparison.OrdinalIgnoreCase))
//                {
//                    var resolvedTenant = allTenants.FirstOrDefault(q => q.Id == tenantBrand.TenantPlatformMapId);
//                    if (resolvedTenant != null)
//                    {
//                        resolvedTenant.ActiveTenantPlatformMapBrand = tenantBrand;
//                        resolvedTenant.BrandName = tenantBrand?.BrandName;
//                        resolvedTenant.SubDomain = tenantBrand?.SubDomain;
//                        return resolvedTenant;
//                    }
//                }

//            }

//            return null;
//        }
//        public async Task<TenantConfig> ResolveByDomain(string domain)
//        {
//            var allTenants = await _tenantResolverService.GetTenants();
//            return await ResolveByDomain(domain, allTenants);
//        }
//        private TenantConfig ResolveDomain(List<TenantConfig> tenants, string accountContext, string guid)
//        {
//            TenantConfig tenant = null;
//            var allTenantBrands = tenants.SelectMany(q => q.TenantPlatformMapBrands);
//            var activeBrand = allTenantBrands.FirstOrDefault(q => q.TenantUid.Equals(accountContext, StringComparison.OrdinalIgnoreCase));
//            if (activeBrand != null)
//            {
//                tenant = tenants.FirstOrDefault(q => q.Id == activeBrand.TenantPlatformMapId);
//            }
//            else
//            {
//                tenant = tenants.FirstOrDefault(q => q.TenantPlatformMapGuid.Equals(guid, StringComparison.OrdinalIgnoreCase));
//                if (tenant != null)
//                {
//                    var hasOnlyOneBrand = tenant.TenantPlatformMapBrands.Count() == 1;
//                    if (hasOnlyOneBrand)
//                    {
//                        activeBrand = tenant.TenantPlatformMapBrands.FirstOrDefault();
//                    }
//                }
//            }
//            if (tenant != null)
//            {
//                tenant.ActiveTenantPlatformMapBrand = activeBrand;
//                tenant.BrandName = activeBrand?.BrandName;
//                tenant.SubDomain = activeBrand?.SubDomain;
//                tenant.AccountContext = accountContext;
//            }
//            return tenant;
//        }

//        public async Task<TenantConfig> ResolveByGuid(string guid, string accountContext)
//        {
//            var tenants = await _tenantResolverService.GetTenants(accountContext).ConfigureAwait(false);
//            var tenant = ResolveDomain(tenants, accountContext, guid);
//            if (tenant == null)
//            {
//                tenants = await _tenantResolverService.GetTenants(accountContext, true).ConfigureAwait(false);
//                tenant = ResolveDomain(tenants, accountContext, guid);
//            }
//            return tenant;
//        }



//        public async Task<TenantConfig> ResolveByGuid(string id)
//        {
//            var allTenants = await _tenantResolverService.GetTenants();
//            var tenant = allTenants.FirstOrDefault(t => t.TenantPlatformMapGuid.Equals(id, StringComparison.OrdinalIgnoreCase));
//            return tenant;
//        }

//    }
//}
