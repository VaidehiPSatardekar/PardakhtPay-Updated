using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.BotAutoTransferService.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.BotAutoTransferService.OperationManagers
{
    public class AutoTransferOperationManager : IAutoTransferOperationManager
    {
        IAutoTransferManager _Manager;
        AutoTransferSettings _Settings;
        ICardToCardAccountManager _CardToCardAccountManager = null;
        IBankBotService _BankBotService = null;
        IAesEncryptionService _EncryptionService = null;
        ILogger<AutoTransferOperationManager> _Logger = null;
        IApplicationSettingService _ApplicationSettingService = null;

        public AutoTransferOperationManager(IAutoTransferManager manager,
            IOptions<AutoTransferSettings> autoTransferOptions,
            ICardToCardAccountManager cardToCardAccountManager,
            IBankBotService bankBotService,
            IAesEncryptionService encryptionService,
            ILogger<AutoTransferOperationManager> logger,
            IApplicationSettingService applicationSettingService)
        {
            _Manager = manager;
            _Settings = autoTransferOptions.Value;
            _CardToCardAccountManager = cardToCardAccountManager;
            _BankBotService = bankBotService;
            _EncryptionService = encryptionService;
            _Logger = logger;
            _ApplicationSettingService = applicationSettingService;
        }

        public async Task Run()
        {
            try
            {
                var cardToCardAccounts = await _CardToCardAccountManager.GetActiveCardToCardAccountsAsync();

                var uncompletedRequests = await _Manager.GetPendingTransfers();

                var logins = await _BankBotService.GetLogins();

                cardToCardAccounts = cardToCardAccounts.Where(t => !uncompletedRequests.Select(p => p.CardToCardAccountId).Contains(t.Id)).ToList();

                logins = logins.Where(t => t.IsActive && !t.IsBlocked).ToList();

                cardToCardAccounts = cardToCardAccounts.Where(t => logins.Any(p => p.LoginGuid == t.LoginGuid && !p.IsDeleted)).ToList();

                var bankBotAccounts = await _BankBotService.GetAccountsWithStatus();
                var configuration = await _ApplicationSettingService.Get<BankAccountConfiguration>();

                var banks = await _BankBotService.GetBanks();

                for (int i = 0; i < cardToCardAccounts.Count; i++)
                {
                    try
                    {
                        var cardToCardAccount = cardToCardAccounts[i];

                        if (cardToCardAccount.IsTransferThresholdActive && !string.IsNullOrEmpty(cardToCardAccount.SafeAccountNumber))
                        {
                            var login = logins.FirstOrDefault(t => t.LoginGuid == cardToCardAccount.LoginGuid);

                            bool supportSatna = await _BankBotService.IsSatnaApplicable(login.BankId);

                            var bankBotAccount = bankBotAccounts.FirstOrDefault(t => t.AccountGuid == cardToCardAccount.AccountGuid && t.TransferType == (int)TransferType.Normal);

                            if (bankBotAccount != null && bankBotAccount.IsOpen(configuration.BlockAccountLimit) && bankBotAccount.Balance > cardToCardAccount.TransferThreshold && login != null)
                            {
                                var bank = banks.FirstOrDefault(t => t.Id == login.BankId);

                                if (bank != null)
                                {
                                    var decryptedSafeAccounts = _EncryptionService.DecryptToString(cardToCardAccount.SafeAccountNumber);

                                    var safeAccounts = decryptedSafeAccounts.Split('#', StringSplitOptions.RemoveEmptyEntries).ToList();

                                    for (int safeAccountIndex = 0; safeAccountIndex < safeAccounts.Count; safeAccountIndex++)
                                    {
                                        var safeAccount = bankBotAccounts.FirstOrDefault(t => t.AccountNo == safeAccounts[safeAccountIndex]);

                                        if (safeAccount != null && !safeAccount.IsOpen(configuration.BlockAccountLimit))
                                        {
                                            safeAccounts.RemoveAt(safeAccountIndex);
                                            safeAccountIndex--;
                                        }
                                    }

                                    if (safeAccounts.Count == 0)
                                    {
                                        continue;
                                    }

                                    var autoTransfer = await _Manager.GetLatestAutoTranferRecord(cardToCardAccount.Id);

                                    if (autoTransfer != null)
                                    {
                                        var toAccount = _EncryptionService.DecryptToString(autoTransfer.TransferToAccount);

                                        var index = safeAccounts.IndexOf(toAccount);

                                        if (index != -1)
                                        {
                                            var olds = safeAccounts.Take(index + 1);
                                            safeAccounts = safeAccounts.Skip(index + 1).ToList();
                                            safeAccounts.AddRange(olds);
                                        }
                                    }

                                    decimal amount = bankBotAccount.WithdrawableBalance;

                                    if (bankBotAccount.Balance <= amount)
                                    {
                                        amount = bankBotAccount.Balance;
                                    }

                                    if (cardToCardAccount.TransferThresholdLimit.HasValue)
                                    {
                                        amount -= cardToCardAccount.TransferThresholdLimit.Value;
                                    }

                                    if (bank.ThresholdLimit.HasValue && amount > bank.ThresholdLimit)
                                    {
                                        amount = Convert.ToDecimal(bank.ThresholdLimit);
                                    }

                                    for (int safeAccountIndex = 0; safeAccountIndex < safeAccounts.Count; safeAccountIndex++)
                                    {
                                        bool sent = false;

                                        var safeAccountNumber = safeAccounts[safeAccountIndex];

                                        if (safeAccountNumber.ToLowerInvariant().StartsWith("ir"))
                                        {
                                            if (!safeAccountNumber.CheckIbanIsValid())
                                            {
                                                continue;
                                            }
                                            if (supportSatna && amount >= 150_000_000)
                                            {
                                                bankBotAccount = bankBotAccounts.FirstOrDefault(t => t.AccountGuid == cardToCardAccount.AccountGuid
                                                                                                && t.TransferType == (int)TransferType.Satna
                                                                                                && t.WithdrawRemainedAmountForDay >= t.MinimumWithdrawalOfEachTransaction
                                                                                                && t.WithdrawRemainedAmountForMonth >= t.MinimumWithdrawalOfEachTransaction
                                                                                                && amount >= t.MinimumWithdrawalOfEachTransaction
                                                                                                && t.WithdrawableLimit >= amount);

                                                if (bankBotAccount != null)
                                                {
                                                    sent = true;
                                                }
                                            }

                                            if (!sent)
                                            {
                                                bankBotAccount = bankBotAccounts.FirstOrDefault(t => t.AccountGuid == cardToCardAccount.AccountGuid
                                                                                                && t.TransferType == (int)TransferType.Paya
                                                                                                && t.WithdrawRemainedAmountForDay >= t.MinimumWithdrawalOfEachTransaction
                                                                                                && t.WithdrawRemainedAmountForMonth >= t.MinimumWithdrawalOfEachTransaction
                                                                                                && amount >= t.MinimumWithdrawalOfEachTransaction
                                                                                                && t.WithdrawableLimit >= amount);
                                            }
                                        }
                                        else
                                        {
                                            bankBotAccount = bankBotAccounts.FirstOrDefault(t => t.AccountGuid == cardToCardAccount.AccountGuid && t.TransferType == (int)TransferType.Normal);
                                        }

                                        if (bankBotAccount != null)
                                        {
                                            if (amount > bankBotAccount.WithdrawRemainedAmountForDay)
                                            {
                                                amount = bankBotAccount.WithdrawRemainedAmountForDay;
                                            }

                                            if (amount > bankBotAccount.MaximumWithdrawalOfEachTransaction)
                                            {
                                                amount = bankBotAccount.MaximumWithdrawalOfEachTransaction;
                                            }

                                            if (amount > 0 && amount >= bankBotAccount.MinimumWithdrawalOfEachTransaction)
                                            {
                                                try
                                                {
                                                    var transfer = new AutoTransfer();

                                                    transfer.AccountGuid = cardToCardAccount.AccountGuid;
                                                    transfer.Amount = amount;
                                                    transfer.CancelDate = null;
                                                    transfer.CardToCardAccountId = cardToCardAccount.Id;
                                                    transfer.IsCancelled = false;
                                                    transfer.OwnerGuid = cardToCardAccount.OwnerGuid;
                                                    transfer.Priority = (int)TransferPriority.High;
                                                    transfer.TenantGuid = cardToCardAccount.TenantGuid;
                                                    transfer.TransferFromAccount = _EncryptionService.EncryptToBase64(bankBotAccount.AccountNo);
                                                    transfer.TransferredDate = null;
                                                    transfer.TransferRequestDate = DateTime.UtcNow;
                                                    transfer.TransferToAccount = _EncryptionService.EncryptToBase64(safeAccountNumber);

                                                    var dto = new BotTransferRequestDTO();
                                                    dto.TransferBalance = Convert.ToInt64(amount);
                                                    dto.TransferFromAccount = bankBotAccount.AccountNo;
                                                    dto.TransferPriority = (int)TransferPriority.High;
                                                    dto.TransferToAccount = safeAccountNumber;
                                                    dto.TransferType = bankBotAccount.TransferType;
                                                    dto.FirstName = _EncryptionService.DecryptToString(cardToCardAccount.CardHolderName);
                                                    dto.LastName = _EncryptionService.DecryptToString(cardToCardAccount.CardHolderName);

                                                    var botTransfer = await _BankBotService.CreateTransferRequest(dto);

                                                    transfer.RequestGuid = botTransfer.TransferRequestGuid;
                                                    transfer.RequestId = botTransfer.Id;
                                                    transfer.Status = botTransfer.TransferStatus;
                                                    transfer.StatusDescription = botTransfer.TransferStatusDescription;

                                                    await _Manager.AddAsync(transfer);
                                                    await _Manager.SaveAsync();
                                                    break;
                                                }
                                                catch (Exception ex)
                                                {
                                                    _Logger.LogError(ex, ex.Message);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _Logger.LogError(ex, $"{cardToCardAccounts[i].Id} {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }
    }
}
