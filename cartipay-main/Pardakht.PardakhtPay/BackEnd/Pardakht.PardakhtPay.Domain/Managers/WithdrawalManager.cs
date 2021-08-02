using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using System.Linq;
using Pardakht.PardakhtPay.Shared.Extensions;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.ExternalServices.Queue;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class WithdrawalManager : BaseManager<Withdrawal, IWithdrawalRepository>, IWithdrawalManager
    {
        IAesEncryptionService _AesEncryptionService = null;
        IMerchantRepository _MerchantRepository = null;
        ITimeZoneService _TimeZoneService = null;
        IBankBotService _BankBotService = null;
        IMerchantCustomerRepository _MerchantCustomerRepository = null;
        ICardToCardAccountRepository _CardToCardAccountRepository = null;
        ILogger _Logger = null;
        IBankStatementItemRepository _BankStatementItemRepository = null;
        ITransactionQueueService _TransactionQueueService = null;
        WithdrawalConfiguration _Configuration = null;
        ITransactionWithdrawalRelationRepository _TransactionWithdrawalRelationRepository = null;
        ITransactionRepository _TransactionRepository = null;
        CurrentUser _CurrentUser = null;
        IWithdrawalTransferHistoryRepository _WithdrawalTransferHistoryRepository = null;
        ICardHolderNameRepository _CardHolderNameRepository = null;
        IMerchantCustomerManager _MerchantCustomerManager = null;
        IWithdrawalRequestContentRepository _WithdrawalRequestContentRepository = null;


        public WithdrawalManager(IWithdrawalRepository repository,
            IMerchantRepository merchantRepository,
            ITimeZoneService timeZoneService,
            IBankBotService bankBotService,
            ICardToCardAccountRepository cardToCardAccountRepository,
            IMerchantCustomerRepository merchantCustomerRepository,
            ILogger<WithdrawalManager> logger,
            IBankStatementItemRepository bankStatementItemRepository,
            ITransactionQueueService transactionQueueService,
            IOptions<WithdrawalConfiguration> withdrawalOptions,
            ITransactionWithdrawalRelationRepository transactionWithdrawalRelationRepository,
            ITransactionRepository transactionRepository,
            CurrentUser currentUser,
            IWithdrawalTransferHistoryRepository withdrawalTransferHistoryRepository,
            ICardHolderNameRepository cardHolderNameRepository,
            IAesEncryptionService aesEncryptionService,
            IMerchantCustomerManager merchantCustomerManager,
            IWithdrawalRequestContentRepository withdrawalRequestContentRepository) : base(repository)
        {
            _AesEncryptionService = aesEncryptionService;
            _TimeZoneService = timeZoneService;
            _MerchantRepository = merchantRepository;
            _BankBotService = bankBotService;
            _MerchantCustomerRepository = merchantCustomerRepository;
            _CardToCardAccountRepository = cardToCardAccountRepository;
            _Logger = logger;
            _BankStatementItemRepository = bankStatementItemRepository;
            _TransactionQueueService = transactionQueueService;
            _Configuration = withdrawalOptions.Value;
            _TransactionWithdrawalRelationRepository = transactionWithdrawalRelationRepository;
            _TransactionRepository = transactionRepository;
            _CurrentUser = currentUser;
            _WithdrawalTransferHistoryRepository = withdrawalTransferHistoryRepository;
            _CardHolderNameRepository = cardHolderNameRepository;
            _MerchantCustomerManager = merchantCustomerManager;
            _WithdrawalRequestContentRepository = withdrawalRequestContentRepository;
        }

        public async Task<Withdrawal> CheckWithdrawalStatus(int id)
        {
            var withdrawal = await GetEntityByIdAsync(id);

            if (withdrawal == null)
            {
                return null;
            }

            return await CheckWithdrawalStatus(withdrawal, false);
        }

        public async Task<Withdrawal> CheckWithdrawalStatus(Withdrawal item, bool force = false, BankStatementItemDTO statement =null)
        {
            if (!item.TransferId.HasValue)
            {
                return item;
            }

            if (!force)
            {
                if (item.TransferStatus == (int)TransferStatus.Complete || item.TransferStatus == (int)TransferStatus.RefundFromBank)
                {
                    return item;
                }
            }

            var response = await _BankBotService.GetTransferRequestWithStatus(item.TransferId.Value);

            if (response != null && item.TransferStatus != response.TransferStatus)
            {
                var history = await _WithdrawalTransferHistoryRepository.GetItemAsync(t => t.TransferId == item.TransferId && t.WithdrawalId == item.Id);

                if (history != null)
                {
                    history.TransferStatus = response.TransferStatus;
                    history.TransferStatusDescription = response.TransferStatusDescription;
                    history.LastCheckDate = DateTime.UtcNow;

                    await _WithdrawalTransferHistoryRepository.UpdateAsync(history);
                    await _WithdrawalTransferHistoryRepository.SaveChangesAsync();
                }
            }

            if (response == null || response.TransferStatus == (int)TransferStatus.Cancelled)
            {
                item.TransferStatusDescription = "Cancelled By System";
                item.WithdrawalStatus = (int)WithdrawalStatus.CancelledBySystem;
                item.TransferStatus = response == null ? (int)TransferStatus.Cancelled : response.TransferStatus;
                item.UpdateDate = DateTime.UtcNow;

                decimal? paid = GetPaidAmount(item);

                if (paid.HasValue && paid.Value > 0)
                {
                    item.RemainingAmount = item.Amount - paid.Value;
                    item.WithdrawalStatus = (int)WithdrawalStatus.PartialPaid;
                    item.TransferStatusDescription = $"Partial Paid From Cancel {paid}";
                }

                item = await UpdateAsync(item);
                await SaveAsync();
            }
            else if (response.TransferStatus == (int)TransferStatus.RefundFromBank)
            {
                decimal? paid = GetPaidAmount(item);

                if (paid.HasValue && paid.Value > 0)
                {
                    item.TransferStatus = response.TransferStatus;
                    item.RemainingAmount = item.Amount - paid.Value;
                    item.WithdrawalStatus = (int)WithdrawalStatus.PartialPaid;
                    item.TransferStatusDescription = "Partial Paid";
                }
                else
                {
                    item.TransferStatusDescription = response.TransferStatusDescription;
                    item.TransferStatus = response.TransferStatus;
                    item.WithdrawalStatus = (int)WithdrawalStatus.Refunded;
                    item.RemainingAmount = response.TransferBalance;
                }

                item = await UpdateAsync(item);
                await SaveAsync();
            }
            else
            {
                if (response.TransferStatus != item.TransferStatus || statement != null)
                {
                    item.TransferStatus = response.TransferStatus;
                    item.TransferStatusDescription = response.TransferStatusDescription;
                    item.TransferDate = DateTime.UtcNow;
                    item.TransferRequestDate = response.TransferRequestDate;
                    item.TransferRequestGuid = response.TransferRequestGuid;
                    if (statement != null)
                    {
                        item.BankStatementItemId = statement.RecordId;
                        item.WithdrawalStatus = (int)WithdrawalStatus.Confirmed;
                        item.RemainingAmount = 0;

                        var statementItem = await _BankStatementItemRepository.GetEntityByIdAsync(statement.Id);

                        statementItem.WithdrawalId = item.Id;
                        await _BankStatementItemRepository.UpdateAsync(statementItem);
                        await _BankStatementItemRepository.SaveChangesAsync();
                    }
                    item.UpdateDate = DateTime.UtcNow;

                    if (item.TransferStatus == (int)TransferStatus.Complete || item.TransferStatus == (int)TransferStatus.AwaitingConfirmation)
                    {
                        try
                        {
                            var history = await _BankBotService.GetTransferHistory(response.Id);

                            if (history != null)
                            {
                                item.TrackingNumber = history.TrackingNumber;
                            }
                        }
                        catch (Exception ex)
                        {
                            _Logger.LogError(ex, ex.Message);
                        }
                    }

                    item = await UpdateAsync(item);
                    await SaveAsync();


                }
            }

            return item;
        }

        private decimal? GetPaidAmount(Withdrawal item)
        {
            return _TransactionRepository.GetQuery(t => t.WithdrawalId == item.Id).Sum(p => (decimal?)p.TransactionAmount);
        }

        public async Task<Withdrawal> Retry(int id, Transaction transaction)
        {
            var withdraw = await CheckWithdrawalStatus(id);

            if (withdraw == null)
            {
                throw new WithdrawException(WithdrawRequestResult.WithdrawlIdIsNotValid);
            }
            List<Transaction> items = new List<Transaction>();

            if (transaction == null)
            {
                items = _TransactionRepository.GetQuery(t => t.WithdrawalId == id && t.Status == (int)TransactionStatus.WaitingConfirmation).ToList();
                if (withdraw.WithdrawalStatus == (int)WithdrawalStatus.CancelledBySystem || (withdraw.WithdrawalStatus == (int)WithdrawalStatus.CancelledByUser))
                {
                    throw new Exception("This withdrawal is cancelled");
                }

                if (withdraw.WithdrawalStatus == (int)WithdrawalStatus.PendingCardToCardConfirmation && withdraw.UpdateDate.HasValue && DateTime.UtcNow.Subtract(withdraw.UpdateDate.Value).TotalHours < 24)
                {
                    throw new Exception("You should wait 24 hours to retry this withdrawal");
                }

                if (withdraw.TransferId.HasValue)
                {
                    await _BankBotService.CancelTransferRequest(withdraw.TransferId.Value);
                }
            }
            else
            {
                items.Add(transaction);
            }

            decimal amount = 0;

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                amount += item.TransactionAmount;

                item.WithdrawalId = null;
                item.TransactionStatus = TransactionStatus.Expired;

                await _TransactionRepository.UpdateAsync(item);
                await _TransactionRepository.SaveChangesAsync();
            }

            decimal? paid = GetPaidAmount(withdraw);

            var anyPending = _TransactionRepository.GetQuery(t => t.WithdrawalId == withdraw.Id && t.Status == (int)TransactionStatus.WaitingConfirmation).Any();


            if (!anyPending)
            {
                if (paid.HasValue && paid.Value > 0)
                {
                    //withdraw.RemainingAmount = withdraw.Amount - paid.Value;
                    withdraw.WithdrawalStatus = (int)WithdrawalStatus.PartialPaid;
                    withdraw.TransferStatusDescription = "Partial Paid - Reactivated";
                }
                else
                {
                    withdraw.WithdrawalStatus = (int)WithdrawalStatus.Pending;
                    withdraw.TransferStatusDescription = "Reactivated";
                }
            }

            withdraw.RemainingAmount += amount;
            withdraw.TransferStatus = (int)TransferStatus.Incomplete;
            withdraw.AccountGuid = null;
            withdraw.BankStatementItemId = null;
            withdraw.FromAccountNumber = null;
            withdraw.TrackingNumber = null;
            withdraw.TransferAccountId = 0;
            withdraw.TransferDate = null;
            withdraw.TransferId = null;
            withdraw.TransferNotes = null;
            withdraw.TransferRequestDate = null;
            withdraw.TransferRequestGuid = null;
            withdraw.UpdateDate = DateTime.UtcNow;


            await UpdateAsync(withdraw);
            await SaveAsync();

            await _TransactionQueueService.InsertWithdrawalCallbackQueueItem(new WithdrawalQueueItem()
            {
                Id = withdraw.Id,
                LastTryDateTime = null,
                TenantGuid = withdraw.TenantGuid,
                TryCount = 0
            });

            return withdraw;
        }

        public async Task<Withdrawal> GetOldItem(int merchantId, string reference)
        {
            return await Repository.GetItemAsync(t => t.MerchantId == merchantId && !string.IsNullOrEmpty(t.Reference) && t.Reference == reference);
        }

        public async Task<List<Withdrawal>> GetUncompletedWithdrawals(DateTime date)
        {
            var merchantQuery = _MerchantRepository.GetQuery();

            return await Repository.GetItemsAsync(
                t => t.TransferId.HasValue && t.TransferStatus != (int)TransferStatus.Complete
                && t.TransferStatus != (int)TransferStatus.RefundFromBank
                && t.TransferStatus != (int)TransferStatus.RejectedDueToBlokedAccount
                && t.TransferStatus != (int)TransferStatus.FailedFromBank
                && t.TransferStatus != (int)TransferStatus.Invalid
                && t.TransferStatus != (int)TransferStatus.InvalidIBANNumber
                && t.TransferStatus != (int)TransferStatus.AccountNumberInvalid
                && t.TransferStatus != (int)TransferStatus.Cancelled
                && t.WithdrawalStatus != (int)WithdrawalStatus.Refunded
                && t.WithdrawalStatus == (int)WithdrawalStatus.Sent
                && t.TransferRequestDate >= date
                && merchantQuery.Select(p => p.Id).Contains(t.MerchantId));
        }

        public async Task ConfirmWithdrawals(DateTime startDate)
        {
            var query = Repository.GetQuery(t => t.TransferId.HasValue && t.TransferStatus == (int)TransferStatus.Complete && t.WithdrawalStatus == (int)WithdrawalStatus.Sent && t.TransferDate >= startDate);

            var statementRepository = _BankStatementItemRepository.GetQuery();

            int normal = (int)TransferType.Normal;
            int paya = (int)TransferType.Paya;

            var items = (from w in query
                         join s in statementRepository on w.AccountGuid equals s.AccountGuid
                         where w.RemainingAmount == s.Debit && ((w.TransferType == normal && w.TransferNotes == s.Notes) || (w.TransferType == paya && s.Description.Contains(w.TrackingNumber)))
                         select new
                         {
                             Withdrawal = w,
                             Statement = s
                         }).ToList();

            for (int i = 0; i < items.Count; i++)
            {
                var s = items[i].Statement;
                var withdrawal = items[i].Withdrawal;

                withdrawal.BankStatementItemId = s.Id;
                withdrawal.WithdrawalStatus = (int)WithdrawalStatus.Confirmed;
                withdrawal.RemainingAmount = 0;
                withdrawal.UpdateDate = DateTime.UtcNow;

                await _TransactionQueueService.InsertWithdrawalCallbackQueueItem(new WithdrawalQueueItem()
                {
                    Id = withdrawal.Id,
                    LastTryDateTime = null,
                    TenantGuid = withdrawal.TenantGuid,
                    TryCount = 0
                });

                s.WithdrawalId = withdrawal.Id;

                await UpdateAsync(withdrawal);
                await _BankStatementItemRepository.UpdateAsync(s);
                await SaveAsync();
                await _BankStatementItemRepository.SaveChangesAsync();

                //await _MerchantCustomerManager.WithdrawalConfirmed(withdrawal.MerchantCustomerId, withdrawal.Amount);
            }

            query = Repository.GetQuery(t => t.TransferId.HasValue && t.TransferStatus == (int)TransferStatus.Complete && t.WithdrawalStatus == (int)WithdrawalStatus.Sent && t.TransferDate < startDate);

            var withdrawals = query.ToList();

            for (int i = 0; i < withdrawals.Count; i++)
            {
                var withdrawal = withdrawals[i];

                withdrawal.WithdrawalStatus = (int)WithdrawalStatus.Confirmed;
                withdrawal.RemainingAmount = 0;
                withdrawal.UpdateDate = DateTime.UtcNow;

                await _TransactionQueueService.InsertWithdrawalCallbackQueueItem(new WithdrawalQueueItem()
                {
                    Id = withdrawal.Id,
                    LastTryDateTime = null,
                    TenantGuid = withdrawal.TenantGuid,
                    TryCount = 0
                });;

                await UpdateAsync(withdrawal);
                await SaveAsync();
            }
        }

        public async Task<List<Withdrawal>> GetUnprocessedWithdrawals()
        {
            var merchantQuery = _MerchantRepository.GetQuery();

            var relationQuery = _TransactionWithdrawalRelationRepository.GetQuery();
            var types = new int[] { (int)WithdrawalProcessType.Transfer, (int)WithdrawalProcessType.Both };
            var date = DateTime.UtcNow;
            var totalMinutes = _Configuration.CardToCardDeadline.TotalMinutes;

            return await Repository.GetItemsAsync(t =>
                                                    t.TransferId == null
                                                    && t.TransferRequestDate == null
                                                    && t.TransferStatus != (int)TransferStatus.Cancelled
                                                    && (t.WithdrawalStatus == (int)WithdrawalStatus.Pending || t.WithdrawalStatus == (int)WithdrawalStatus.PendingBalance || t.WithdrawalStatus == (int)WithdrawalStatus.PartialPaid)
                                                    && t.RemainingAmount > 0
                                                    && (t.ExpectedTransferDate == null || t.ExpectedTransferDate <= DateTime.UtcNow)
                                                    && !relationQuery.Any(p => p.WithdrawalId == t.Id)
                                                    && (t.WithdrawalProcessType == (int)WithdrawalProcessType.Transfer || t.CardToCardTryCount >= _Configuration.MaxCardToCardTryCount || t.ExpectedTransferDate.AddMinutes(totalMinutes) <= date)
                                                    && types.Contains(t.WithdrawalProcessType)
                                                    && merchantQuery.Select(p => p.Id).Contains(t.MerchantId));
        }

        public async Task<List<PendingWithdrawalAmount>> GetPendingWithdrawalAmounts()
        {
            var query = Repository.GetQuery();
            var merchantQuery = _MerchantRepository.GetQuery();

            var pendingWithdrawalQuery = (from w in query
                                          join m in merchantQuery on w.MerchantId equals m.Id
                                          where w.AccountGuid != null
                                          && w.TransferStatus != (int)TransferStatus.Complete
                                          && w.TransferStatus != (int)TransferStatus.AwaitingConfirmation
                                          && w.TransferStatus != (int)TransferStatus.FailedFromBank
                                          && w.TransferStatus != (int)TransferStatus.Invalid
                                          && w.TransferStatus != (int)TransferStatus.RefundFromBank
                                          && w.TransferStatus != (int)TransferStatus.CompletedWithNoReceipt
                                          && w.TransferStatus != (int)TransferStatus.BankSubmitted
                                          && w.TransferStatus != (int)TransferStatus.InvalidIBANNumber
                                          && w.TransferStatus != (int)TransferStatus.AccountNumberInvalid
                                          && w.TransferStatus != (int)TransferStatus.Cancelled
                                          && (w.WithdrawalStatus == (int)WithdrawalStatus.Sent)
                                          && w.TransferId.HasValue
                                          group w.RemainingAmount by new { w.AccountGuid, w.TransferType } into g
                                          select new PendingWithdrawalAmount()
                                          {
                                              AccountGuid = g.Key.AccountGuid,
                                              TransferType = g.Key.TransferType,
                                              Amount = (decimal?)g.Sum()
                                          });

            return pendingWithdrawalQuery.ToList();
        }

        public async Task<List<PendingWithdrawalAmount>> GetPendingWithdrawalBalance()
        {
            var query = Repository.GetQuery();
            var merchantQuery = _MerchantRepository.GetQuery();

            var statuses = new int[]
            {
                (int)TransferStatus.AwaitingConfirmation,
                (int)TransferStatus.Pending,
                (int)TransferStatus.InsufficientBalance
            };

            var pendingWithdrawalQuery = (from w in query
                                          join m in merchantQuery on w.MerchantId equals m.Id
                                          where w.AccountGuid != null
                                          && statuses.Contains(w.TransferStatus)
                                          && (w.WithdrawalStatus == (int)WithdrawalStatus.Sent)
                                          && w.TransferId.HasValue
                                          group w.RemainingAmount by new { w.AccountGuid } into g
                                          select new PendingWithdrawalAmount()
                                          {
                                              AccountGuid = g.Key.AccountGuid,
                                              Amount = (decimal?)g.Sum()
                                          });

            return pendingWithdrawalQuery.ToList();
        }

        public async Task<ListSearchResponse<IEnumerable<WithdrawalSearchDTO>>> Search(WithdrawalSearchArgs args)
        {
            IQueryable<WithdrawalSearchDTO> itemQuery = await GetSearchQuery(args);

            var totalCount = await Repository.GetModelCountAsync(itemQuery);

            List<WithdrawalSearchDTO> items = null;

            itemQuery = itemQuery.Skip(args.StartRow).Take(args.EndRow);

            items = await Repository.GetModelItemsAsync(itemQuery);

            var accounts = await _BankBotService.GetAccountsAsync();

            var logins = await _BankBotService.GetLoginSelect();

            var requestDates = new List<DateTime>();

            var transferDates = new List<DateTime>();

            var expectedDates = new List<DateTime>();

            var cancelDates = new List<DateTime>();

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                requestDates.Add(item.TransferRequestDate ?? DateTime.UtcNow);

                transferDates.Add(item.TransferDate ?? DateTime.UtcNow);

                cancelDates.Add(item.CancelDate ?? DateTime.UtcNow);

                expectedDates.Add(item.ExpectedTransferDate);
            }

            List<Task> tasks = new List<Task>();

            string calendarCode = args.TimeZoneInfo.GetCalendarCode();

            var requestTask = _TimeZoneService.ConvertCalendarLocal(requestDates, string.Empty, calendarCode);
            var transferTask = _TimeZoneService.ConvertCalendarLocal(transferDates, string.Empty, calendarCode);
            var expectedTask = _TimeZoneService.ConvertCalendarLocal(expectedDates, string.Empty, calendarCode);
            var cancelTask = _TimeZoneService.ConvertCalendarLocal(cancelDates, string.Empty, calendarCode);

            await Task.WhenAll(requestTask, transferTask, expectedTask, cancelTask);

            var index = 0;

            items.ForEach(item =>
            {
                if (item.TransferRequestDate.HasValue)
                {
                    item.TransferRequestDateStr = requestTask.Result[index];
                }

                if (item.TransferDate != null)
                {
                    item.TransferDateStr = transferTask.Result[index];
                }

                if (item.CancelDate.HasValue)
                {
                    item.CancelDateStr = cancelTask.Result[index];
                }

                item.ExpectedTransferDateStr = expectedTask.Result[index];

                if (!string.IsNullOrEmpty(item.FromAccountNumber))
                {
                    item.FromAccountNumber = _AesEncryptionService.DecryptToString(item.FromAccountNumber);
                }

                if (!string.IsNullOrEmpty(item.ToAccountNumber))
                {
                    item.ToAccountNumber = _AesEncryptionService.DecryptToString(item.ToAccountNumber);
                }

                item.ToIbanNumber = _AesEncryptionService.DecryptToString(item.ToIbanNumber);

                if (!string.IsNullOrEmpty(item.FirstName))
                {
                    item.FirstName = _AesEncryptionService.DecryptToString(item.FirstName);
                }

                if (!string.IsNullOrEmpty(item.LastName))
                {
                    item.LastName = _AesEncryptionService.DecryptToString(item.LastName);
                }

                if (!string.IsNullOrEmpty(item.FriendlyName))
                {
                    item.FriendlyName = _AesEncryptionService.DecryptToString(item.FriendlyName);
                }

                item.CardNumber = _AesEncryptionService.DecryptToString(item.CardNumber);

                var account = accounts.FirstOrDefault(t => t.AccountGuid == item.AccountGuid);

                if (account != null)
                {
                    item.BankLoginId = account.LoginId;
                    item.BankAccountId = account.Id;
                }

                if (item.BankLoginId.HasValue && item.BankLoginId > 0)
                {
                    var login = logins.FirstOrDefault(t => t.Id == item.BankLoginId);

                    if (login != null)
                    {
                        item.IsLoginBlocked = login.IsBlocked;
                    }
                }

                index++;
            });

            return new ListSearchResponse<IEnumerable<WithdrawalSearchDTO>>()
            {
                Items = items.AsEnumerable(),
                Success = true,
                Paging = new PagingHeader(totalCount, 0, 0, 0)
            };
        }

        private async Task<IQueryable<WithdrawalSearchDTO>> GetSearchQuery(WithdrawalSearchArgs args)
        {
            var query = Repository.GetQuery();

            var customerQuery = _MerchantCustomerRepository.GetQuery();

            var cardToCardQuery = _CardToCardAccountRepository.GetQuery();

            int status = (int)TransactionStatus.WaitingConfirmation;

            int mobile = (int)PaymentType.Mobile;

            var cardHolderNameQuery = _CardHolderNameRepository.GetQuery();

            var transactionQuery = _TransactionRepository.GetQuery();

            var transactionGroupQuery = (from q in transactionQuery
                                         where q.WithdrawalId.HasValue && q.Status == status && q.PaymentType == mobile
                                         group q.TransactionAmount by q.WithdrawalId into gr
                                         select new
                                         {
                                             WithdrawalId = gr.Key,
                                             TransactionAmount = (decimal?)gr.Sum()
                                         });

            if (args.MerchantCustomerId.HasValue)
            {
                query = query.Where(t => t.MerchantCustomerId == args.MerchantCustomerId);
            }

            //if (args.Id.HasValue)
            //{
            //    query = query.Where(t => t.Id == args.Id);
            //}

            if (args.Merchants != null && args.Merchants.Length > 0)
            {
                query = query.Where(t => args.Merchants.Contains(t.MerchantId));
            }

            if (args.Status.HasValue && args.Status != 0)
            {
                if (args.Status == (int)WithdrawalStatusSearch.Pending)
                {
                    query = query.Where(t => t.WithdrawalStatus == (int)WithdrawalStatus.Pending || t.WithdrawalStatus == (int)WithdrawalStatus.PendingBalance);
                }
                else if (args.Status == (int)WithdrawalStatusSearch.PendingApproval)
                {
                    query = query.Where(t => t.WithdrawalStatus == (int)WithdrawalStatus.PendingCardToCardConfirmation);
                }
                else if (args.Status == (int)WithdrawalStatusSearch.PartialPaid)
                {
                    query = query.Where(t => t.WithdrawalStatus == (int)WithdrawalStatus.PartialPaid);
                }
                else if (args.Status == (int)WithdrawalStatusSearch.Cancelled)
                {
                    query = query.Where(t => t.WithdrawalStatus == (int)WithdrawalStatus.CancelledBySystem || t.WithdrawalStatus == (int)WithdrawalStatus.CancelledByUser);
                }
                else if (args.Status == (int)WithdrawalStatusSearch.Sent)
                {
                    query = query.Where(t => t.WithdrawalStatus == (int)WithdrawalStatus.Sent && t.TransferStatus != (int)TransferStatus.Complete);
                }
                else if (args.Status == (int)WithdrawalStatusSearch.Transfered)
                {
                    query = query.Where(t => (t.TransferStatus == (int)TransferStatus.Complete || t.TransferStatus == (int)TransferStatus.CompletedWithNoReceipt) && t.WithdrawalStatus != (int)WithdrawalStatus.Confirmed);
                }
                else if (args.Status == (int)WithdrawalStatusSearch.Confirmed)
                {
                    query = query.Where(t => t.WithdrawalStatus == (int)WithdrawalStatus.Confirmed);
                }
                else if (args.Status == (int)WithdrawalStatusSearch.Refund)
                {
                    query = query.Where(t => t.WithdrawalStatus == (int)WithdrawalStatus.Refunded);
                }
            }


            var itemQuery = (from t in query
                             join mc in customerQuery on t.MerchantCustomerId equals mc.Id into customers
                             from customer in customers.DefaultIfEmpty()
                             join ctc in cardToCardQuery on t.TransferAccountId equals ctc.Id into cards
                             from c in cards.DefaultIfEmpty()
                             join tr in transactionGroupQuery on t.Id equals tr.WithdrawalId into transactions
                             from tra in transactions.DefaultIfEmpty()
                             join ch in cardHolderNameQuery on t.CardNumber equals ch.CardNumber into cardNames
                             from chn in cardNames.DefaultIfEmpty()
                             select new WithdrawalSearchDTO()
                             {
                                 Id = t.Id,
                                 TransferStatus = t.TransferStatus,
                                 Amount = t.Amount,
                                 TransferRequestDate = t.TransferRequestDate,
                                 TransferDate = t.TransferDate,
                                 FromAccountNumber = t.FromAccountNumber,
                                 ToAccountNumber = t.ToAccountNumber,
                                 Priority = t.Priority,
                                 TransferNotes = t.TransferNotes,
                                 TransferType = t.TransferType,
                                 TenantGuid = t.TenantGuid,
                                 FirstName = t.FirstName,
                                 LastName = t.LastName,
                                 ExpectedTransferDate = t.ExpectedTransferDate,
                                 MerchantId = t.MerchantId,
                                 OwnerGuid = t.OwnerGuid,
                                 Reference = t.Reference,
                                 ToIbanNumber = t.ToIbanNumber,
                                 TransferStatusDescription = t.TransferStatusDescription,
                                 UserId = customer.UserId,
                                 WebsiteName = customer.WebsiteName,
                                 WithdrawalStatus = t.WithdrawalStatus,
                                 CreateDate = t.CreateDate,
                                 CancelDate = t.CancelDate,
                                 FriendlyName = c.CardHolderName,
                                 TrackingNumber = t.TrackingNumber,
                                 Description = t.Description,
                                 MerchantCustomerId = t.MerchantCustomerId,
                                 BankStatementItemId = t.BankStatementItemId,
                                 AccountGuid = t.AccountGuid,
                                 WithdrawalProcessType = t.WithdrawalProcessType,
                                 CardToCardTryCount = t.CardToCardTryCount,
                                 CardNumber = t.CardNumber,
                                 UpdateDate = t.UpdateDate,
                                 RemainingAmount = t.RemainingAmount,
                                 PendingApprovalAmount = tra.TransactionAmount,
                                 TransferId = t.TransferId,
                                 CardHolderName = chn.Name
                             });

            //if (!string.IsNullOrEmpty(args.UserId))
            //{
            //    itemQuery = itemQuery.Where(t => t.UserId == args.UserId);
            //}

            //if (!string.IsNullOrEmpty(args.WebsiteName))
            //{
            //    itemQuery = itemQuery.Where(t => t.WebsiteName == args.WebsiteName);
            //}

            //if (!string.IsNullOrEmpty(args.Reference))
            //{
            //    itemQuery = itemQuery.Where(t => t.Reference == args.Reference);
            //}

            //if (!string.IsNullOrEmpty(args.TrackingNumber))
            //{
            //    itemQuery = itemQuery.Where(t => t.TrackingNumber == args.TrackingNumber);
            //}

            //if (!string.IsNullOrEmpty(args.CardNumber))
            //{
            //    var encrypted = _AesEncryptionService.EncryptToBase64(args.CardNumber);

            //    itemQuery = itemQuery.Where(t => t.CardNumber == encrypted);
            //}

            if(args.FilterModel != null)
            {
                itemQuery = itemQuery.ApplyParameters(args.FilterModel, _AesEncryptionService);
            }

            DateTime? startDate = null;
            DateTime? endDate = null;

            string timeZoneCode = args.TimeZoneInfo.GetTimeZoneCode();

            var today = await _TimeZoneService.ConvertCalendar(DateTime.UtcNow, string.Empty, timeZoneCode);  //TimeZoneInfo.ConvertTime(DateTime.Now, args.TimeZoneInfo).Date;

            //today = await _TimeZoneService.ConvertCalendar(today.Date, calendarCode, "utc");

            //switch (args.DateRange)
            //{
            //    case DatePeriodType.Today:
            //        startDate = today;
            //        endDate = today.AddDays(1);
            //        break;
            //    case DatePeriodType.Yesterday:
            //        startDate = today.Yesterday();
            //        endDate = today;
            //        break;
            //    case DatePeriodType.ThisWeek:
            //        startDate = today.ThisMonday();
            //        endDate = startDate.Value.AddDays(7);
            //        break;
            //    case DatePeriodType.LastWeek:
            //        startDate = today.LastMonday();
            //        endDate = today.ThisMonday();
            //        break;
            //    case DatePeriodType.ThisMonth:
            //        startDate = today.ThisMonthStart();
            //        endDate = today.ThisMonthEnd();
            //        break;
            //    case DatePeriodType.LastMonth:
            //        startDate = today.LastMonthStart();
            //        endDate = today.ThisMonthStart();
            //        break;
            //    default:
            //        break;
            //}

            //if (startDate.HasValue)
            //{
            //    startDate = await _TimeZoneService.ConvertCalendar(startDate.Value.Date, timeZoneCode, Helper.Utc);
            //}

            //if (endDate.HasValue)
            //{
            //    endDate = await _TimeZoneService.ConvertCalendar(endDate.Value.Date, timeZoneCode, Helper.Utc);
            //}

            bool sort = false;
            if (!string.IsNullOrEmpty(args.SortColumn))
            {
                switch (args.SortColumn)
                {
                    case "amount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.Amount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.Amount);
                        }
                        break;
                    case "remainingAmount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.RemainingAmount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.RemainingAmount);
                        }
                        break;
                    case "transferStatusDescription":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TransferStatusDescription);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TransferStatusDescription);
                        }
                        break;
                    case "transferRequestDate":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TransferRequestDate);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TransferRequestDate);
                        }
                        break;
                    case "transferDate":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TransferDate);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TransferDate);
                        }
                        break;
                    case "firstName":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.FirstName);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.FirstName);
                        }
                        break;
                    case "lastName":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.LastName);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.LastName);
                        }
                        break;
                    case "expectedTransferDate":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.ExpectedTransferDate);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.ExpectedTransferDate);
                        }
                        break;
                    case "trackingNumber":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TrackingNumber);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TrackingNumber);
                        }
                        break;
                    case "reference":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.Reference);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.Reference);
                        }
                        break;
                    case "cancelDate":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.CancelDate);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.CancelDate);
                        }
                        break;
                    case "transferNotes":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TransferNotes);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TransferNotes);
                        }
                        break;
                    case "userId":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.UserId);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.UserId);
                        }
                        break;
                    case "websiteName":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.WebsiteName);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.WebsiteName);
                        }
                        break;
                    case "description":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.Description);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.Description);
                        }
                        break;
                    case "transactionId":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.TransactionId);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.TransactionId);
                        }
                        break;
                    case "cardToCardTryCount":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            itemQuery = itemQuery.OrderBy(t => t.CardToCardTryCount);
                        }
                        else
                        {
                            itemQuery = itemQuery.OrderByDescending(t => t.CardToCardTryCount);
                        }
                        break;
                    default:
                        break;
                }
            }

            if (!sort)
            {
                itemQuery = itemQuery.OrderByDescending(t => t.CreateDate);
            }

            if (startDate.HasValue)
            {
                //startDate = startDate.Value.ConvertTimeToUtc(args.TimeZoneInfo);
                itemQuery = itemQuery.Where(t => (t.ExpectedTransferDate) >= startDate);
            }

            if (endDate.HasValue)
            {
                //endDate = endDate.Value.ConvertTimeToUtc(args.TimeZoneInfo);
                itemQuery = itemQuery.Where(t => (t.ExpectedTransferDate) < endDate);
            }

            return itemQuery;
        }

        public async Task<Withdrawal> SetAsCompleted(int id)
        {
            var withdraw = await CheckWithdrawalStatus(id);

            if (withdraw == null)
            {
                throw new WithdrawException(WithdrawRequestResult.WithdrawlIdIsNotValid);
            }

            if (withdraw.WithdrawalStatus == (int)WithdrawalStatus.CancelledBySystem || (withdraw.WithdrawalStatus == (int)WithdrawalStatus.CancelledByUser))
            {
                throw new Exception("This withdrawal is cancelled");
            }

            withdraw.RemainingAmount = 0;
            withdraw.WithdrawalStatus = (int)WithdrawalStatus.Confirmed;
            withdraw.TransferStatusDescription = "Completed By User";
            withdraw.TransferStatus = (int)TransferStatus.Complete;
            withdraw.TransferDate = DateTime.UtcNow;
            withdraw.UpdateDate = DateTime.UtcNow;
            withdraw.UpdateUserId = _CurrentUser.IdentifierGuid;

            await UpdateAsync(withdraw);
            await SaveAsync();

            await _TransactionQueueService.InsertWithdrawalCallbackQueueItem(new WithdrawalQueueItem()
            {
                Id = withdraw.Id,
                LastTryDateTime = null,
                TenantGuid = withdraw.TenantGuid,
                TryCount = 0
            });

            //await _MerchantCustomerManager.WithdrawalConfirmed(withdraw.MerchantCustomerId, withdraw.Amount);

            return withdraw;
        }

        public async Task<Withdrawal> SetAsCompleted(int id, decimal amount)
        {
            var withdraw = await CheckWithdrawalStatus(id);

            if (withdraw == null)
            {
                throw new WithdrawException(WithdrawRequestResult.WithdrawlIdIsNotValid);
            }

            if (withdraw.WithdrawalStatus == (int)WithdrawalStatus.CancelledBySystem || (withdraw.WithdrawalStatus == (int)WithdrawalStatus.CancelledByUser))
            {
                throw new Exception("This withdrawal is cancelled");
            }

            var notCompleted = _TransactionRepository.GetQuery(t => t.WithdrawalId == id && t.Status == (int)TransactionStatus.WaitingConfirmation).Any();

            bool updateCustomer = false;

            if (!notCompleted)
            {
                if (withdraw.RemainingAmount == amount)
                {
                    withdraw.WithdrawalStatus = (int)WithdrawalStatus.Confirmed;
                    withdraw.TransferStatusDescription = "Completed By Deposit";
                    withdraw.TransferStatus = (int)TransferStatus.Complete;

                    updateCustomer = true;
                }
                else
                {
                    withdraw.WithdrawalStatus = (int)WithdrawalStatus.PartialPaid;
                    withdraw.TransferStatusDescription = "Partial Paid By Deposit";
                }
            }

            withdraw.CardToCardTryCount = 0;
            withdraw.RemainingAmount -= amount;
            withdraw.TransferDate = DateTime.UtcNow;
            withdraw.UpdateDate = DateTime.UtcNow;
            withdraw.UpdateUserId = _CurrentUser.IdentifierGuid;

            await UpdateAsync(withdraw);
            await SaveAsync();

            await _TransactionQueueService.InsertWithdrawalCallbackQueueItem(new WithdrawalQueueItem()
            {
                Id = withdraw.Id,
                LastTryDateTime = null,
                TenantGuid = withdraw.TenantGuid,
                TryCount = 0
            });

            //if (updateCustomer)
            //{
            //    await _MerchantCustomerManager.WithdrawalConfirmed(withdraw.MerchantCustomerId, withdraw.Amount);
            //}

            return withdraw;
        }

        public async Task<Withdrawal> ChangeProcessType(int id, int processType)
        {
            var withdraw = await CheckWithdrawalStatus(id);

            if (withdraw == null)
            {
                throw new WithdrawException(WithdrawRequestResult.WithdrawlIdIsNotValid);
            }

            if (withdraw.WithdrawalStatus != (int)WithdrawalStatus.Pending)
            {
                throw new Exception("This withdrawal status is invalid for this operation");
            }

            if (!Enum.IsDefined(typeof(WithdrawalProcessType), processType))
            {
                throw new Exception("Withdrawal process type is not valid");
            }

            withdraw.WithdrawalProcessType = processType;
            withdraw.UpdateDate = DateTime.UtcNow;

            await UpdateAsync(withdraw);
            await SaveAsync();

            await _TransactionQueueService.InsertWithdrawalCallbackQueueItem(new WithdrawalQueueItem()
            {
                Id = withdraw.Id,
                LastTryDateTime = null,
                TenantGuid = withdraw.TenantGuid,
                TryCount = 0
            });

            return withdraw;
        }

        public async Task ChangeProcessType(WithdrawalSearchArgs args, int processType)
        {
            if (!Enum.IsDefined(typeof(WithdrawalProcessType), processType))
            {
                throw new Exception("Withdrawal process type is not valid");
            }

            var query = await GetSearchQuery(args);

            var statuses = new int[] { (int)WithdrawalStatus.Pending, (int)WithdrawalStatus.PendingBalance, (int)WithdrawalStatus.PartialPaid };

            var items = query.Where(t => statuses.Contains(t.WithdrawalStatus) && t.WithdrawalProcessType != processType && !t.TransferId.HasValue).Select(t => t.Id).ToList();

            for (int i = 0; i < items.Count; i++)
            {
                var itemId = items[i];

                var withdraw = await Repository.GetEntityByIdAsync(itemId);

                withdraw.WithdrawalProcessType = processType;
                withdraw.UpdateDate = DateTime.UtcNow;

                await UpdateAsync(withdraw);
                await SaveAsync();

                await _TransactionQueueService.InsertWithdrawalCallbackQueueItem(new WithdrawalQueueItem()
                {
                    Id = withdraw.Id,
                    LastTryDateTime = null,
                    TenantGuid = withdraw.TenantGuid,
                    TryCount = 0
                });
            }
        }

        public async Task<List<WithdrawalTransferHistoryDTO>> GetWithdrawalTransferHistories(int withdrawalId)
        {
            var items = await _WithdrawalTransferHistoryRepository.GetItemsAsync(t => t.WithdrawalId == withdrawalId);

            return items.Select(t => new WithdrawalTransferHistoryDTO()
            {
                Amount = t.Amount,
                Id = t.Id,
                LastCheckDate = t.LastCheckDate,
                RequestedDate = t.RequestedDate,
                TransferId = t.TransferId,
                TransferNotes = t.TransferNotes,
                TransferStatus = t.TransferStatus,
                TransferStatusDescription = t.TransferStatusDescription,
                WithdrawalId = t.WithdrawalId
            }).ToList();
        }

        public async Task<WithdrawalTransferHistoryDTO> GetCompletedWithdrawalTransferHistory(int withdrawalId)
        {
            var t = await _WithdrawalTransferHistoryRepository.GetItemAsync(p => p.WithdrawalId == withdrawalId && p.TransferStatus == (int)TransferStatus.Complete);

            if(t == null)
            {
                return null;
            }

            return new WithdrawalTransferHistoryDTO()
            {
                Amount = t.Amount,
                Id = t.Id,
                LastCheckDate = t.LastCheckDate,
                RequestedDate = t.RequestedDate,
                TransferId = t.TransferId,
                TransferNotes = t.TransferNotes,
                TransferStatus = t.TransferStatus,
                TransferStatusDescription = t.TransferStatusDescription,
                WithdrawalId = t.WithdrawalId
            };
        }

        public async Task InsertContentAsync(Withdrawal item)
        {
            await _WithdrawalRequestContentRepository.InsertAsync(new WithdrawalRequestContent()
            {
                WithdrawalId = item.Id,
                RequestContent = item.RequestContent
            });

            await _WithdrawalRequestContentRepository.SaveChangesAsync();
        }

        public async Task<string> WithdrawalCallbackToMerchant(int id, Transaction transaction)
        {
            var withdraw = await CheckWithdrawalStatus(id);

            if (withdraw != null)
            {
                await _TransactionQueueService.InsertWithdrawalCallbackQueueItem(new WithdrawalQueueItem()
                {
                    Id = withdraw.Id,
                    LastTryDateTime = null,
                    TenantGuid = withdraw.TenantGuid,
                    TryCount = 0
                });
                return "Callback added in a queue.";
            }
            return "";
        }
    }
}
