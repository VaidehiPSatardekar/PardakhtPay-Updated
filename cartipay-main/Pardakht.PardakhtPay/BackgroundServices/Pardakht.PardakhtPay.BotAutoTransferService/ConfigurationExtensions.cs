using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
using Pardakht.PardakhtPay.Shared.Models.Configuration;

namespace Pardakht.PardakhtPay.BotAutoTransferService
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
            services.Configure<AutoTransferSettings>(configuration.GetSection(nameof(AutoTransferSettings)));
            services.Configure<TenantManagementSettings>(configuration.GetSection(nameof(TenantManagementSettings)));
            services.Configure<ManualTransferConfiguration>(configuration.GetSection(nameof(ManualTransferConfiguration)));
            services.Configure<MobileTransferConfiguration>(configuration.GetSection(nameof(MobileTransferConfiguration)));
            services.Configure<PausedAccountConfiguration>(configuration.GetSection(nameof(PausedAccountConfiguration)));
            services.Configure<CardHolderNameConfiguration>(configuration.GetSection(nameof(CardHolderNameConfiguration)));
            services.Configure<InvoiceConfiguration>(configuration.GetSection(nameof(InvoiceConfiguration)));
            services.Configure<MobileTransferConfiguration>(configuration.GetSection(nameof(MobileTransferConfiguration)));
            services.Configure<Shared.Models.Configuration.LoginManagementSettings>(configuration.GetSection(nameof(Shared.Models.Configuration.LoginManagementSettings)));
            services.Configure<PardakhtPayAuthenticationSettings>(configuration.GetSection(nameof(PardakhtPayAuthenticationSettings)));
            services.Configure<PlatformInformationSettings>(configuration.GetSection(nameof(PlatformInformationSettings)));
            services.Configure<QRRegisterConfiguration>(configuration.GetSection(nameof(QRRegisterConfiguration)));
            services.Configure<LoginDeviceConfiguration>(configuration.GetSection(nameof(LoginDeviceConfiguration)));

            services.Configure<ProxyPaymentApiSettings>(configuration.GetSection(nameof(ProxyPaymentApiSettings)));
        }
    }
}
