using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.BotAutoTransferService.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.BotAutoTransferService.OperationManagers
{
    public class WithdrawalOperationManager : IWithdrawalOperationManager
    {
        IWithdrawalManager _Manager;
        WithdrawalConfiguration _Configuration;
        IBankBotService _BankBotService = null;
        IAesEncryptionService _EncryptionService = null;
        ILogger _Logger = null;
        IMerchantCustomerManager _MerchantCustomerManager = null;
        ICardToCardAccountGroupItemManager _CardToCardGroupItemManager = null;
        ICachedObjectManager _CachedObjectManager = null;
        IApplicationSettingService _ApplicationSettingsService = null;
        IWithdrawalTransferHistoryRepository _WithdrawalTransferRepository = null;
        IApplicationSettingService _ApplicationSettingService = null;
        IOwnerSettingManager _OwnerSettingManager = null;

        public WithdrawalOperationManager(IWithdrawalManager manager,
            IOptions<WithdrawalConfiguration> withdrawalOptions,
            ICardToCardAccountManager cardToCardAccountManager,
            IBankBotService bankBotService,
            IAesEncryptionService encryptionService,
            IMerchantCustomerManager merchantCustomerManager,
            ICardToCardAccountGroupItemManager cardAccountGroupItemManager,
            ICachedObjectManager cachedObjectManager,
            IApplicationSettingService applicationSettingsService,
            IWithdrawalTransferHistoryRepository withdrawalTransferHistoryRepository,
            ILogger<WithdrawalOperationManager> logger,
            IApplicationSettingService applicationSettingService,
            IOwnerSettingManager ownerSettingManager)
        {
            _Manager = manager;
            _Configuration = withdrawalOptions.Value;
            _BankBotService = bankBotService;
            _EncryptionService = encryptionService;
            _Logger = logger;
            _MerchantCustomerManager = merchantCustomerManager;
            _CardToCardGroupItemManager = cardAccountGroupItemManager;
            _CachedObjectManager = cachedObjectManager;
            _ApplicationSettingsService = applicationSettingsService;
            _WithdrawalTransferRepository = withdrawalTransferHistoryRepository;
            _ApplicationSettingService = applicationSettingService;
            _OwnerSettingManager = ownerSettingManager;
        }

        public async Task Run()
        {
            try
            {
                var items = await _Manager.GetUnprocessedWithdrawals();

                items = items.OrderBy(t => t.Id).ToList();

                var bankLogins = await _BankBotService.GetLogins();
                var banks = await _BankBotService.GetBanks();
                var statuses = await _BankBotService.GetAccountsWithStatus();

                var pendings = await _Manager.GetPendingWithdrawalAmounts();

                var pendingBalances = await _Manager.GetPendingWithdrawalBalance();

                for (int i = 0; i < statuses.Count; i++)
                {
                    var pending = pendings.FirstOrDefault(t => t.AccountGuid == statuses[i].AccountGuid && t.TransferType == statuses[i].TransferType);

                    if (pending != null && pending.Amount.HasValue)
                    {
                        statuses[i].WithdrawRemainedAmountForDay -= Convert.ToInt64(pending.Amount.Value);
                    }

                    var pendingAmounts = pendings.Where(t => t.AccountGuid == statuses[i].AccountGuid).Sum(t => t.Amount);

                    if (pendingAmounts.HasValue)
                    {
                        statuses[i].WithdrawableBalance -= Convert.ToInt64(pendingAmounts.Value);
                    }

                    var pendingBalance = pendingBalances.Where(t => t.AccountGuid == statuses[i].AccountGuid).Sum(t => t.Amount);

                    if (pendingBalance.HasValue)
                    {
                        statuses[i].WithdrawableBalance -= Convert.ToInt64(pendingBalance.Value);
                    }
                }

                var configuration = await _ApplicationSettingsService.Get<BankAccountConfiguration>();

                if (configuration.UseSameWithdrawalAccountForCustomer)
                {
                    await ProcessWithdrawalWithCustomerAccount(items, bankLogins, banks, statuses);
                }
                else
                {
                    await ProcessWithdrawalWithoutCustomerAccount(items, bankLogins, banks, statuses);
                }


                var date = DateTime.UtcNow.Subtract(_Configuration.ConfirmationDeadline);
                await _Manager.ConfirmWithdrawals(date);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }

        private async Task ProcessWithdrawalWithoutCustomerAccount(List<Withdrawal> items, List<BotLoginInformation> bankLogins, List<BotBankInformation> banks, List<BankBotAccountWithStatusDTO> statuses)
        {
            var accounts = await _CachedObjectManager.GetCachedItems<CardToCardAccountGroupItem, ICardToCardAccountGroupItemRepository>();

            accounts = accounts.Where(t => t.AllowWithdrawal).ToList();

            var ownerSettings = await _OwnerSettingManager.GetAllAsync();

            var cardToCardAccounts = await _CachedObjectManager.GetCachedItems<CardToCardAccount, ICardToCardAccountRepository>();
            var configuration = await _ApplicationSettingService.Get<BankAccountConfiguration>();

            Dictionary<string, bool> ownerWaitFlags = new Dictionary<string, bool>();

            for (int itemIndex = 0; itemIndex < items.Count; itemIndex++)
            {
                try
                {
                    var withdrawal = await _Manager.GetItemAsync(t => t.Id == items[itemIndex].Id);

                    if ( withdrawal == null
                        || withdrawal.TransferId.HasValue
                        || withdrawal.TransferRequestDate != null
                        || withdrawal.TransferStatus == (int)TransferStatus.Cancelled
                        || ownerWaitFlags.ContainsKey(withdrawal.OwnerGuid) && ownerWaitFlags[withdrawal.OwnerGuid])
                    {
                        continue;
                    }

                    var activeItems = await _CardToCardGroupItemManager.GetActiveRelationsWithoutUserSegmentation(withdrawal.MerchantId, null, true);

                    activeItems = activeItems.OrderBy(t => t.Id).ToList();

                    bool ignored = false;

                    for (int accountIndex = 0; accountIndex < activeItems.Count; accountIndex++)
                    {
                        var groupItem = activeItems[accountIndex];

                        var cardAccount = cardToCardAccounts.FirstOrDefault(t => t.Id == groupItem.CardToCardAccountId);

                        if (cardAccount != null)
                        {
                            var bankBotAccount = statuses.FirstOrDefault(t => t.AccountGuid == cardAccount.AccountGuid);

                            if (bankBotAccount != null && !bankBotAccount.IsDeleted && bankBotAccount.IsOpen(configuration.BlockAccountLimit))
                            {
                                var login = bankLogins.FirstOrDefault(t => t.LoginGuid == bankBotAccount.LoginGuid);

                                if (!login.IsDeleted && !login.IsBlocked)
                                {
                                    var iban = _EncryptionService.DecryptToString(withdrawal.ToIbanNumber);

                                    var bankCode = iban.GetBankCodeFromIban();

                                    var bank = banks.FirstOrDefault(t => t.BankCode == bankCode);

                                    int transferType = (int)TransferType.Normal;

                                    if (bank == null || bank.Id != login.BankId)
                                    {
                                        transferType = (int)TransferType.Paya;
                                    }

                                    var status = statuses.FirstOrDefault(t => t.AccountGuid == cardAccount.AccountGuid && t.TransferType == transferType);

                                    if (status != null)
                                    {
                                        var balance = status.WithdrawableBalance;

                                        if (balance < withdrawal.RemainingAmount || withdrawal.RemainingAmount > status.WithdrawRemainedAmountForDay || withdrawal.RemainingAmount > status.WithdrawRemainedAmountForMonth)
                                        {
                                            _Logger.LogWarning($"This account is not available. {withdrawal.RemainingAmount} {balance} {status.WithdrawRemainedAmountForDay} {status.WithdrawRemainedAmountForMonth}");

                                            ignored = true;

                                            continue;
                                        }
                                        else
                                        {
                                            ignored = false;

                                            withdrawal.TransferType = transferType;

                                            var dto = new BotTransferRequestDTO();
                                            dto.TransferBalance = Convert.ToInt64(withdrawal.RemainingAmount);
                                            dto.TransferFromAccount = bankBotAccount.AccountNo;

                                            dto.TransferPriority = withdrawal.Priority;
                                            dto.TransferType = withdrawal.TransferType;
                                            dto.FirstName = _EncryptionService.DecryptToString(withdrawal.FirstName);
                                            dto.LastName = _EncryptionService.DecryptToString(withdrawal.LastName);

                                            if (withdrawal.TransferType == (int)TransferType.Normal)
                                            {
                                                var accountNumber = iban.GetAccountNumberFromIban();

                                                if (string.IsNullOrEmpty(accountNumber))
                                                {
                                                    dto.TransferToAccount = _EncryptionService.DecryptToString(withdrawal.ToAccountNumber);
                                                }
                                                else
                                                {
                                                    dto.TransferToAccount = accountNumber;
                                                }
                                            }
                                            else
                                            {
                                                dto.TransferToAccount = _EncryptionService.DecryptToString(withdrawal.ToIbanNumber);
                                            }

                                            var botTransfer = await _BankBotService.CreateTransferRequest(dto);

                                            await AddWithdrawalTransferHistory(botTransfer, withdrawal);

                                            statuses.Where(t => t.AccountGuid == cardAccount.AccountGuid).ToList().ForEach(t =>
                                            {
                                                t.WithdrawableBalance -= Convert.ToInt64(withdrawal.RemainingAmount);
                                            });

                                            status.WithdrawRemainedAmountForDay -= Convert.ToInt64(withdrawal.RemainingAmount);
                                            status.WithdrawRemainedAmountForMonth -= Convert.ToInt64(withdrawal.RemainingAmount);

                                            string description = string.Empty;

                                            try
                                            {
                                                if (!string.IsNullOrEmpty(botTransfer.TransferStatusDescription))
                                                {
                                                    description = botTransfer.TransferStatusDescription;

                                                    if (description.Length > 2_000)
                                                    {
                                                        description = description.Substring(0, 2000);
                                                    }
                                                }
                                            }
                                            catch(Exception e)
                                            {
                                                _Logger.LogError(e, e.Message);
                                            }

                                            withdrawal.FromAccountNumber = _EncryptionService.EncryptToBase64(bankBotAccount.AccountNo);
                                            withdrawal.AccountGuid = bankBotAccount.AccountGuid;
                                            withdrawal.TransferAccountId = cardAccount.Id;
                                            withdrawal.TransferRequestGuid = botTransfer.TransferRequestGuid;
                                            withdrawal.TransferId = botTransfer.Id;
                                            withdrawal.TransferStatus = botTransfer.TransferStatus;
                                            withdrawal.TransferStatusDescription = description;
                                            withdrawal.TransferNotes = botTransfer.TransferNotes;
                                            withdrawal.TransferRequestDate = botTransfer.TransferRequestDate;
                                            withdrawal.WithdrawalStatus = (int)WithdrawalStatus.Sent;

                                            await _Manager.UpdateAsync(withdrawal);
                                            await _Manager.SaveAsync();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (ignored)
                    {
                        var setting = ownerSettings.FirstOrDefault(t => t.OwnerGuid == withdrawal.OwnerGuid);

                        if (setting != null && setting.WaitAmountForCurrentWithdrawal)
                        {
                            ownerWaitFlags.Add(withdrawal.OwnerGuid, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _Logger.LogError(ex, ex.Message);
                }
            }
        }

        private async Task ProcessWithdrawalWithCustomerAccount(List<Withdrawal> items, List<BotLoginInformation> bankLogins, List<BotBankInformation> banks, List<BankBotAccountWithStatusDTO> statuses)
        {
            for (int i = 0; i < items.Count; i++)
            {
                try
                {
                    var withdrawal = items[i];

                    var customerResponse = await _MerchantCustomerManager.GetWithdrawAccountForCustomer(withdrawal.MerchantCustomerId, withdrawal.MerchantId);

                    if (customerResponse != null && customerResponse.Account != null)
                    {
                        var bankBotAccount = await _BankBotService.GetAccountByGuid(customerResponse.Account.AccountGuid);

                        if (bankBotAccount != null)
                        {
                            var login = bankLogins.FirstOrDefault(t => t.LoginGuid == bankBotAccount.LoginGuid);

                            var iban = _EncryptionService.DecryptToString(withdrawal.ToIbanNumber);

                            var bankCode = iban.GetBankCodeFromIban();

                            var bank = banks.FirstOrDefault(t => t.BankCode == bankCode);

                            int transferType = (int)TransferType.Normal;

                            if (bank == null || bank.Id != login.BankId)
                            {
                                transferType = (int)TransferType.Paya;
                            }

                            withdrawal.TransferType = transferType;
                            withdrawal.FromAccountNumber = _EncryptionService.EncryptToBase64(bankBotAccount.AccountNo);
                            withdrawal.AccountGuid = bankBotAccount.AccountGuid;
                            withdrawal.TransferAccountId = customerResponse.Account.Id;

                            await _Manager.UpdateAsync(withdrawal);
                            await _Manager.SaveAsync();

                            var status = statuses.FirstOrDefault(t => t.AccountGuid == withdrawal.AccountGuid && t.TransferType == withdrawal.TransferType);

                            if (status != null)
                            {
                                var balance = status.WithdrawableBalance;

                                if (balance < withdrawal.RemainingAmount)
                                {
                                    withdrawal.TransferStatusDescription = "Balance";
                                    withdrawal.WithdrawalStatus = (int)WithdrawalStatus.PendingBalance;

                                    await _Manager.UpdateAsync(withdrawal);
                                    await _Manager.SaveAsync();
                                }
                                else if (withdrawal.RemainingAmount > status.WithdrawRemainedAmountForDay)
                                {
                                    withdrawal.TransferStatusDescription = "Daily";
                                    withdrawal.WithdrawalStatus = (int)WithdrawalStatus.PendingBalance;

                                    await _Manager.UpdateAsync(withdrawal);
                                    await _Manager.SaveAsync();
                                }
                                else if (withdrawal.RemainingAmount > status.WithdrawRemainedAmountForMonth)
                                {
                                    withdrawal.TransferStatusDescription = "Monthly";
                                    withdrawal.WithdrawalStatus = (int)WithdrawalStatus.PendingBalance;

                                    await _Manager.UpdateAsync(withdrawal);
                                    await _Manager.SaveAsync();
                                }
                                else
                                {

                                    var dto = new BotTransferRequestDTO();
                                    dto.TransferBalance = Convert.ToInt64(withdrawal.RemainingAmount);
                                    dto.TransferFromAccount = bankBotAccount.AccountNo;

                                    dto.TransferPriority = withdrawal.Priority;
                                    dto.TransferType = withdrawal.TransferType;
                                    dto.FirstName = _EncryptionService.DecryptToString(withdrawal.FirstName);
                                    dto.LastName = _EncryptionService.DecryptToString(withdrawal.LastName);

                                    if (withdrawal.TransferType == (int)TransferType.Normal)
                                    {
                                        var accountNumber = iban.GetAccountNumberFromIban();

                                        if (string.IsNullOrEmpty(accountNumber))
                                        {
                                            dto.TransferToAccount = _EncryptionService.DecryptToString(withdrawal.ToAccountNumber);
                                        }
                                        else
                                        {
                                            dto.TransferToAccount = accountNumber;
                                        }
                                    }
                                    else
                                    {
                                        dto.TransferToAccount = _EncryptionService.DecryptToString(withdrawal.ToIbanNumber);
                                    }

                                    var botTransfer = await _BankBotService.CreateTransferRequest(dto);

                                    await AddWithdrawalTransferHistory(botTransfer, withdrawal);

                                    status.WithdrawableBalance -= Convert.ToInt64(withdrawal.RemainingAmount);
                                    status.WithdrawRemainedAmountForDay -= Convert.ToInt64(withdrawal.RemainingAmount);
                                    status.WithdrawRemainedAmountForMonth -= Convert.ToInt64(withdrawal.RemainingAmount);

                                    withdrawal.TransferRequestGuid = botTransfer.TransferRequestGuid;
                                    withdrawal.TransferId = botTransfer.Id;
                                    withdrawal.TransferStatus = botTransfer.TransferStatus;
                                    withdrawal.TransferStatusDescription = botTransfer.TransferStatusDescription;
                                    withdrawal.TransferNotes = botTransfer.TransferNotes;
                                    withdrawal.TransferRequestDate = botTransfer.TransferRequestDate;
                                    withdrawal.WithdrawalStatus = (int)WithdrawalStatus.Sent;

                                    await _Manager.UpdateAsync(withdrawal);
                                    await _Manager.SaveAsync();
                                }
                            }
                            else
                            {
                                _Logger.LogWarning($"Balance could not be found for this account. Account Guid : {withdrawal.AccountGuid}. Id : {withdrawal.Id}");
                            }
                        }
                        else
                        {
                            _Logger.LogWarning($"Withdrawal ignored because of null bank bot account. Account Guid : {customerResponse.Account.AccountGuid}");
                        }
                    }
                    else
                    {
                        _Logger.LogWarning($"Withdrawal ignored because of null account. Id : {withdrawal.Id}");
                    }
                }
                catch (Exception ex)
                {
                    _Logger.LogError(ex, ex.Message);
                }
            }
        }

        private async Task AddWithdrawalTransferHistory(BankBotTransferRequest transfer, Withdrawal withdrawal)
        {
            try
            {
                await _WithdrawalTransferRepository.InsertAsync(new WithdrawalTransferHistory()
                {
                    Amount = transfer.TransferBalance,
                    LastCheckDate = transfer.TransferRequestDate,
                    RequestedDate = transfer.TransferRequestDate,
                    TransferId = transfer.Id,
                    TransferNotes = transfer.TransferNotes,
                    TransferStatus = transfer.TransferStatus,
                    TransferStatusDescription = transfer.TransferStatusDescription,
                    WithdrawalId = withdrawal.Id
                });

                await _WithdrawalTransferRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }
    }
}
