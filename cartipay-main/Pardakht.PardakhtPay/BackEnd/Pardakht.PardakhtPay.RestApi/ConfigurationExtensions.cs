using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
using Pardakht.PardakhtPay.Shared.Models.Configuration;

namespace Pardakht.PardakhtPay.RestApi
{
    /// <summary>
    /// Manages configurations
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Configures settings
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Configuration instance</param>
        public static void SetupConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CacheConfiguration>(configuration.GetSection(nameof(CacheConfiguration)));
            services.Configure<AesEncryptionSettings>(configuration.GetSection(nameof(AesEncryptionSettings)));
            services.Configure<BankBotConfiguration>(configuration.GetSection(nameof(BankBotConfiguration)));
            services.Configure<TransactionConfiguration>(configuration.GetSection(nameof(TransactionConfiguration)));
            services.Configure<LogConfiguration>(configuration.GetSection(nameof(LogConfiguration)));
            services.Configure<QueueConfiguration>(configuration.GetSection(nameof(QueueConfiguration)));
            services.Configure<JwtIssuerOptions>(configuration.GetSection(nameof(JwtIssuerOptions)));
            services.Configure<SeedDataSettings>(configuration.GetSection(nameof(SeedDataSettings)));
            services.Configure<WithdrawalConfiguration>(configuration.GetSection(nameof(WithdrawalConfiguration)));
            //services.Configure<TimeZoneConfiguration>(configuration.GetSection(nameof(TimeZoneConfiguration)));
            services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
            services.Configure<SmsServiceConfiguration>(configuration.GetSection(nameof(SmsServiceConfiguration)));
            services.Configure<UserManagementSettings>(configuration.GetSection(nameof(UserManagementSettings)));
            services.Configure<MobileTransferConfiguration>(configuration.GetSection(nameof(MobileTransferConfiguration)));
            services.Configure<Enterprise.Utilities.Models.Settings.LoginManagementSettings>(configuration.GetSection(nameof(Enterprise.Utilities.Models.Settings.LoginManagementSettings)));
            services.Configure<PlatformInformationSettings>(configuration.GetSection(nameof(PlatformInformationSettings)));
            //services.AddScoped<IGenericManagementFunctions<PaymentManagementSettings>, GenericManagementFunctions<PaymentManagementSettings>>();
            //services.AddSingleton<GenericManagementAuth<PaymentManagementSettings>>();
            //services.AddTransient<GenericManagementTokenGenerator<PaymentManagementSettings>>();
            //services.Configure<PaymentManagementSettings>(configuration.GetSection(nameof(PaymentManagementSettings)));
            //services.Configure<TenantManagementSettings>(configuration.GetSection(nameof(TenantManagementSettings)));
            services.Configure<MobileTransferConfiguration>(configuration.GetSection(nameof(MobileTransferConfiguration)));
            services.Configure<ProxyPaymentApiSettings>(configuration.GetSection(nameof(ProxyPaymentApiSettings)));
            services.Configure<QRRegisterConfiguration>(configuration.GetSection(nameof(QRRegisterConfiguration)));
        }
    }
}