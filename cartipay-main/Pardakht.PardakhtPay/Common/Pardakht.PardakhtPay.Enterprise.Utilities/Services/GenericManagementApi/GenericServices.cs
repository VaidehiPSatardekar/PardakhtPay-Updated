using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.ApiKey;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.GenericManagementApi;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.Tenant;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Tenant;
using Pardakht.PardakhtPay.Enterprise.Utilities.Services.CacheService;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Services.Domain;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Services.Tenant;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Services.GenericManagementApi
{
    public static class GenericServices
    {
        //public static void TenantConfig(this IServiceCollection services, IConfiguration Configuration)
        //{
        //    //services.AddScoped<TenantConfig>();
        //    //services.AddScoped<IDomainManagementService, DomainManagementService>();
        //    services.AddScoped<ITenantResolver, TenantResolver>();
        //    services.AddScoped<ITenantResolverService, TenantResolverService<TenantManagementSettings>>();
        //    //services.RegisterGenericServices<DomainManagementSettings>(Configuration);
        //    services.RegisterGenericServices<TenantManagementSettings>(Configuration);
        //}
        public static void RegisterGenericServices<T>(this IServiceCollection services, IConfiguration Configuration) where T : ApiSettings, new()
        {
            services.AddHttpClient((typeof(T).Name), c =>
            {
                c.BaseAddress = new Uri(Configuration.GetSection(typeof(T).Name)["Url"].ToString());
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.AddTransient<IGenericManagementFunctions<T>, GenericManagementFunctions<T>>();
            services.AddSingleton<GenericManagementAuth<T>>();
            services.AddScoped<GenericManagementTokenGenerator<T>>();
            services.Configure<T>(Configuration.GetSection(typeof(T).Name));
            services.Configure<LoginManagementSettings>(Configuration.GetSection(nameof(LoginManagementSettings)));

        }

        public static void SystemFunctions<T>(this IServiceCollection services, IConfiguration Configuration) where T : ApiSettings, new()
        {
            services.AddHttpClient((typeof(T).Name), c =>
            {
                c.BaseAddress = new Uri(Configuration.GetSection(typeof(T).Name)["Url"].ToString());
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            services.Configure<T>(Configuration.GetSection(typeof(T).Name));
            services.AddTransient<ISystemFunctions<T>, SystemFunctions<T>>();
        }

        public static void CacheService<T>(this IServiceCollection services, IConfiguration Configuration) where T : class, new()
        {
            services.AddScoped<ICacheService, CacheService.CacheService>();
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = Configuration.GetSection(typeof(T).Name)["Url"].ToString();
            });
        }
    }
}


