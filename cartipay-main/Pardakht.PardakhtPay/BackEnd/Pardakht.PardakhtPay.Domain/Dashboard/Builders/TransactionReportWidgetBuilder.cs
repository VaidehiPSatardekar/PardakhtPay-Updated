using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Dashboard.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Domain.Dashboard.Builders
{
    /// <summary>
    /// Represents a class which builds the transaction report widget
    /// </summary>
    public class TransactionReportWidgetBuilder : WidgetBuilder, ITransactionReportWidgetBuilder
    {
        ITransactionManager _TransactionManager;
        ITransactionRepository _TransactionRepository = null;
        ITimeZoneService _TimeZoneService = null;
        IWithdrawalRepository _WithdrawalRepository = null;

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="transactionManager"></param>
        /// <param name="transactionRepository"></param>
        public TransactionReportWidgetBuilder(ITransactionManager transactionManager,
            ITransactionRepository transactionRepository,
            ITimeZoneService timeZoneService,
            IWithdrawalRepository withdrawalRepository)
        {
            _TransactionManager = transactionManager;
            _TransactionRepository = transactionRepository;
            _TimeZoneService = timeZoneService;
            _WithdrawalRepository = withdrawalRepository;
        }

        /// <summary>
        /// Builds dashboard widget for transactions
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public override async Task<DashboardWidget> Build(DashboardQuery query)
        {
            var timeZoneCode = query.TimeZoneInfo.GetTimeZoneCode();

            DateTime startDate = await _TimeZoneService.ConvertCalendar(DateTime.UtcNow, string.Empty, timeZoneCode);

            //startDate = await _TimeZoneService.ConvertCalendar(startDate.Date, timeZoneCode, "utc");

            CurrentDate = startDate.Date;

            var widget = new DashboardWidget
            {
                Ranges = new Dictionary<string, string>
                {
                    {"dt", "DATERANGE.TODAY"},
                    {"dy", "DATERANGE.YESTERDAY"},
                    {"dtw", "DATERANGE.THIS_WEEK"},
                    {"dlw", "DATERANGE.LAST_WEEK"},
                    {"dtm", "DATERANGE.THIS_MONTH"},
                    {"dlm", "DATERANGE.LAST_MONTH"},
                    {"all", "DATERANGE.ALL"}
                },
                CurrentRange = query.DateType,
                Detail = "",
                Data = new WidgetData { CoreData = new List<WidgetDataCore>(), Extra = new List<WidgetDataCore>() }
            };

            var widgetDataCore = new WidgetDataCore()
            {
                Label = "DASHBOARD.TOTAL_TRANSACTION_AMOUNT",
                Count = new Dictionary<string, object>()
            };

            //await Task.Run(() =>
            //{
            var completed = (int)TransactionStatus.Completed;
            IQueryable<Transaction> transactionQuery = _TransactionRepository.GetQuery();
            transactionQuery = transactionQuery.Where(t => t.Status == completed);

            DateTime? endDate = null;

            var withdrawalQuery = _WithdrawalRepository.GetQuery();
            var pendingWithdrawalQuery = withdrawalQuery;

            withdrawalQuery = withdrawalQuery.Where(t => t.WithdrawalStatus == (int)WithdrawalStatus.Confirmed || t.WithdrawalStatus == (int)WithdrawalStatus.PartialPaid);
            pendingWithdrawalQuery = pendingWithdrawalQuery.Where(t => t.WithdrawalStatus == (int)WithdrawalStatus.Pending || t.WithdrawalStatus == (int)WithdrawalStatus.PendingBalance);

            switch (query.DateType)
            {
                case DatePeriodType.Today:
                    startDate = CurrentDate;
                    endDate = startDate.AddDays(1);

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);
                    endDate = await _TimeZoneService.ConvertCalendar(endDate.Value, timeZoneCode, Helper.Utc);

                    transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate && t.CreationDate < endDate);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate && t.TransferDate < endDate);
                    break;

                case DatePeriodType.Yesterday:
                    startDate = CurrentDate.AddDays(-1);
                    endDate = CurrentDate;

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);
                    endDate = await _TimeZoneService.ConvertCalendar(endDate.Value, timeZoneCode, Helper.Utc);

                    transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate && t.CreationDate < endDate);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate && t.TransferDate < endDate);
                    break;
                case DatePeriodType.ThisWeek:

                    startDate = ThisWeekMonday;

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);

                    transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate);
                    break;
                case DatePeriodType.LastWeek:
                    startDate = LastWeekMonday;
                    endDate = ThisWeekMonday;

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);
                    endDate = await _TimeZoneService.ConvertCalendar(endDate.Value, timeZoneCode, Helper.Utc);

                    transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate && t.CreationDate < endDate);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate && t.TransferDate < endDate);
                    break;
                case DatePeriodType.ThisMonth:
                    startDate = ThisMonth;

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);

                    transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate);
                    break;
                case DatePeriodType.LastMonth:
                    startDate = LastMonth;
                    endDate = ThisMonth;

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);
                    endDate = await _TimeZoneService.ConvertCalendar(endDate.Value, timeZoneCode, Helper.Utc);

                    transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate && t.CreationDate < endDate);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate && t.TransferDate < endDate);
                    break;
                case DatePeriodType.All:
                    break;
                default:
                    throw new NotImplementedException($"Transaction report widget builder has not implemented this date type : {query.DateType}");
            }
            widgetDataCore.Count.Add(query.DateType, new
            {
                transaction = ((decimal?)transactionQuery.Sum(t => t.TransactionAmount)) ?? 0,
                withdrawal = withdrawalQuery.Sum(t => (decimal?)t.Amount - t.RemainingAmount),
                pendingWithdrawal = pendingWithdrawalQuery.Sum(t => (decimal?)t.RemainingAmount)
            });

            widget.Data.CoreData.Add(widgetDataCore);

            
            var transactionCount = await _TransactionRepository.GetModelCountAsync(transactionQuery);
            var withdrawalCount = await _WithdrawalRepository.GetModelCountAsync(withdrawalQuery);
            var pendingCount = pendingWithdrawalQuery.Count();
            ////var mobileCount = await _TransactionRepository.GetModelCountAsync(transactionQuery.Where(t => t.PaymentType == (int)PaymentType.Mobile));
            ////var paymentGatewayCount = await _TransactionRepository.GetModelCountAsync(transactionQuery.Where(t => t.PaymentType > (int)PaymentType.Mobile));

            widgetDataCore = new WidgetDataCore()
            {
                Label = "DASHBOARD.TOTAL_TRANSACTION_COUNT",
                Count = new Dictionary<string, object>()
                        {
                            {
                                query.DateType, new {
                                    transaction = transactionCount.ToString(),
                                    withdrawal = withdrawalCount,
                                    pendingWithdrawal=pendingCount
                                }
                            }
                        }
            };

            widget.Data.CoreData.Add(widgetDataCore);
            //});

            return widget;
        }
    }
}
