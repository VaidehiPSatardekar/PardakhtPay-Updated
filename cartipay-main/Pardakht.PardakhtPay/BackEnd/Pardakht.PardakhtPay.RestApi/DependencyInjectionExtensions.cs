using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Application.Mock;
using Pardakht.PardakhtPay.Application.Services;
using Pardakht.PardakhtPay.Domain.Dashboard;
using Pardakht.PardakhtPay.Domain.Dashboard.Builders;
using Pardakht.PardakhtPay.Domain.Dashboard.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Domain.Managers;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.User;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.User;
using Pardakht.PardakhtPay.Enterprise.Utilities.Services.GenericManagementApi;
using Pardakht.PardakhtPay.Enterprise.Utilities.Services.User;
using Pardakht.PardakhtPay.ExternalServices.Queue;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Services;
using Pardakht.PardakhtPay.SqlRepository;
using Pardakht.PardakhtPay.SqlRepository.Repositories;
using Pardakht.PardakhtPay.SqlRepository.SeedData;

namespace Pardakht.PardakhtPay.RestApi
{
    public static class DependencyInjectionExtensions
    {
        public static void AddDependencyInjections(this IServiceCollection services, IConfiguration configuration)
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

            services.AddTransient(typeof(IAccountingReportChartWidgetBuilder), typeof(AccountingReportChartWidgetBuilder));

            services.AddTransient(typeof(IAccountStatusWidgetBuilder), typeof(AccountStatusWidgetBuilder));
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

            #region Transfer Account
            services.AddScoped(typeof(ITransferAccountManager), typeof(TransferAccountManager));
            services.AddScoped(typeof(ITransferAccountRepository), typeof(TransferAccountRepository));
            services.AddScoped(typeof(ITransferAccountService), typeof(TransferAccountService));
            #endregion

            #region Time Zone
            services.AddScoped(typeof(ITimeZoneService), typeof(TimeZoneService));
            #endregion

            #region Owner Bank Login
            services.AddScoped(typeof(IOwnerBankLoginManager), typeof(OwnerBankLoginManager));
            services.AddScoped(typeof(IOwnerBankLoginRepository), typeof(OwnerBankLoginRepository));
            services.AddScoped(typeof(IOwnerBankLoginService), typeof(OwnerBankLoginService));
            #endregion

            #region Bank Statement Item

            services.AddScoped(typeof(IBankStatementItemManager), typeof(BankStatementItemManager));
            services.AddScoped(typeof(IBankStatementItemRepository), typeof(BankStatementItemRepository));
            services.AddScoped(typeof(IBankStatementItemService), typeof(BankStatementItemService));
            #endregion

            #region Auto Transfer
            services.AddScoped(typeof(IAutoTransferManager), typeof(AutoTransferManager));
            services.AddScoped(typeof(IAutoTransferRepository), typeof(AutoTransferRepository));
            services.AddScoped(typeof(IAutoTransferService), typeof(AutoTransferService));
            #endregion

            #region Merchant Customer

            services.AddScoped(typeof(IMerchantCustomerManager), typeof(MerchantCustomerManager));
            services.AddScoped(typeof(IMerchantCustomerRepository), typeof(MerchantCustomerRepository));
            services.AddScoped(typeof(IMerchantCustomerService), typeof(MerchantCustomerService));

            #endregion

            #region Merchant Card To Card Account Relation

            services.AddScoped(typeof(ICardToCardAccountGroupItemManager), typeof(CardToCardAccountGroupItemManager));
            services.AddScoped(typeof(ICardToCardAccountGroupItemRepository), typeof(CardToCardAccountGroupItemRepository));

            #endregion

            #region Card To Card Account Group

            services.AddScoped(typeof(ICardToCardAccountGroupManager), typeof(CardToCardAccountGroupManager));
            services.AddScoped(typeof(ICardToCardAccountGroupRepository), typeof(CardToCardAccountGroupRepository));
            services.AddScoped(typeof(ICardToCardAccountGroupService), typeof(CardToCardAccountGroupService));

            #endregion

            #region User Segment Group

            services.AddScoped(typeof(IUserSegmentGroupManager), typeof(UserSegmentGroupManager));
            services.AddScoped(typeof(IUserSegmentGroupRepository), typeof(UserSegmentGroupRepository));
            services.AddScoped(typeof(IUserSegmentGroupService), typeof(UserSegmentGroupService));

            #endregion

            #region User Segment

            services.AddScoped(typeof(IUserSegmentManager), typeof(UserSegmentManager));
            services.AddScoped(typeof(IUserSegmentRepository), typeof(UserSegmentRepository));
            services.AddScoped(typeof(IUserSegmentService), typeof(UserSegmentService));

            #endregion

            #region Card To Card User Segment Relation

            services.AddScoped(typeof(ICardToCardUserSegmentRelationRepository), typeof(CardToCardUserSegmentRelationRepository));

            #endregion

            #region Application Settings

            services.AddScoped(typeof(IApplicationSettingManager), typeof(ApplicationSettingManager));
            services.AddScoped(typeof(IApplicationSettingRepository), typeof(ApplicationSettingRepository));
            services.AddScoped(typeof(IApplicationSettingService), typeof(ApplicationSettingService));
            #endregion

            #region Risky Keywords
            services.AddScoped(typeof(IRiskyKeywordManager), typeof(RiskyKeywordManager));
            services.AddScoped(typeof(IRiskyKeywordRepository), typeof(RiskyKeywordRepository));
            services.AddScoped(typeof(IRiskyKeywordService), typeof(RiskyKeywordService));

            #endregion

            #region Device Merchant Customer Relation
            services.AddScoped(typeof(IDeviceMerchantCustomerRelationRepository), typeof(DeviceMerchantCustomerRelationRepository));
            #endregion

            #region Manual Transfers

            services.AddScoped(typeof(IManualTransferRepository), typeof(ManualTransferRepository));
            services.AddScoped(typeof(IManualTransferSourceCardDetailsRepository), typeof(ManualTransferSourceCardDetailsRepository));

            #endregion

            #region Manual Transfer Details

            services.AddScoped(typeof(IManualTransferDetailRepository), typeof(ManualTransferDetailRepository));

            #endregion

            #region Mobile Transfer Card Device

            services.AddScoped(typeof(IMobileTransferDeviceManager), typeof(MobileTransferDeviceManager));
            services.AddScoped(typeof(IMobileTransferDeviceRepository), typeof(MobileTransferDeviceRepository));
            services.AddScoped(typeof(IMobileTransferDeviceService), typeof(MobileTransferDeviceService));

            #endregion

            #region Mobile Transfer Card Account

            services.AddScoped(typeof(IMobileTransferCardAccountManager), typeof(MobileTransferCardAccountManager));
            services.AddScoped(typeof(IMobileTransferCardAccountRepository), typeof(MobileTransferCardAccountRepository));
            services.AddScoped(typeof(IMobileTransferCardAccountService), typeof(MobileTransferCardAccountService));

            #endregion

            #region Mobile Transfer Card Account Group

            services.AddScoped(typeof(IMobileTransferCardAccountGroupManager), typeof(MobileTransferCardAccountGroupManager));
            services.AddScoped(typeof(IMobileTransferCardAccountGroupRepository), typeof(MobileTransferCardAccountGroupRepository));
            services.AddScoped(typeof(IMobileTransferCardAccountGroupService), typeof(MobileTransferCardAccountGroupService));

            #endregion

            #region Mobile Transfer Card Account Group Item

            services.AddScoped(typeof(IMobileTransferCardAccountGroupItemManager), typeof(MobileTransferCardAccountGroupItemManager));
            services.AddScoped(typeof(IMobileTransferCardAccountGroupItemRepository), typeof(MobileTransferCardAccountGroupItemRepository));
            services.AddScoped(typeof(IMobileTransferCardAccountGroupItemService), typeof(MobileTransferCardAccountGroupItemService));

            #endregion

            #region Mobile Transfer Card Account User Segment Relation

            services.AddScoped(typeof(IMobileCardAccountUserSegmentRelationRepository), typeof(MobileCardAccountUserSegmentRelationRepository));

            #endregion

            #region Transaction Withdrawal Relation

            services.AddScoped(typeof(ITransactionWithdrawalRelationManager), typeof(TransactionWithdrawalRelationManager));
            services.AddScoped(typeof(ITransactionWithdrawalRelationRepository), typeof(TransactionWithdrawalRelationRepository));

            #endregion

            #region Transaction Withdrawal History

            services.AddScoped(typeof(ITransactionWithdrawalHistoryManager), typeof(TransactionWithdrawalHistoryManager));
            services.AddScoped(typeof(ITransactionWithdrawalHistoryRepository), typeof(TransactionWithdrawalHistoryRepository));

            #endregion

            #region Blocked Phone Numbers

            services.AddScoped(typeof(IBlockedPhoneNumberManager), typeof(BlockedPhoneNumberManager));
            services.AddScoped(typeof(IBlockedPhoneNumberRepository), typeof(BlockedPhoneNumberRepository));
            services.AddScoped(typeof(IBlockedPhoneNumberService), typeof(BlockedPhoneNumberService));
            #endregion

            #region Withdrawal Transfer History

            services.AddScoped(typeof(IWithdrawalTransferHistoryManager), typeof(WithdrawalTransferHistoryManager));
            services.AddScoped(typeof(IWithdrawalTransferHistoryRepository), typeof(WithdrawalTransferHistoryRepository));
            #endregion

            #region Card Holder Name

            services.AddScoped(typeof(ICardHolderNameManager), typeof(CardHolderNameManager));
            services.AddScoped(typeof(ICardHolderNameRepository), typeof(CardHolderNameRepository));
            services.AddScoped(typeof(ICardHolderNameService), typeof(CardHolderNameService));

            #endregion

            #region Transfer request

            services.AddScoped(typeof(ITransferRequestService), typeof(TransferRequestService));

            #endregion

            #region Reports

            services.AddScoped(typeof(IReportManager), typeof(ReportManager));
            services.AddScoped(typeof(IReportService), typeof(ReportService));

            #endregion

            #region Sekeh Device

            services.AddScoped(typeof(ISekehDeviceManager), typeof(SekehDeviceManager));
            services.AddScoped(typeof(ISekehDeviceRepository), typeof(SekehDeviceRepository));
            #endregion

            #region Unsupported Bin

            services.AddScoped(typeof(IUnsupportedBinRepository), typeof(UnsupportedBinRepository));

            #endregion

            #region Invoice

            services.AddScoped(typeof(IInvoiceManager), typeof(InvoiceManager));
            services.AddScoped(typeof(IInvoiceRepository), typeof(InvoiceRepository));
            services.AddScoped(typeof(IInvoiceService), typeof(InvoiceService));
            #endregion

            #region Invoice Detail

            services.AddScoped(typeof(IInvoiceDetailManager), typeof(InvoiceDetailManager));
            services.AddScoped(typeof(IInvoiceDetailRepository), typeof(InvoiceDetailRepository));
            #endregion

            #region Invoice Owner Setting

            services.AddScoped(typeof(IInvoiceOwnerSettingManager), typeof(InvoiceOwnerSettingManager));
            services.AddScoped(typeof(IInvoiceOwnerSettingRepository), typeof(InvoiceOwnerSettingRepository));
            services.AddScoped(typeof(IInvoiceOwnerSettingService), typeof(InvoiceOwnerSettingService));
            #endregion

            #region Invoice Payment

            services.AddScoped(typeof(IInvoicePaymentManager), typeof(InvoicePaymentManager));
            services.AddScoped(typeof(IInvoicePaymentRepository), typeof(InvoicePaymentRepository));
            services.AddScoped(typeof(IInvoicePaymentService), typeof(InvoicePaymentService));

            #endregion

            #region Ses Device
            services.AddScoped(typeof(ISesDeviceManager), typeof(SesDeviceManager));
            services.AddScoped(typeof(ISesDeviceRepository), typeof(SesDeviceRepository));
            #endregion

            #region Transaction Request Content
            services.AddScoped(typeof(ITransactionRequestContentRepository), typeof(TransactionRequestContentRepository));

            #endregion

            #region Withdrawal Request Content
            services.AddScoped(typeof(IWithdrawalRequestContentRepository), typeof(WithdrawalRequestContentRepository));

            #endregion

            #region Owner Setting

            services.AddScoped(typeof(IOwnerSettingManager), typeof(OwnerSettingManager));
            services.AddScoped(typeof(IOwnerSettingRepository), typeof(OwnerSettingRepository));
            services.AddScoped(typeof(IOwnerSettingService), typeof(OwnerSettingService));

            #endregion

            #region Sadad Psp Devices

            services.AddScoped(typeof(ISadadPspDeviceManager), typeof(SadadPspDeviceManager));
            services.AddScoped(typeof(ISadadPspDeviceRepository), typeof(SadadPspDeviceRepository));
            #endregion

            #region Mydigi Devices

            services.AddScoped(typeof(IMydigiDeviceManager), typeof(MydigiDeviceManager));
            services.AddScoped(typeof(IMydigiDeviceRepository), typeof(MydigiDeviceRepository));
            #endregion

            #region IZMobile Devices

            services.AddScoped(typeof(IIZMobileDeviceRepository), typeof(IZMobileRepository));
            services.AddScoped(typeof(IIZMobileDeviceManager), typeof(IZMobileDeviceManager));

            #endregion

            #region Blocked Card Number

            services.AddScoped(typeof(IBlockedCardNumberManager), typeof(BlockedCardNumberManager));
            services.AddScoped(typeof(IBlockedCardNumberRepository), typeof(BlockedCardNumberRepository));
            services.AddScoped(typeof(IBlockedCardNumberService), typeof(BlockedCardNumberService));

            #endregion

            #region Payment780

            services.AddScoped(typeof(IPayment780DeviceManager), typeof(Payment780DeviceManager));
            services.AddScoped(typeof(IPayment780DeviceRepository), typeof(Payment780DeviceRepository));

            #endregion

            services.AddHttpClient();

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

            services.AddHttpContextAccessor();

            services.AddTransient<IConnectionStringManager, ConnectionStringManager>();
            services.AddTransient(typeof(SeedDataInvoker));
            services.AddScoped(typeof(CurrentUser));

            services.AddScoped(typeof(IAccountService), typeof(AccountService));

            var provider = services.BuildServiceProvider();

            services.AddScoped(typeof(ISmsService), typeof(SmsMockService));

            var mobileTransferConfiguration = provider.GetRequiredService<IOptions<MobileTransferConfiguration>>();

            if (mobileTransferConfiguration.Value.Mock)
            {
                services.AddScoped(typeof(IMobileTransferService), typeof(MobileTransferMockService));
            }
            else
            {
                services.AddScoped(typeof(IMobileTransferService), typeof(MobileTransferService));
            }

            services.AddScoped(typeof(IPaymentProxyApiService), typeof(PaymentProxyApiService));
            services.AddScoped(typeof(IPaymentProxyApiCommunicationService), typeof(PaymentProxyApiCommunicationService));

            services.AddSingleton(typeof(ICardToCardAccountGroupOrderManager), typeof(CardToCardAccountGroupOrderManager));

            services.AddScoped(typeof(ICachedObjectManager), typeof(CachedObjectManager));

            services.RegisterGenericServices<Pardakht.PardakhtPay.Shared.Models.Configuration.LoginManagementSettings>(configuration);
            services.RegisterGenericServices<UserManagementSettings>(configuration);
            services.RegisterGenericServices<TenantManagementSettings>(configuration);
            //services.RegisterGenericServices<TimeZoneConfiguration>(configuration);
            //services.RegisterGenericServices<DomainManagementSettings>(configuration);
            services.AddTransient<IUserResolverService, UserResolverService>();
            services.AddScoped<UserValidation>();

            services.AddScoped(typeof(ICardToCardPaymentService), typeof(CardToCardPaymentService));
            services.AddScoped(typeof(IMobilePaymentService), typeof(MobilePaymentService));
            services.AddScoped(typeof(UserContext));
        }

        /// <summary>
        /// Resolves and adds to the cache service
        /// </summary>
        /// <param name="services"></param>
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
    }
}
