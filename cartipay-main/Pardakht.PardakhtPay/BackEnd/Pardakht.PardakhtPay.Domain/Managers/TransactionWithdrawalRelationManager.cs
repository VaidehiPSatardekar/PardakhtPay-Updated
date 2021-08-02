using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using System.Linq;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.Shared.Models;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class TransactionWithdrawalRelationManager : BaseManager<TransactionWithdrawalRelation, ITransactionWithdrawalRelationRepository>, ITransactionWithdrawalRelationManager
    {
        IWithdrawalRepository _WithdrawalRepository = null;
        IWithdrawalManager _WithdrawalManager = null;
        ILogger _Logger = null;
        WithdrawalConfiguration _Configuration = null;
        const long MAX_WITHDRAWAL_AMOUNT = 30_000_000;
        const short MAX_SUGGESTED_ITEM_COUNT = 5;

        public TransactionWithdrawalRelationManager(ITransactionWithdrawalRelationRepository repository,
            IWithdrawalRepository withdrawalRepository,
            IWithdrawalManager withdrawalManager,
            IOptions<WithdrawalConfiguration> withdrawalOptions,
            ILogger<TransactionWithdrawalRelationManager> logger) : base(repository)
        {
            _WithdrawalManager = withdrawalManager;
            _WithdrawalRepository = withdrawalRepository;
            _Logger = logger;
            _Configuration = withdrawalOptions.Value;
        }

        public async Task<Withdrawal> GetWithdrawal(Transaction transaction, Merchant merchant)
        {
            try
            {
                Withdrawal withdrawal = GetExactAmountWithdrawal(transaction);

                if (withdrawal == null && merchant.AllowPartialPaymentForWithdrawals)
                {
                    withdrawal = GetRemainingAmountWithdrawal(transaction);

                    if (withdrawal == null)
                    {
                        withdrawal = GetPartialAmountWithdrawal(transaction);
                    }
                }

                if (withdrawal != null)
                {
                    var relation = new TransactionWithdrawalRelation();
                    relation.TransactionId = transaction.Id;
                    relation.WithdrawalId = withdrawal.Id;
                    relation.Date = DateTime.UtcNow;

                    await AddAsync(relation);
                    await SaveAsync();

                    return withdrawal;
                }

                return null;

            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return null;
            }
        }

        public async Task DeleteRelation(Transaction transaction)
        {
            try
            {
                var relation = await GetItemAsync(t => t.TransactionId == transaction.Id);

                if (relation != null)
                {
                    var withdrawal = await _WithdrawalManager.GetEntityByIdAsync(relation.WithdrawalId);

                    if (!string.IsNullOrEmpty(transaction.ExternalMessage)
                        && (transaction.ExternalMessage.Contains(Helper.MobileDeviceInvalidRequest)
                        || transaction.ExternalMessage.Contains(Helper.MobileDeviceLimit)
                        || transaction.ExternalMessage.Contains(Helper.MobileUnspecifiedTransactionResult)))
                    {
                        if (transaction.ExternalMessage.Contains(Helper.MobileUnspecifiedTransactionResult))
                        {
                            SetAsNeedsToBeConfirmed(transaction, withdrawal);
                        }
                    }
                    else if (transaction.TransactionStatus == TransactionStatus.WaitingConfirmation)
                    {
                        SetAsNeedsToBeConfirmed(transaction, withdrawal);
                    }
                    //else
                    //{
                    //    withdrawal.CardToCardTryCount++;
                    //}

                    await DeleteAsync(relation);
                    await SaveAsync();

                    await _WithdrawalManager.UpdateAsync(withdrawal);
                    await _WithdrawalManager.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }

        private static void SetAsNeedsToBeConfirmed(Transaction transaction, Withdrawal withdrawal)
        {
            withdrawal.WithdrawalStatus = (int)WithdrawalStatus.PendingCardToCardConfirmation;
            withdrawal.TransferStatusDescription = $"Withdrawal needs to be confirmed. Pelase check transaction {transaction.Id}";
            withdrawal.UpdateDate = DateTime.UtcNow;
            withdrawal.RemainingAmount -= transaction.TransactionAmount;
        }

        private Withdrawal GetExactAmountWithdrawal(Transaction transaction)
        {
            var types = new int[] { (int)WithdrawalProcessType.CardToCard, (int)WithdrawalProcessType.Both };
            var date = DateTime.UtcNow;
            var totalMinutes = _Configuration.CardToCardDeadline.TotalMinutes;

            var relationQuery = Repository.GetQuery();

            var pending = (int)WithdrawalStatus.Pending;
            var query = _WithdrawalRepository.GetQuery(t => t.OwnerGuid == transaction.OwnerGuid
                                                            && t.TenantGuid == transaction.TenantGuid
                                                            && t.WithdrawalStatus == pending
                                                            && !t.TransferId.HasValue
                                                            && t.Amount == transaction.TransactionAmount
                                                            && t.RemainingAmount == transaction.TransactionAmount
                                                            && t.CardToCardTryCount < _Configuration.MaxCardToCardTryCount
                                                            && types.Contains(t.WithdrawalProcessType)
                                                            && !string.IsNullOrEmpty(t.CardNumber)
                                                            && t.CardNumber != transaction.CustomerCardNumber
                                                            && (t.WithdrawalProcessType == (int)WithdrawalProcessType.CardToCard || t.ExpectedTransferDate.AddMinutes(totalMinutes) > date)
                                                            && (t.ExpectedTransferDate <= DateTime.UtcNow || t.ExpectedTransferDate == null));

            var withdrawal = query.Where(t => !relationQuery.Any(p => p.WithdrawalId == t.Id)).FirstOrDefault();
            return withdrawal;
        }

        private Withdrawal GetRemainingAmountWithdrawal(Transaction transaction)
        {
            var types = new int[] { (int)WithdrawalProcessType.CardToCard, (int)WithdrawalProcessType.Both };
            var date = DateTime.UtcNow;
            var totalMinutes = _Configuration.CardToCardDeadline.TotalMinutes;

            var relationQuery = Repository.GetQuery();

            var partial = (int)WithdrawalStatus.PartialPaid;
            var confirmation = (int)WithdrawalStatus.PendingCardToCardConfirmation;

            var query = _WithdrawalRepository.GetQuery(t => t.OwnerGuid == transaction.OwnerGuid
                                                            && t.TenantGuid == transaction.TenantGuid
                                                            && (t.WithdrawalStatus == partial || t.WithdrawalStatus == confirmation)
                                                            && !t.TransferId.HasValue
                                                            && (/*(t.Amount >= 20000000 && (t.Amount / 10) <= transaction.TransactionAmount) || */((t.Amount / 4) <= transaction.TransactionAmount) || (t.RemainingAmount == transaction.TransactionAmount) || ((t.RemainingAmount - transaction.TransactionAmount) == 100000))
                                                            && t.RemainingAmount >= transaction.TransactionAmount
                                                            && t.CardToCardTryCount < _Configuration.MaxCardToCardTryCount
                                                            && types.Contains(t.WithdrawalProcessType)
                                                            && !string.IsNullOrEmpty(t.CardNumber)
                                                            && t.CardNumber != transaction.CustomerCardNumber
                                                            && (t.WithdrawalProcessType == (int)WithdrawalProcessType.CardToCard || t.ExpectedTransferDate.AddMinutes(totalMinutes) > date)
                                                            && (t.ExpectedTransferDate <= DateTime.UtcNow || t.ExpectedTransferDate == null));

            var withdrawal = query.Where(t => !relationQuery.Any(p => p.WithdrawalId == t.Id)).OrderByDescending(t => t.Amount == t.RemainingAmount).ThenBy(t => t.ExpectedTransferDate).FirstOrDefault();
            return withdrawal;
        }

        private Withdrawal GetPartialAmountWithdrawal(Transaction transaction)
        {
            var types = new int[] { (int)WithdrawalProcessType.CardToCard, (int)WithdrawalProcessType.Both };
            var date = DateTime.UtcNow;
            var totalMinutes = _Configuration.CardToCardDeadline.TotalMinutes;

            var relationQuery = Repository.GetQuery();

            var pending = (int)WithdrawalStatus.Pending;

            var query = _WithdrawalRepository.GetQuery(t => t.OwnerGuid == transaction.OwnerGuid
                                                            && t.TenantGuid == transaction.TenantGuid
                                                            && t.WithdrawalStatus == pending
                                                            && !t.TransferId.HasValue
                                                            && (/*(t.Amount >= 20000000 && (t.Amount / 10) <= transaction.TransactionAmount) || */ ((t.Amount / 4) <= transaction.TransactionAmount))
                                                            && t.RemainingAmount >= transaction.TransactionAmount
                                                            && t.CardToCardTryCount < _Configuration.MaxCardToCardTryCount
                                                            && types.Contains(t.WithdrawalProcessType)
                                                            && !string.IsNullOrEmpty(t.CardNumber)
                                                            && t.CardNumber != transaction.CustomerCardNumber
                                                            && (t.WithdrawalProcessType == (int)WithdrawalProcessType.CardToCard || t.ExpectedTransferDate.AddMinutes(totalMinutes) > date)
                                                            && (t.ExpectedTransferDate <= DateTime.UtcNow || t.ExpectedTransferDate == null));

            var withdrawal = query.Where(t => !relationQuery.Any(p => p.WithdrawalId == t.Id)).FirstOrDefault();
            return withdrawal;
        }

        public async Task<List<DecimalContainer>> GetSuggestedWithdrawalData(Merchant merchant, long minAmount, long maxAmount)
        {
            var types = new int[] { (int)WithdrawalProcessType.CardToCard, (int)WithdrawalProcessType.Both };
            var date = DateTime.UtcNow;
            var totalMinutes = _Configuration.CardToCardDeadline.TotalMinutes;

            var relationQuery = Repository.GetQuery();

            var pending = (int)WithdrawalStatus.Pending;
            var partialPaid = (int)WithdrawalStatus.PartialPaid;

            int[] statuses = null;

            if (merchant.AllowPartialPaymentForWithdrawals)
            {
                statuses = new int[] { pending, partialPaid };
            }
            else
            {
                statuses = new int[] { pending };
            }

            var query = _WithdrawalRepository.GetQuery(t => t.OwnerGuid == merchant.OwnerGuid
                                                            && statuses.Contains(t.WithdrawalStatus)
                                                            && !t.TransferId.HasValue
                                                            && t.RemainingAmount >= minAmount
                                                            && t.RemainingAmount <= maxAmount
                                                            && t.CardToCardTryCount < _Configuration.MaxCardToCardTryCount
                                                            && types.Contains(t.WithdrawalProcessType)
                                                            && !string.IsNullOrEmpty(t.CardNumber)
                                                            && (t.WithdrawalProcessType == (int)WithdrawalProcessType.CardToCard || t.ExpectedTransferDate.AddMinutes(totalMinutes) > date)
                                                            && (t.ExpectedTransferDate <= DateTime.UtcNow || t.ExpectedTransferDate == null));

            var amountQuery = query.Where(t => !relationQuery.Any(p => p.WithdrawalId == t.Id)).Select(p => new DecimalContainer() { Value = p.RemainingAmount }).Distinct();

            var items = await _WithdrawalRepository.GetModelItemsAsync(amountQuery);

            var amounts = items.ToList();

            return amounts;
        }


        public async Task<List<long>> GetSuggestedWithdrawalAmounts(long amount, Merchant merchant)
        {
            var minAmount = amount / 4;
            var maxAmount = amount * 2;

            maxAmount = Math.Min(maxAmount, MAX_WITHDRAWAL_AMOUNT);

            var amounts = await GetSuggestedWithdrawalData(merchant, minAmount, maxAmount);

            if (!amounts.Any(p => p.Value <= amount))
            {
                amounts.Add(new DecimalContainer()
                {
                    Value = amount
                });
            }

            if (amounts.Count > MAX_SUGGESTED_ITEM_COUNT)
            {
                amounts = amounts
                    .OrderBy(p => p.Value == amount ? 0 : 1)
                    .ThenBy(p => Math.Abs(p.Value - amount))
                    .Take(MAX_SUGGESTED_ITEM_COUNT)
                    .ToList();
            }

            return amounts
                .OrderBy(p => p.Value == amount ? 0 : 1)
                .ThenBy(p => p.Value)
                .Select(p => Convert.ToInt64(p.Value))
                .ToList();
        }

        
        public async Task<List<long>> GetSuggestedWithdrawalAmountsWithZeroAmount(Merchant merchant)
        {
            var minAmount = 0;
            var maxAmount = _Configuration.MaxWithdrawalAmount;

            var amounts = await GetSuggestedWithdrawalData(merchant, minAmount, maxAmount);

            if (amounts.Count > MAX_SUGGESTED_ITEM_COUNT)
            {
                amounts = amounts
                    .OrderByDescending(p => p.Value)
                    .Take(MAX_SUGGESTED_ITEM_COUNT)
                    .ToList();
            }

            return amounts
                .OrderByDescending(p => p.Value)
                .Select(p => Convert.ToInt64(p.Value))
                .ToList();
        }

        public async Task CompleteWithdrawal(Transaction transaction)
        {
            try
            {
                var relation = await GetItemAsync(t => t.WithdrawalId == transaction.WithdrawalId);

                if (relation != null)
                {
                    var withdrawal = await _WithdrawalManager.SetAsCompleted(transaction.WithdrawalId.Value, transaction.TransactionAmount);

                    await DeleteAsync(relation);
                    await SaveAsync();
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }
    }

    public class DecimalContainer
    {
        public decimal Value { get; set; }
    }
}
