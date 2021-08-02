using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
using Pardakht.PardakhtPay.Shared.Models.Configuration;

namespace Pardakht.PardakhtPay.BankBotWebJobService
{
    public static class ConfigurationExtensions
    {
        public static void SetupConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CacheConfiguration>(configuration.GetSection(nameof(CacheConfiguration)));
            services.Configure<AesEncryptionSettings>(configuration.GetSection(nameof(AesEncryptionSettings)));
            services.Configure<BankBotConfiguration>(configuration.GetSection(nameof(BankBotConfiguration)));
            services.Configure<TransactionConfiguration>(configuration.GetSection(nameof(TransactionConfiguration)));
            services.Configure<LogConfiguration>(configuration.GetSection(nameof(LogConfiguration)));
            services.Configure<QueueConfiguration>(configuration.GetSection(nameof(QueueConfiguration)));
            services.Configure<WithdrawalConfiguration>(configuration.GetSection(nameof(WithdrawalConfiguration)));
            services.Configure<TenantManagementSettings>(configuration.GetSection(nameof(TenantManagementSettings)));
            services.Configure<MobileTransferConfiguration>(configuration.GetSection(nameof(MobileTransferConfiguration)));
            services.Configure<Shared.Models.Configuration.LoginManagementSettings>(configuration.GetSection(nameof(Shared.Models.Configuration.LoginManagementSettings)));
            services.Configure<PardakhtPayAuthenticationSettings>(configuration.GetSection(nameof(PardakhtPayAuthenticationSettings)));
            services.Configure<PlatformInformationSettings>(configuration.GetSection(nameof(PlatformInformationSettings)));
        }
    }
}
