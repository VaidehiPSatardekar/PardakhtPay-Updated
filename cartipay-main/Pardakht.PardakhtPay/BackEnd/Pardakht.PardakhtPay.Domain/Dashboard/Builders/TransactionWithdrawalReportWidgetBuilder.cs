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
    public class TransactionWithdrawalReportWidgetBuilder : WidgetBuilder, ITransactionWithdrawalReportWidgetBuilder
    {
        ITransactionManager _TransactionManager;
        IWithdrawalRepository _WithdrawalRepository = null;
        ITimeZoneService _TimeZoneService = null;

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="transactionManager"></param>
        /// <param name="transactionRepository"></param>
        public TransactionWithdrawalReportWidgetBuilder(ITransactionManager transactionManager,
            IWithdrawalRepository withdrawalRepository,
            ITimeZoneService timeZoneService)
        {
            _TransactionManager = transactionManager;
            _WithdrawalRepository = withdrawalRepository;
            _TimeZoneService = timeZoneService;
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
                Label = "DASHBOARD.TOTAL_WITHDRAWAL_AMOUNT",
                Count = new Dictionary<string, object>()
            };

            var completed = (int)TransactionStatus.Completed;

            var withdrawalQuery = _WithdrawalRepository.GetQuery();

            var pendingWithdrawalQuery = withdrawalQuery;

            withdrawalQuery = withdrawalQuery.Where(t => t.WithdrawalStatus == (int)WithdrawalStatus.Confirmed || t.WithdrawalStatus == (int)WithdrawalStatus.PartialPaid);

            pendingWithdrawalQuery = pendingWithdrawalQuery.Where(t => t.WithdrawalStatus == (int)WithdrawalStatus.Pending || t.WithdrawalStatus == (int)WithdrawalStatus.PendingBalance);

            DateTime? endDate = null;

            switch (query.DateType)
            {
                case DatePeriodType.Today:
                    startDate = CurrentDate;
                    endDate = startDate.AddDays(1);

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);
                    endDate = await _TimeZoneService.ConvertCalendar(endDate.Value, timeZoneCode, Helper.Utc);

                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate && t.TransferDate < endDate);
                    break;

                case DatePeriodType.Yesterday:
                    startDate = CurrentDate.AddDays(-1);
                    endDate = CurrentDate;

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);
                    endDate = await _TimeZoneService.ConvertCalendar(endDate.Value, timeZoneCode, Helper.Utc);

                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate && t.TransferDate < endDate);
                    break;
                case DatePeriodType.ThisWeek:
                    startDate = ThisWeekMonday;
                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);
                    break;
                case DatePeriodType.LastWeek:
                    startDate = LastWeekMonday;
                    endDate = ThisWeekMonday;

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);
                    endDate = await _TimeZoneService.ConvertCalendar(endDate.Value, timeZoneCode, Helper.Utc);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate && t.TransferDate < endDate);
                    break;
                case DatePeriodType.ThisMonth:
                    startDate = ThisMonth;

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);
                    break;
                case DatePeriodType.LastMonth:
                    startDate = LastMonth;
                    endDate = ThisMonth;

                    startDate = await _TimeZoneService.ConvertCalendar(startDate, timeZoneCode, Helper.Utc);
                    endDate = await _TimeZoneService.ConvertCalendar(endDate.Value, timeZoneCode, Helper.Utc);
                    withdrawalQuery = withdrawalQuery.Where(t => t.TransferDate >= startDate && t.TransferDate < endDate);
                    break;
                case DatePeriodType.All:
                    break;
                default:
                    throw new NotImplementedException($"Transaction report widget builder has not implemented this date type : {query.DateType}");
            }

            var pendingAmount = pendingWithdrawalQuery.Sum(t => (decimal?)t.RemainingAmount);
            var pendingCount = pendingWithdrawalQuery.Count();

            var withdrawalAmount = withdrawalQuery.Sum(t => (decimal?)t.Amount - t.RemainingAmount);
            var withdrawalCount = await _WithdrawalRepository.GetModelCountAsync(withdrawalQuery);

            widgetDataCore.Count.Add(query.DateType, new
            {
                withdrawal = withdrawalAmount ?? 0
                //pendingWithdrawal = pendingAmount ?? 0
            });

            widget.Data.CoreData.Add(widgetDataCore);

            widgetDataCore = new WidgetDataCore()
            {
                Label = "DASHBOARD.TOTAL_WITHDRAWAL_COUNT",
                Count = new Dictionary<string, object>()
                        {
                            {
                                query.DateType, new {                                    
                                    withdrawal = withdrawalCount.ToString()
                                  //  pendingWithdrawal = pendingCount
  
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
