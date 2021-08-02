using System;
using System.Collections.Generic;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Interfaces;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.ExternalServices.Queue;
using Pardakht.PardakhtPay.Shared.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class BankStatementItemManager : BaseManager<BankStatementItem, IBankStatementItemRepository>, IBankStatementItemManager
    {
        ILogger<BankStatementItemManager> _Logger;
        ITimeZoneService _TimeZoneService = null;
        CurrentUser _CurrentUser = null;
        IBankBotService _BankBotService = null;
        ITransactionRepository _TransactionRepository = null;
        ITransactionManager _TransactionManager = null;
        IAesEncryptionService _EncryptionService = null;
        ITransactionQueueService _TransactionQueueService = null;
        IRiskyKeywordRepository _RiskyKeywordRepository = null;
        IWithdrawalRepository _WithdrawalRepository = null;
        const string RefundPattern = "برگشت عمليات  انتقال از (?<srcNo>\\w+) به (?<destNo>\\w+) با کدرهگيري (?<trackingNo>\\w+)";
        const string TransferPattern = "انتقال ازانتقال از {0} به {1} با کدرهگيري";
        const string RefundPatternIz = " اصلاح سند(?<trackingNo>\\w+)تاريخ";

        public BankStatementItemManager(IBankStatementItemRepository repository,
            ILogger<BankStatementItemManager> logger,
            CurrentUser currentUser,
            IAesEncryptionService encryptionService,
            IBankBotService bankBotService,
            ITransactionManager transactionManager,
            ITransactionQueueService transactionQueueService,
            ITransactionRepository transactionRepository,
            IRiskyKeywordRepository riskyKeywordRepository,
            IWithdrawalRepository withdrawalRepository,
            ITimeZoneService timeZoneService) : base(repository)
        {
            _Logger = logger;
            _TimeZoneService = timeZoneService;
            _CurrentUser = currentUser;
            _BankBotService = bankBotService;
            _TransactionManager = transactionManager;
            _TransactionRepository = transactionRepository;
            _EncryptionService = encryptionService;
            _TransactionQueueService = transactionQueueService;
            _RiskyKeywordRepository = riskyKeywordRepository;
            _WithdrawalRepository = withdrawalRepository;
        }

        public override async Task<BankStatementItem> AddAsync(BankStatementItem item)
        {
            var result = await base.AddAsync(item);

            if (!string.IsNullOrEmpty(item.Description) && item.Debit > 0)
            {
                try
                {
                    var matches = Regex.Matches(item.Description.Trim(), RefundPatternIz.Trim());

                    if (matches.Count > 0)
                    {
                        var trackingNumber = matches[0].Groups["trackingNo"]?.Value;
                        //var srcNo = matches[0].Groups["srcNo"]?.Value;
                        //var destNo = matches[0].Groups["destNo"]?.Value;

                        if (!string.IsNullOrEmpty(trackingNumber))
                        {
                            //var transferText = string.Format(TransferPattern, srcNo, destNo);

                            //var transferText  = $"انتقال از {srcNo} به {destNo} با کدرهگيري";

                            //var transferText = $"انتقال از {srcNo} به {destNo} با کدرهگيري {trackingNumber}";

                            var statementItem = Repository.GetQuery(t => t.TransactionNo == trackingNumber && t.Credit == item.Debit && t.AccountGuid == item.AccountGuid).FirstOrDefault();

                            if (statementItem != null)
                            {
                                //var transactionNo = statementItem.TransactionNo;

                                //if (!string.IsNullOrEmpty(transactionNo))
                                //{
                                var transaction = _TransactionRepository.GetQuery(t => t.Id == statementItem.ConfirmedTransactionId && t.AccountGuid == item.AccountGuid).FirstOrDefault();

                                if (transaction != null && transaction.TransactionStatus != TransactionStatus.Reversed)
                                {
                                    transaction.TransactionStatus = TransactionStatus.Reversed;

                                    await _TransactionManager.UpdateAsync(transaction);
                                    await _TransactionManager.SaveAsync();

                                    if (!string.IsNullOrEmpty(transaction.ReturnUrl))
                                    {
                                        await _TransactionQueueService.InsertCallbackQueueItem(new CallbackQueueItem()
                                        {
                                            LastTryDateTime = null,
                                            TransactionCode = transaction.Token,
                                            TryCount = 0,
                                            TenantGuid = transaction.TenantGuid
                                        });
                                    }
                                }
                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    _Logger.LogError(ex, ex.Message);
                }
            }

            return result;
        }

        public async Task<ListSearchResponse<List<BankStatementItemSearchDTO>>> Search(BankStatementSearchArgs args)
        {
            var query = Repository.GetQuery();
            //var transactionQuery = _TransactionRepository.GetQuery();
            var riskyQuery = _RiskyKeywordRepository.GetQuery();
            //var withdrawalQuery = _WithdrawalRepository.GetQuery();

            var selectQuery = (from s in query
                               //join t in transactionQuery on s.ConfirmedTransactionId equals t.Id into emptyTransactions
                               //from tran in emptyTransactions.DefaultIfEmpty()
                               //join wd in withdrawalQuery on s.Id equals wd.BankStatementItemId into withdrawals
                               //from w in withdrawals.DefaultIfEmpty()
                               select new BankStatementItemSearchDTO()
                               {
                                   AccountGuid = s.AccountGuid,
                                   AccountId = s.AccountId,
                                   Balance = s.Balance,
                                   CheckNo = s.CheckNo,
                                   CreationDate = s.CreationDate,
                                   Credit = s.Credit,
                                   Debit = s.Debit,
                                   Description = s.Description,
                                   Id = s.Id,
                                   InsertDateTime = s.InsertDateTime,
                                   TransactionDateTime = s.TransactionDateTime,
                                   TransactionNo = s.TransactionNo,
                                   UsedDate = s.CreationDate,
                                   TransactionId = s.ConfirmedTransactionId,
                                   Notes = s.Notes,
                                   //IsRisky = riskyQuery.Any(p => s.Description.Contains(p.Keyword)),
                                   WithdrawalId = s.WithdrawalId
                               });

            var accounts = await _BankBotService.GetAccountsAsync();

            accounts = accounts.Where(t => args.AccountGuids.Contains(t.AccountGuid)).ToList();

            if (accounts.Any(t => !_CurrentUser.LoginGuids.Contains(t.LoginGuid)))
            {
                throw new UnauthorizedAccessException("You don't have permission to access these accounts");
            }

            var accountIds = accounts.Select(p => p.Id).ToList();

            selectQuery = selectQuery.Where(t => accountIds.Contains(t.AccountId));

            if (args.FilterModel != null && args.FilterModel.Count > 0)
            {
                selectQuery = selectQuery.ApplyParameters(args.FilterModel, _EncryptionService);
            }
            else if(accountIds.Count > 5)
            {
                selectQuery = selectQuery.Where(t => t.TransactionDateTime >= DateTime.UtcNow.AddDays(-15).Date);
            }

            if (args.StatementItemType.HasValue)
            {
                var statementItemType = (StatementItemTypes)args.StatementItemType;

                if (statementItemType == StatementItemTypes.Credit)
                {
                    selectQuery = selectQuery.Where(t => t.Credit > 0);
                }
                else if (statementItemType == StatementItemTypes.Debit)
                {
                    selectQuery = selectQuery.Where(t => t.Debit > 0);
                }
                else if (statementItemType == StatementItemTypes.Unconfirmed)
                {
                    selectQuery = selectQuery.Where(t => t.Credit > 0 && t.TransactionId == null);
                }
            }

            if (args.IsRisky.HasValue && args.IsRisky.Value)
            {
                selectQuery = selectQuery.Where(t => riskyQuery.Any(p => t.Description.Contains(p.Keyword)));
            }

            var totalCount = await Repository.GetModelCountAsync(selectQuery);

            bool sort = false;

            if (!string.IsNullOrEmpty(args.SortColumn))
            {
                switch (args.SortColumn)
                {
                    case "balance":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            selectQuery = selectQuery.OrderBy(t => t.Balance);
                        }
                        else
                        {
                            selectQuery = selectQuery.OrderByDescending(t => t.Balance);
                        }
                        break;
                    case "credit":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            selectQuery = selectQuery.OrderBy(t => t.Credit);
                        }
                        else
                        {
                            selectQuery = selectQuery.OrderByDescending(t => t.Credit);
                        }
                        break;
                    case "debit":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            selectQuery = selectQuery.OrderBy(t => t.Debit);
                        }
                        else
                        {
                            selectQuery = selectQuery.OrderByDescending(t => t.Debit);
                        }
                        break;
                    case "transactionDateTime":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            selectQuery = selectQuery.OrderBy(t => t.TransactionDateTime);
                        }
                        else
                        {
                            selectQuery = selectQuery.OrderByDescending(t => t.TransactionDateTime);
                        }
                        break;
                    case "transactionNo":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            selectQuery = selectQuery.OrderBy(t => t.TransactionNo);
                        }
                        else
                        {
                            selectQuery = selectQuery.OrderByDescending(t => t.TransactionNo);
                        }
                        break;
                    case "checkNo":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            selectQuery = selectQuery.OrderBy(t => t.CheckNo);
                        }
                        else
                        {
                            selectQuery = selectQuery.OrderByDescending(t => t.CheckNo);
                        }
                        break;
                    case "transactionId":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            selectQuery = selectQuery.OrderBy(t => t.TransactionId);
                        }
                        else
                        {
                            selectQuery = selectQuery.OrderByDescending(t => t.TransactionId);
                        }
                        break;
                    case "usedDate":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            selectQuery = selectQuery.OrderBy(t => t.UsedDate);
                        }
                        else
                        {
                            selectQuery = selectQuery.OrderByDescending(t => t.UsedDate);
                        }
                        break;
                    case "notes":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            selectQuery = selectQuery.OrderBy(t => t.Notes);
                        }
                        else
                        {
                            selectQuery = selectQuery.OrderByDescending(t => t.Notes);
                        }
                        break;
                    case "accountGuid":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            selectQuery = selectQuery.OrderBy(t => t.AccountGuid);
                        }
                        else
                        {
                            selectQuery = selectQuery.OrderByDescending(t => t.AccountGuid);
                        }
                        break;
                    case "withdrawalId":
                        sort = true;
                        if (string.IsNullOrEmpty(args.SortOrder) || args.SortOrder.StartsWith("a"))
                        {
                            selectQuery = selectQuery.OrderBy(t => t.WithdrawalId);
                        }
                        else
                        {
                            selectQuery = selectQuery.OrderByDescending(t => t.WithdrawalId);
                        }
                        break;
                }
            }

            if (!sort)
            {
                selectQuery = selectQuery.OrderByDescending(t => t.TransactionDateTime).ThenByDescending(t => t.TransactionNo);
            }

            selectQuery = selectQuery.Skip(args.StartRow).Take(args.EndRow - args.StartRow);

            var items = await Repository.GetModelItemsAsync(selectQuery);

            List<DateTime> dates = items.Select(t => t.TransactionDateTime).ToList();

            var usedDates = items.Select(t => t.UsedDate ?? DateTime.UtcNow).ToList();

            string calendarCode = args.TimeZoneInfo.GetCalendarCode();

            var convertedDatesTask = _TimeZoneService.ConvertCalendarLocal(dates, string.Empty, calendarCode);

            var usedConvertedDatesTask = _TimeZoneService.ConvertCalendarLocal(usedDates, string.Empty, calendarCode);

            await Task.WhenAll(convertedDatesTask, usedConvertedDatesTask);

            //return await Task.Run(() =>
            //{
            List<BankStatementItemSearchDTO> dtos = new List<BankStatementItemSearchDTO>();

            var riskyItems = riskyQuery.ToList();

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                if(item.TransactionId == 0)
                {
                    item.TransactionId = null;
                    item.UsedDate = null;
                }

                item.IsRisky = riskyItems.Any(p => !string.IsNullOrEmpty(item.Description) && item.Description.Contains(p.Keyword));

                item.TransactionDateTimeStr = convertedDatesTask.Result[i];

                if (item.UsedDate != null)
                {
                    item.UsedDateStr = usedConvertedDatesTask.Result[i];
                }

                dtos.Add(item);
            }

            return new ListSearchResponse<List<BankStatementItemSearchDTO>>()
            {
                Items = dtos,
                Success = true,
                Paging = new PagingHeader(totalCount, 0, 0, 0)
            };

            //});
        }

        public async Task UpdateStatementsWithTransaction(List<int> statementIds, int transactionId)
        {
            try
            {
                for (int i = 0; i < statementIds.Count; i++)
                {
                    var item = Repository.GetItemByRecordId(statementIds[i]);

                    if (item != null)
                    {
                        item.ConfirmedTransactionId = transactionId;
                        await UpdateAsync(item);
                    }
                }

                await SaveAsync();
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
            }
        }

        public async Task<decimal> GetTotalCreditAmount(CardToCardAccount account)
        {
            var iranianDate = await _TimeZoneService.ConvertCalendar(DateTime.UtcNow, Helper.Utc, "ir2");

            var iranianDateUtc = await _TimeZoneService.ConvertCalendar(iranianDate.Date, "ir2", Helper.Utc);

            return await Repository.GetTotalCreditAmount(account.AccountGuid, iranianDateUtc, iranianDateUtc.AddDays(1));
        }
    }
}
