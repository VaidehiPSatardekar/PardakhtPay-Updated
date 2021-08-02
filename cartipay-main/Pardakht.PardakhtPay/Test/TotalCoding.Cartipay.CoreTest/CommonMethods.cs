using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using TotalCoding.Cartipay.Application.Interfaces;
using TotalCoding.Cartipay.Application.Mock;
using TotalCoding.Cartipay.Application.Services;
using TotalCoding.Cartipay.Domain.Dashboard;
using TotalCoding.Cartipay.Domain.Dashboard.Builders;
using TotalCoding.Cartipay.Domain.Dashboard.Interfaces;
using TotalCoding.Cartipay.Domain.Interfaces;
using TotalCoding.Cartipay.Domain.Managers;
using TotalCoding.Cartipay.ExternalServices.Queue;
using TotalCoding.Cartipay.Infrastructure.Interfaces;
using TotalCoding.Cartipay.Shared.Interfaces;
using TotalCoding.Cartipay.Shared.Models.Configuration;
using TotalCoding.Cartipay.Shared.Models.WebService;
using TotalCoding.Cartipay.Shared.Services;
using TotalCoding.Cartipay.SqlRepository;
using TotalCoding.Cartipay.SqlRepository.Repositories;
using TotalCoding.Cartipay.SqlRepository.SeedData;

namespace TotalCoding.Cartipay.CoreTest
{
    public static class CommonMethods
    {
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
        }


        public static void AddDependencyInjections(this IServiceCollection services)
        {
            #region Merchant
            services.AddScoped(typeof(IMerchantManager), typeof(MerchantManager));
            services.AddScoped(typeof(IMerchantService), typeof(MerchantService));
            services.AddScoped(typeof(IMerchantRepository), typeof(MerchantRepository));
            #endregion

            #region Merchant Bank Account

            //services.AddScoped(typeof(IMerchantBankAccountManager), typeof(MerchantBankAccountManager));
            //services.AddScoped(typeof(IMerchantBankAccountService), typeof(MerchantBankAccountService));
            //services.AddScoped(typeof(IMerchantBankAccountRepository), typeof(MerchantBankAccountRepository));

            #endregion

            #region Merchant Group
            services.AddScoped(typeof(ITenantManager), typeof(TenantManager));
            services.AddScoped(typeof(ITenantRepository), typeof(TenantRepository));
            services.AddScoped(typeof(ITenantService), typeof(TenantService));
            #endregion

            #region Language

            services.AddScoped(typeof(ILanguageManager), typeof(LanguageManager));
            services.AddScoped(typeof(ILanguageService), typeof(LanguageService));
            services.AddScoped(typeof(ILanguageRepository), typeof(LanguageRepository));
            #endregion

            #region Currency

            services.AddScoped(typeof(ICurrencyManager), typeof(CurrencyManager));
            services.AddScoped(typeof(ICurrencyService), typeof(CurrencyService));
            services.AddScoped(typeof(ICurrencyRepository), typeof(CurrencyRepository));
            #endregion

            #region Transaction

            services.AddScoped(typeof(ITransactionManager), typeof(TransactionManager));
            services.AddScoped(typeof(ITransactionService), typeof(TransactionService));
            services.AddScoped(typeof(ITransactionRepository), typeof(TransactionRepository));
            #endregion

            #region Transaction Query History

            services.AddScoped(typeof(ITransactionQueryHistoryManager), typeof(TransactionQueryHistoryManager));
            services.AddScoped(typeof(ITransactionQueryHistoryService), typeof(TransactionQueryHistoryService));
            services.AddScoped(typeof(ITransactionQueryHistoryRepository), typeof(TransactionQueryHistoryRepository));
            #endregion

            #region Dashboard

            services.AddScoped(typeof(IDashboardManager), typeof(DashboardManager));
            services.AddScoped(typeof(IDashboardService), typeof(DashboardService));
            services.AddScoped(typeof(IDashboardWidgetFactory), typeof(DashboardWidgetFactory));

            services.AddTransient(typeof(ITransactionReportChartWidgetBuilder), typeof(TransactionReportChartWidgetBuilder));
            services.AddTransient(typeof(ITransactionReportWidgetBuilder), typeof(TransactionReportWidgetBuilder));
            services.AddTransient(typeof(IMerchantTransactionReportWidgetBuilder), typeof(MerchantTransactionReportWidgetBuilder));
            services.AddTransient(typeof(IDepositBreakDownReportChartWidgetBuilder), typeof(DepositBreakDownReportChartWidgetBuilder));
            services.AddTransient(typeof(ITransactionWithdrawalReportWidgetBuilder), typeof(TransactionWithdrawalReportWidgetBuilder));

            #endregion

            #region Withdraw

            services.AddScoped(typeof(IWithdrawalManager), typeof(WithdrawalManager));
            services.AddScoped(typeof(IWithdrawalRepository), typeof(WithdrawalRepository));
            services.AddScoped(typeof(IWithdrawalService), typeof(WithdrawalService));
            #endregion

            #region Withdrawal Query History

            services.AddScoped(typeof(IWithdrawalQueryHistoryManager), typeof(WithdrawalQueryHistoryManager));
            services.AddScoped(typeof(IWithdrawalQueryHistoryRepository), typeof(WithdrawalQueryHistoryRepository));
            #endregion

            #region Card To Card Account

            services.AddScoped(typeof(ICardToCardAccountManager), typeof(CardToCardAccountManager));
            services.AddScoped(typeof(ICardToCardAccountRepository), typeof(CardToCardAccountRepository));
            services.AddScoped(typeof(ICardToCardAccountService), typeof(CardToCardAccountService));
            #endregion

            #region Payment Gateway

            services.AddScoped(typeof(IPaymentGatewayManager), typeof(PaymentGatewayManager));
            services.AddScoped(typeof(IPaymentGatewayService), typeof(PaymentGatewayService));
            services.AddScoped(typeof(IPaymentGatewayRepository), typeof(PaymentGatewayRepository));

            #endregion

            #region Payment Gateway Configuration

            services.AddScoped(typeof(IPaymentGatewayConfigurationManager), typeof(PaymentGatewayConfigurationManager));
            services.AddScoped(typeof(IPaymentGatewayConfigurationService), typeof(PaymentGatewayConfigurationService));
            services.AddScoped(typeof(IPaymentGatewayConfigurationRepository), typeof(PaymentGatewayConfigurationRepository));
            #endregion

            #region Transfer Account
            services.AddScoped(typeof(ITransferAccountManager), typeof(TransferAccountManager));
            services.AddScoped(typeof(ITransferAccountRepository), typeof(TransferAccountRepository));
            services.AddScoped(typeof(ITransferAccountService), typeof(TransferAccountService));
            #endregion

            services.AddSingleton(typeof(IReflectionService), typeof(ReflectionService));
            services.AddSingleton(typeof(ISha256EncryptionService), typeof(Sha256EncryptionService));
            services.AddScoped(typeof(IAesEncryptionService), typeof(AesEncryptionService));

            var bankBotConfiguration = services.BuildServiceProvider().GetRequiredService<IOptions<BankBotConfiguration>>();
            if (bankBotConfiguration.Value.Mock)
            {
                services.AddScoped(typeof(IBankBotService), typeof(BankBotMockService));
            }
            else
            {
                services.AddScoped(typeof(IBankBotService), typeof(BankBotService));
            }
            services.AddTransient(typeof(ITransactionQueueService), typeof(TransactionAzureQueueService));

            services.AddTransient<IConnectionStringManager, ConnectionStringManager>();
            services.AddTransient(typeof(SeedDataInvoker));
            services.AddScoped(typeof(CurrentUser));

        }

        public static void AddCacheService(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IOptions<CacheConfiguration>>().Value;

            switch (configuration.CacheType)
            {
                case CacheTypes.Memory:
                    services.AddSingleton(typeof(ICacheService), typeof(MemoryCacheService));
                    break;
                case CacheTypes.Redis:
                default:
                    throw new NotImplementedException();
            }
        }

        public static void AddContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = "Server=" + configuration.GetSection("ConnectionString")["Server"].ToString() +
                                      ";User=" + configuration.GetSection("ConnectionString")["User"].ToString() +
                                      ";Password=" + configuration.GetSection("ConnectionString")["Password"].ToString() +
                                      ";Database=" + configuration.GetSection("ConnectionString")["Database"].ToString();

            connectionString += ";MultipleActiveResultSets = true";

            services.AddDbContext<PardakhtPayDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        }

        public static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                              .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true, reloadOnChange: true)
                .Build();
            return config;
        }
    }
}
