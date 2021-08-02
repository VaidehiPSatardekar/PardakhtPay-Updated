using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.BotAutoTransferService.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.GenericManagementApi;

namespace Pardakht.PardakhtPay.BotAutoTransferService.OperationManagers
{
    public class ProxyPaymentThresholdManager : IProxyPaymentThresholdManager
    {
        ITransactionManager _TransactionManager;
        IMobileTransferCardAccountGroupItemManager _MobileTransferCardToCardAccountGroupItemManager;
        IMobileTransferCardAccountManager _MobileTransferCardAccountManager;
        ILogger _Logger;
        IServiceProvider _Provider;
        IBankBotService _BankBotService = null;
        IApplicationSettingService _ApplicationSettingService = null;
        ICardToCardAccountGroupItemManager _CardToCardGroupItemManager = null;
        ICardToCardAccountManager _CardToCardAccountManager = null;
        IBankStatementItemManager _BankStatementItemManager = null;

        public ProxyPaymentThresholdManager(ITransactionManager transactionManager,
            IMobileTransferCardAccountGroupItemManager mobileTransferCardAccountGroupItemManager,
            IMobileTransferCardAccountManager mobileTransferCardAccountManager,
            ILogger<ProxyPaymentThresholdManager> logger,
            IServiceProvider provider,
            IBankBotService bankBotService,
            IApplicationSettingService applicationSettingsService,
            ICardToCardAccountGroupItemManager cardToCardAccountGroupItemManager,
            ICardToCardAccountManager cardToCardAccountManager,
            IBankStatementItemManager bankStatementItemManager)
        {
            _TransactionManager = transactionManager;
            _MobileTransferCardToCardAccountGroupItemManager = mobileTransferCardAccountGroupItemManager;
            _MobileTransferCardAccountManager = mobileTransferCardAccountManager;
            _Logger = logger;
            _Provider = provider;
            _BankBotService = bankBotService;
            _ApplicationSettingService = applicationSettingsService;
            _CardToCardGroupItemManager = cardToCardAccountGroupItemManager;
            _CardToCardAccountManager = cardToCardAccountManager;
            _BankStatementItemManager = bankStatementItemManager;
        }

        public async Task Run()
        {
            try
            {
                await RunForTargetCardAccounts();

                await RunForCardToCardAccounts();
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, $"Error while processing proxy payment threshold amounts {ex.Message}");
            }
        }

        private async Task RunForCardToCardAccounts()
        {
            var activeItems = await _CardToCardGroupItemManager.GetActiveRelations();

            for (int i = 0; i < activeItems.Count; i++)
            {
                var activeItem = activeItems[i];

                try
                {
                    var account = await _CardToCardAccountManager.GetEntityByIdAsync(activeItem.CardToCardAccountId);

                    if (account != null && account.SwitchCreditDailyLimit.HasValue && account.SwitchCreditDailyLimit > 0)
                    {
                        var amount = await _BankStatementItemManager.GetTotalCreditAmount(account);

                        if (amount >= account.SwitchCreditDailyLimit)
                        {
                            _Logger.LogWarning($"ProxyPaymentThresholdManager: Changing status to Paused {account.AccountGuid} {account.SwitchCreditDailyLimit} {amount}");

                            await _CardToCardGroupItemManager.ReplaceReservedAccount(account, CardToCardAccountGroupItemStatus.Paused);

                            var functions = _Provider.GetRequiredService<IGenericManagementFunctions<PardakhtPayAuthenticationSettings>>();

                            using (var response = await functions.MakeRequest(null, null, "/api/banklogin/clearcache", HttpMethod.Post))
                            {
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _Logger.LogError(ex, $"Error while processing proxy payment threshold amounts for account {activeItem.Id} {ex.Message}");
                }
            }
        }

        private async Task RunForTargetCardAccounts()
        {
            var activeItems = await _MobileTransferCardToCardAccountGroupItemManager.GetAccountGroupItems();

            for (int i = 0; i < activeItems.Count; i++)
            {
                var activeItem = activeItems[i];

                try
                {
                    var account = await _MobileTransferCardAccountManager.GetEntityByIdAsync(activeItem.ItemId);

                    if (account.PaymentProviderType == (int)PaymentProviderTypes.PardakhtPal)
                    {
                        var isActive = await _MobileTransferCardToCardAccountGroupItemManager.CheckPardakhtPayAccountIsBlocked(account, activeItem);

                        if (!isActive)
                        {
                            var functions = _Provider.GetRequiredService<IGenericManagementFunctions<PardakhtPayAuthenticationSettings>>();

                            using (var response = await functions.MakeRequest(null, null, "/api/mobiletransfercardaccountgroup/clearcache", HttpMethod.Post))
                            {
                            }
                        }
                    }
                    else
                    {
                        if (account.ThresholdAmount > 0)
                        {
                            var amount = await _TransactionManager.GetTotalPaymentAmountForPaymentGateway(account);

                            if (amount >= account.ThresholdAmount)
                            {
                                _Logger.LogWarning($"ProxyPaymentThresholdManager: Changing status to Dormant {account.Title} {account.ThresholdAmount} {amount}");
                                activeItem.ItemStatus = MobileTransferCardAccountGroupItemStatus.Dormant;

                                await _MobileTransferCardToCardAccountGroupItemManager.UpdateAsync(activeItem);
                                await _MobileTransferCardToCardAccountGroupItemManager.SaveAsync();

                                await _MobileTransferCardToCardAccountGroupItemManager.Replace(activeItem);

                                var functions = _Provider.GetRequiredService<IGenericManagementFunctions<PardakhtPayAuthenticationSettings>>();

                                using (var response = await functions.MakeRequest(null, null, "/api/mobiletransfercardaccountgroup/clearcache", HttpMethod.Post))
                                {
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _Logger.LogError(ex, $"Error while processing proxy payment threshold amounts for account {activeItem.Id} {ex.Message}");
                }
            }
        }
    }
}
