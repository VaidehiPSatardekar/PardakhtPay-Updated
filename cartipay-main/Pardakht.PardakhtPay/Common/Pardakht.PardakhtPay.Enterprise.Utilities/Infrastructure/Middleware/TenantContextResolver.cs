//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.DependencyInjection;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.Helpers;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.Tenant;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Tenant;
//using Serilog;

//namespace Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.Middleware
//{
//    public class TenantContextResolver
//    {
//        RequestDelegate next;

//        public TenantContextResolver(RequestDelegate next)
//        {
//            this.next = next;
//        }

//        public async Task Invoke(HttpContext context)
//        {
//            try
//            {
//                TenantConfig tenantConfig = null;
//                var accountContextHeader = HttpContextHelper.GetCustomHeader(context, "account-context");

//                if (!string.IsNullOrEmpty(accountContextHeader))
//                {
//                    //pattern sample X:Y => X is original tenant guid, y is brandId
//                    var accountContextSplit = accountContextHeader.Split(':');
//                    var accountContext = accountContextSplit[0];

//                    // this is for when account actions are being made on customers by back-end users, so we need to run in the tenant's context
//                    // as all DB interaction is with the tenant's DB
//                    tenantConfig = await ResolveTenant(context, accountContext, TenantSearchType.TenantGuid).ConfigureAwait(false);
//                }
//                else
//                {
//                    var domain = HttpContextHelper.GetDomain(context);
//                    if (!string.IsNullOrEmpty(domain))
//                    {
//                        Log.Information($"Searching for domain {domain}");
//                        // this is where the request is coming from the customer portal, so we need to run in the context of the tenant that owns the domain name
//                        tenantConfig = await ResolveTenant(context, domain, TenantSearchType.Domain).ConfigureAwait(false);
//                    }
//                    else
//                    {
//                        Log.Information("No domain info");
//                    }
//                }
//                if (tenantConfig != null)
//                {
//                    Log.Information($"Using tenant context {tenantConfig.TenantPlatformMapGuid} - {tenantConfig.GetConnectionStringWithoutPassword(tenantConfig.ThisProductConnectionString)}");
//                    SwitchContext(context, tenantConfig);
//                }
//                else
//                {
//                    Log.Information("No tenant context being used for this request");
//                }
//            }
//            catch (Exception ex)
//            {
//                Log.Error($"An error occurred resolving tenant context - {ex}");
//            }

//            await next(context).ConfigureAwait(false);
//        }

//        private async Task<TenantConfig> ResolveTenant(HttpContext context, string searchValue,
//            TenantSearchType searchType)
//        {
//            var resolver = context.RequestServices.GetRequiredService<ITenantResolver>();
//            var accountContextHeader = HttpContextHelper.GetCustomHeader(context, "account-context");

//            switch (searchType)
//            {
//                case TenantSearchType.Domain:
//                    return await resolver.ResolveByDomain(searchValue, accountContextHeader).ConfigureAwait(false);
//                case TenantSearchType.TenantGuid:
//                    return await resolver.ResolveByGuid(searchValue, accountContextHeader).ConfigureAwait(false);
//                default:
//                    throw new ArgumentOutOfRangeException(nameof(searchType), searchType, null);
//            }
//        }

//        private void SwitchContext(HttpContext context, TenantConfig requestTenantConfig)
//        {
//            var injectedTenantConfig = context.RequestServices.GetRequiredService<TenantConfig>();

//            if (requestTenantConfig != null)
//            {
//                injectedTenantConfig.Clone(requestTenantConfig);
//            }
//            else
//            {
//                injectedTenantConfig.Reset();
//            }
//        }

//        private enum TenantSearchType
//        {
//            Domain,
//            TenantGuid
//        }
//    }
//}
