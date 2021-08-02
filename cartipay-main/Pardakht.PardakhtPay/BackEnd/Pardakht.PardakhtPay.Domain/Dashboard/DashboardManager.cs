using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Domain.Dashboard
{
    /// <summary>
    /// Represents a class which implements <see cref="IDashboardManager"/> interface to manage dashboard operations
    /// </summary>
    public class DashboardManager : IDashboardManager
    {
        IDashboardWidgetFactory _WidgetFactory = null;
        IOwnerBankLoginManager _OwnerBankLoginManager;
        IBankBotService _BankBotService;
        ICardToCardAccountManager _CardToCardAccountManager = null;
        ITimeZoneService _TimeZoneService = null;
        ITransactionRepository _TransactionRepository = null;
        IMerchantRepository _MerchantRepository = null;
        IWithdrawalRepository _WithdrawalRepository = null;
        IWithdrawalTransferHistoryRepository _WithdrawalTransferHistoryRepository = null;

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="factory"></param>
        public DashboardManager(IDashboardWidgetFactory factory,
            IOwnerBankLoginManager ownerBankLoginManager,
            IBankBotService bankBotService,
            ICardToCardAccountManager cardToCardAccountManager,
            ITransactionRepository transactionRepository,
            IMerchantRepository merchantRepository,
            IWithdrawalRepository withdrawalRepository,
            ITimeZoneService timeZoneService,
            IWithdrawalTransferHistoryRepository withdrawalTransferHistoryRepository)
        {
            _OwnerBankLoginManager = ownerBankLoginManager;
            _BankBotService = bankBotService;
            _CardToCardAccountManager = cardToCardAccountManager;
            _WidgetFactory = factory;
            _TimeZoneService = timeZoneService;
            _MerchantRepository = merchantRepository;
            _TransactionRepository = transactionRepository;
            _WithdrawalRepository = withdrawalRepository;
            _WithdrawalTransferHistoryRepository = withdrawalTransferHistoryRepository;
        }

        public async Task<List<DashboardAccountStatusDTO>> GetAccountStatuses(DashboardQuery query)
        {

            var ownerLogins = await _OwnerBankLoginManager.GetDailyAccountInformations(query);

            var logins = await _BankBotService.GetLoginSelect();

            var accountIds = ownerLogins.Select(t => t.AccountGuid).ToArray();

            var accounts = await _BankBotService.GetAccountsWithStatuses(accountIds);

            var banks = await _BankBotService.GetBanks();

            var items = (from l in logins
                         join o in ownerLogins on l.LoginGuid equals o.LoginGuid
                         join b in banks on l.BankId equals b.Id
                         join a in accounts on l.LoginGuid equals a.LoginGuid
                         where a.AccountGuid == o.AccountGuid
                         group a by new
                         {
                             Login = l,
                             Bank = b,
                             FriendlyName = o.FriendlyName,
                             AccountNo = a.AccountNo,
                             Status = a.StatusDescription,
                             Balance = a.Balance,
                             BlockedBalance = a.BlockedBalance,
                             TotalDepositToday = o.TotalDeposit,
                             TotalWithdrawalToday = o.TotalWithdrawal,
                             CardNo = o.CardNumber,
                             CardHolderName = o.CardHolderName,
                             PendingWithdrawalAmount = o.PendingWithdrawalAmount,
                             BankLoginId = o.BankLoginId,
                             AccountGuid = o.AccountGuid
                         } into xxGroup
                         select new DashboardAccountStatusDTO
                         {
                             BankLoginId = xxGroup.Key.BankLoginId,
                             FriendlyName = xxGroup.Key.FriendlyName,
                             IsBlocked = xxGroup.Key.Login.IsBlocked,
                             AccountNo = xxGroup.Key.AccountNo,
                             BankName = xxGroup.Key.Bank.BankName,
                             Status = xxGroup.Key.Status,
                             AccountBalance = xxGroup.Key.Balance,
                             BlockedBalance = xxGroup.Key.BlockedBalance,
                             NormalWithdrawable = xxGroup.Where(t => t.TransferType == (int)TransferType.Normal).Sum(p => p.WithdrawRemainedAmountForDay),
                             SatnaWithdrawable = xxGroup.Where(t => t.TransferType == (int)TransferType.Satna).Sum(p => p.WithdrawRemainedAmountForDay),
                             PayaWithdrawable = xxGroup.Where(t => t.TransferType == (int)TransferType.Paya).Sum(p => p.WithdrawRemainedAmountForDay),
                             TotalDepositToday = Convert.ToInt64(xxGroup.Key.TotalDepositToday),
                             TotalWithdrawalToday = Convert.ToInt64(xxGroup.Key.TotalWithdrawalToday),
                             CardNumber = xxGroup.Key.CardNo,
                             CardHolderName = xxGroup.Key.CardHolderName,
                             PendingWithdrawalAmount = xxGroup.Key.PendingWithdrawalAmount,
                             AccountGuid = xxGroup.Key.AccountGuid
                         }).OrderByDescending(t => t.TotalDepositToday).ToList();

            items.ForEach(item =>
            {
                var account = accounts.FirstOrDefault(t => t.AccountGuid == item.AccountGuid);

                if (account != null)
                {
                    item.BankAccountId = account.Id;
                }
            });

            return items;
        }

        /// <summary>
        /// Generates and returns a chart widget for given type
        /// </summary>
        /// <param name="widgetType"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<DashboardChartWidget> GetChartWidget(WidgetType widgetType, DashboardQuery query)
        {
            var builder = _WidgetFactory.GetChartWidgetBuilder(widgetType);

            return await builder.Build(query);
        }

        public async Task<List<DashboardMerchantTransactionDTO>> GetMerchantTransactionDTO(DashboardQuery query)
        {
            //return await Task.Run(() =>
            //{
            var timeZoneCode = query.TimeZoneInfo.GetTimeZoneCode();

            var currentDate = await _TimeZoneService.ConvertCalendar(DateTime.UtcNow, string.Empty, timeZoneCode);

            currentDate = currentDate.Date;

            var lastWeekMonday = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday).AddDays(-7);
            var thisWeekMonday = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday);
            var thisMonth = currentDate.AddDays(-(currentDate.Day - 1));
            var lastMonth = currentDate.AddDays(-(currentDate.Day - 1)).AddMonths(-1);

            var completed = (int)TransactionStatus.Completed;

            IQueryable<Transaction> transactionQuery = _TransactionRepository.GetQuery();
            transactionQuery = transactionQuery.Where(t => t.Status == completed);

            IQueryable<Merchant> merchantQuery = _MerchantRepository.GetQuery();

            IQueryable<Withdrawal> withdrawalQuery = _WithdrawalRepository.GetQuery();
            withdrawalQuery = withdrawalQuery.Where(t => t.WithdrawalStatus == (int)WithdrawalStatus.Confirmed);

            //if (!string.IsNullOrEmpty(query.TenantGuid))
            //{
            //    merchantQuery = merchantQuery.Where(t => t.TenantGuid == query.TenantGuid);
            //    transactionQuery = transactionQuery.Where(t => t.TenantGuid == query.TenantGuid);
            //    withdrawalQuery = withdrawalQuery.Where(t => t.TenantGuid == query.TenantGuid);
            //}

            DateTime startDate = DateTime.UtcNow;
            DateTime? endDate = null;

            switch (query.DateType)
            {
                case DatePeriodType.Today:
                    startDate = currentDate;//.ConvertTimeToUtc(query.TimeZoneInfo);
                    endDate = startDate.AddDays(1);

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);
                    endDate = await _TimeZoneService.ConvertCalendar(endDate.Value, timeZoneCode, Helper.Utc);

                    transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate && t.CreationDate < endDate);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate && t.TransferDate < endDate);
                    break;

                case DatePeriodType.Yesterday:
                    startDate = currentDate.AddDays(-1);//.ConvertTimeToUtc(query.TimeZoneInfo);
                    endDate = currentDate;//.ConvertTimeToUtc(query.TimeZoneInfo);

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);
                    endDate = await _TimeZoneService.ConvertCalendar(endDate.Value, timeZoneCode, Helper.Utc);

                    transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate && t.CreationDate < endDate);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate && t.TransferDate < endDate);
                    break;
                case DatePeriodType.ThisWeek:

                    startDate = thisWeekMonday;//.ConvertTimeToUtc(query.TimeZoneInfo);

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);

                    transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate);
                    break;
                case DatePeriodType.LastWeek:
                    startDate = lastWeekMonday;//.ConvertTimeToUtc(query.TimeZoneInfo);
                    endDate = thisWeekMonday;//.ConvertTimeToUtc(query.TimeZoneInfo);

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);
                    endDate = await _TimeZoneService.ConvertCalendar(endDate.Value, timeZoneCode, Helper.Utc);

                    transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate && t.CreationDate < endDate);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate && t.TransferDate < endDate);
                    break;
                case DatePeriodType.ThisMonth:
                    startDate = thisMonth;//.ConvertTimeToUtc(query.TimeZoneInfo);

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);

                    transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate);
                    break;
                case DatePeriodType.LastMonth:
                    startDate = lastMonth;//.ConvertTimeToUtc(query.TimeZoneInfo);
                    endDate = thisMonth;//.ConvertTimeToUtc(query.TimeZoneInfo);

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);
                    endDate = await _TimeZoneService.ConvertCalendar(endDate.Value, timeZoneCode, Helper.Utc);

                    transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate && t.CreationDate < endDate);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate && t.TransferDate < endDate);
                    break;
                case DatePeriodType.All:
                    break;
                default:
                    throw new NotImplementedException($"Merchant Transaction report widget builder has not implemented this date type : {query.DateType}");
            }

            var transactionGroupQuery = (from t in transactionQuery
                                         group t.TransactionAmount by t.MerchantId into g
                                         select new
                                         {
                                             MerchantId = g.Key,
                                             Sum = g.Sum(),
                                             Count = g.Count()
                                         });

            var withdrawalGroupQuery = (from t in withdrawalQuery
                                        group t.Amount - t.RemainingAmount by t.MerchantId into g
                                        select new
                                        {
                                            MerchantId = g.Key,
                                            Sum = g.Sum(),
                                            Count = g.Count()
                                        });

            var merchants = await _MerchantRepository.GetModelItemsAsync(merchantQuery);
            var transactions = await _TransactionRepository.GetModelItemsAsync(transactionGroupQuery);
            var withdrawals = await _WithdrawalRepository.GetModelItemsAsync(withdrawalGroupQuery);

            var items = (from m in merchants
                         join t in transactions on m.Id equals t.MerchantId into emptyTransactions
                         from t in emptyTransactions.DefaultIfEmpty()
                         join w in withdrawals on m.Id equals w.MerchantId into emptyWithdrawals
                         from w in emptyWithdrawals.DefaultIfEmpty()
                         select new DashboardMerchantTransactionDTO { Title = m.Title, TransactionSum = t == null ? 0 : t.Sum, TransactionCount = t == null ? 0 : t.Count, WithdrawalSum = w == null ? 0 : w.Sum, WithdrawalCount = w == null ? 0 : w.Count }).ToList();

            items = items.OrderByDescending(t => t.TransactionCount ?? 0).ToList();

            return items;

            //});
        }

      

        public async Task<List<DashboardPaymentTypeBreakDown>> GetTransactionByPaymentTypeReport(DashboardQuery query)
        {
            //return await Task.Run(() =>
            //{
            var timeZoneCode = query.TimeZoneInfo.GetTimeZoneCode();

            var currentDate = await _TimeZoneService.ConvertCalendar(DateTime.UtcNow, string.Empty, timeZoneCode);

            currentDate = currentDate.Date;

            var lastWeekMonday = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday).AddDays(-7);
            var thisWeekMonday = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday);
            var thisMonth = currentDate.AddDays(-(currentDate.Day - 1));
            var lastMonth = currentDate.AddDays(-(currentDate.Day - 1)).AddMonths(-1);

            var completed = (int)TransactionStatus.Completed;

            IQueryable<Transaction> transactionQuery = _TransactionRepository.GetQuery();
            transactionQuery = transactionQuery.Where(t => t.Status == completed);

            IQueryable<Merchant> merchantQuery = _MerchantRepository.GetQuery();
            IQueryable<WithdrawalTransferHistory> withdrawalTransferHistoryQuery = _WithdrawalTransferHistoryRepository.GetQuery();
            IQueryable<Withdrawal> withdrawalQuery = _WithdrawalRepository.GetQuery();
            withdrawalQuery = withdrawalQuery.Where(t => t.WithdrawalStatus == (int)WithdrawalStatus.Confirmed);


            DateTime startDate = DateTime.UtcNow;
            DateTime? endDate = null;

            switch (query.DateType)
            {
                case DatePeriodType.Today:
                    startDate = currentDate;//.ConvertTimeToUtc(query.TimeZoneInfo);
                    endDate = startDate.AddDays(1);

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);
                    endDate = await _TimeZoneService.ConvertCalendar(endDate.Value, timeZoneCode, Helper.Utc);

                    transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate && t.CreationDate < endDate);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate && t.TransferDate < endDate);
                    break;

                case DatePeriodType.Yesterday:
                    startDate = currentDate.AddDays(-1);//.ConvertTimeToUtc(query.TimeZoneInfo);
                    endDate = currentDate;//.ConvertTimeToUtc(query.TimeZoneInfo);

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);
                    endDate = await _TimeZoneService.ConvertCalendar(endDate.Value, timeZoneCode, Helper.Utc);

                    transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate && t.CreationDate < endDate);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate && t.TransferDate < endDate);
                    break;
                case DatePeriodType.ThisWeek:

                    startDate = thisWeekMonday;//.ConvertTimeToUtc(query.TimeZoneInfo);

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);

                    transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate);
                    break;
                case DatePeriodType.LastWeek:
                    startDate = lastWeekMonday;//.ConvertTimeToUtc(query.TimeZoneInfo);
                    endDate = thisWeekMonday;//.ConvertTimeToUtc(query.TimeZoneInfo);

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);
                    endDate = await _TimeZoneService.ConvertCalendar(endDate.Value, timeZoneCode, Helper.Utc);

                    transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate && t.CreationDate < endDate);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate && t.TransferDate < endDate);
                    break;
                case DatePeriodType.ThisMonth:
                    startDate = thisMonth;//.ConvertTimeToUtc(query.TimeZoneInfo);

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);

                    transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate);
                    break;
                case DatePeriodType.LastMonth:
                    startDate = lastMonth;//.ConvertTimeToUtc(query.TimeZoneInfo);
                    endDate = thisMonth;//.ConvertTimeToUtc(query.TimeZoneInfo);

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);
                    endDate = await _TimeZoneService.ConvertCalendar(endDate.Value, timeZoneCode, Helper.Utc);

                    transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate && t.CreationDate < endDate);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate && t.TransferDate < endDate);
                    break;
                case DatePeriodType.All:
                    break;
                default:
                    throw new NotImplementedException($"Merchant Transaction report widget builder has not implemented this date type : {query.DateType}");
            }

            var transactionGroupQuery = (from t in transactionQuery
                                         group t.TransactionAmount by t.PaymentType into g
                                         select new
                                         {
                                             PaymentType = g.Key,
                                             Sum = g.Sum(),
                                             Count = g.Count()
                                         });

            var withdrawalGroupQuery = (from w in withdrawalQuery join wth in withdrawalTransferHistoryQuery 
                                        on w.Id equals wth.WithdrawalId where wth.TransferStatus==1
                                        group w.Amount - w.RemainingAmount by (int)WithdrawalProcessType.CardToCard into g
                                        select new
                                        {
                                            WithdrawalProcessType = g.Key,
                                            Sum = g.Sum(),
                                            Count = g.Count()
                                        });

            var pardakhtPalWithdrawalGroupQuery = (from t in transactionQuery where t.WithdrawalId != null && t.PaymentType != (int)PaymentType.CardToCard
                                         group t.TransactionAmount by t.PaymentType into g
                                         select new
                                         {
                                             PaymentType = g.Key,
                                             Sum = g.Sum(),
                                             Count = g.Count()
                                         });

            var transactions = await _TransactionRepository.GetModelItemsAsync(transactionGroupQuery);
            var withdrawals = await _WithdrawalRepository.GetModelItemsAsync(withdrawalGroupQuery);

            List<DashboardPaymentTypeBreakDown> itemsTransactions = new List<DashboardPaymentTypeBreakDown>();
            List<DashboardPaymentTypeBreakDown> itemsWithdrawal = new List<DashboardPaymentTypeBreakDown>();
            List<DashboardPaymentTypeBreakDown> pardakhtPalWithdrawalTransactions = new List<DashboardPaymentTypeBreakDown>();
            List<DashboardPaymentTypeBreakDown> mergedList = new List<DashboardPaymentTypeBreakDown>();

            
            foreach (PaymentType paymentType in Enum.GetValues(typeof(PaymentType)))
            {
                /* Deposit count and amount */
                itemsTransactions = (from t in transactions
                         where t.PaymentType == (int)paymentType
                         select new DashboardPaymentTypeBreakDown { Title = paymentType.ToString(), TransactionSum = t == null ? 0 : t.Sum, TransactionCount = t == null ? 0 : t.Count, WithdrawalSum = 0, WithdrawalCount = 0 }).ToList();
                itemsTransactions = itemsTransactions.OrderByDescending(t => t.TransactionCount ?? 0).ToList();
                foreach (var item in itemsTransactions)
                {
                    mergedList.Add(item);
                }

                /* Withdrawal - PardakhtPal count and amount */
                pardakhtPalWithdrawalTransactions = (from t in pardakhtPalWithdrawalGroupQuery
                                                     where t.PaymentType == (int)paymentType
                                                  select new DashboardPaymentTypeBreakDown { Title = paymentType.ToString(), TransactionSum = 0, TransactionCount = 0, WithdrawalSum = t == null ? 0 : t.Sum, WithdrawalCount = t == null ? 0 : t.Count }).ToList();
                itemsTransactions = itemsTransactions.OrderByDescending(t => t.TransactionCount ?? 0).ToList();
                foreach (var item in pardakhtPalWithdrawalTransactions)
                {
                    mergedList.Add(item);
                }
            }

            /* Withdrawal - CardToCard count and amount */
            itemsWithdrawal = (from w in withdrawalGroupQuery
                        where w.WithdrawalProcessType == (int)WithdrawalProcessType.CardToCard
                                   select new DashboardPaymentTypeBreakDown { Title = WithdrawalProcessType.CardToCard.ToString(), TransactionSum = 0, TransactionCount = 0, WithdrawalSum = w == null ? 0 : w.Sum, WithdrawalCount = w == null ? 0 : w.Count }).ToList();
            itemsWithdrawal = itemsWithdrawal.OrderByDescending(w => w.WithdrawalCount ?? 0).ToList();

            foreach (var item in itemsWithdrawal)
            {
                  mergedList.Add(item);
            }

            var combinedTransactionWithdrawalList = mergedList.GroupBy(x => x.Title, x => x).Select(x => new DashboardPaymentTypeBreakDown()
            {
                Title = x.Key,
                TransactionCount = x.Sum(s => s.TransactionCount),
                TransactionSum = x.Sum(s => s.TransactionSum),
                WithdrawalCount= x.Sum(s => s.WithdrawalCount),
                WithdrawalSum= x.Sum(s => s.WithdrawalSum)
            }).ToList();

            combinedTransactionWithdrawalList.Add(new DashboardPaymentTypeBreakDown
            {
                Title = "Total",
                TransactionCount = combinedTransactionWithdrawalList.Sum(t => t.TransactionCount.Value),
                TransactionSum = combinedTransactionWithdrawalList.Sum(t => t.TransactionSum.Value),
                WithdrawalCount = combinedTransactionWithdrawalList.Sum(t => t.WithdrawalCount.Value),
                WithdrawalSum = combinedTransactionWithdrawalList.Sum(t => t.WithdrawalSum.Value)
            });
                       

            //});
            return combinedTransactionWithdrawalList;
        }


        /// <summary>
        /// Generates and returns a widget for given type
        /// </summary>
        /// <param name="widgetType"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<DashboardWidget> GetWidget(WidgetType widgetType, DashboardQuery query)
        {
            var builder = _WidgetFactory.GetWidgetBuilder(widgetType);

            return await builder.Build(query);
        }
    }
}
