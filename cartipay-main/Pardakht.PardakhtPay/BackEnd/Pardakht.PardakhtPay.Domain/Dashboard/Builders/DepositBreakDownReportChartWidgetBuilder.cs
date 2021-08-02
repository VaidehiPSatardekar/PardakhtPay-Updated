using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Pardakht.PardakhtPay.Domain.Dashboard.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models;

namespace Pardakht.PardakhtPay.Domain.Dashboard.Builders
{
    /// <summary>
    /// Represents a class which builds transaction graph widget report
    /// </summary>
    public class DepositBreakDownReportChartWidgetBuilder : ChartWidgetBuilder, IDepositBreakDownReportChartWidgetBuilder
    {
        ITransactionManager _TransactionManager;
        ITransactionRepository _TransactionRepository = null;
        IWithdrawalRepository _WithdrawalRepository = null;
        ITimeZoneService _TimeZoneService = null;

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="transactionManager"></param>
        /// <param name="transactionRepository"></param>
        public DepositBreakDownReportChartWidgetBuilder(ITransactionManager transactionManager,
            IWithdrawalRepository withdrawalRepository,
            ITransactionRepository transactionRepository,
            ITimeZoneService timeZoneService)
        {
            _TransactionManager = transactionManager;
            _TransactionRepository = transactionRepository;
            _WithdrawalRepository = withdrawalRepository;
            _TimeZoneService = timeZoneService;
        }

        /// <summary>
        /// Builds and returns dashbaord chart widget for transactions
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public override async Task<DashboardChartWidget> Build(DashboardQuery query)
        {
            base.CurrentDate = TimeZoneInfo.ConvertTime(DateTime.Now, query.TimeZoneInfo).Date;

            var widget = new DashboardChartWidget
            {
                Ranges = new Dictionary<string, string>
                {
                    {DatePeriodType.Daily, "DATERANGE.DAILY"},
                    {DatePeriodType.Weekly, "DATERANGE.WEEKLY"},
                    {DatePeriodType.Monthly, "DATERANGE.MONTHLY"},
                    {DatePeriodType.Quarterly, "DATERANGE.QUARTERLY"}
                },
                CurrentRange = query.DateType,
                Title = "TRANSACTION.GENERAL.DEPOSIT_BREAKDOWN",
                XAxis = true,
                YAxis = true,
                Gradient = false,
                Legend = false,
                ShowXAxisLabel = false,
                XAxisLabel = "Dates",
                ShowYAxisLabel = false,
                YAxisLabel = "Amount",
                Scheme = new Scheme { Domain = new List<string> { "#42BFF7", "#C6ECFD", "#C7B42C", "#AAAAAA" } },
                MainChart = new Dictionary<string, List<WidgetChartSeries>>()
            };

            var timeZoneCode = query.TimeZoneInfo.GetTimeZoneCode();

            DateTime startDate = await _TimeZoneService.ConvertCalendar(DateTime.UtcNow, string.Empty, timeZoneCode);

            startDate = startDate.Date;

            CurrentDate = startDate;

            int numberOfDays = 0;

            switch (query.DateType)
            {
                case DatePeriodType.Daily:
                    startDate = CurrentDate;
                    break;
                case DatePeriodType.Weekly:
                    startDate = ThisWeekStart;
                    numberOfDays = DaysInWeek;
                    break;
                case DatePeriodType.Monthly:
                    startDate = ThisMonthStart;
                    numberOfDays = DaysInMonth;
                    break;
                case DatePeriodType.Quarterly:
                    startDate = ThisQuarterStart;
                    numberOfDays = DaysInQuarter;
                    break;
                default:
                    break;
            }

            //startDate = startDate.ConvertTimeToUtc(query.TimeZoneInfo);

            var oldDate = startDate.Date;

            startDate = await _TimeZoneService.ConvertCalendar(startDate.Date, timeZoneCode, Helper.Utc);

            long dif = Convert.ToInt64(oldDate.Subtract(startDate).TotalSeconds);

            List<WidgetChartSeries> series = null;

            if (query.DateType == DatePeriodType.Daily)
            {
                series = GetReportDataDaily(query.TimeZoneInfo, startDate, dif);
            }
            else
            {
                series = await GetReportData(query.TimeZoneInfo, startDate, numberOfDays, dif);
            }

            widget.MainChart.Add(query.DateType, series);

            return widget;
        }

        /// <summary>
        /// Generates graph report data for transactions
        /// </summary>
        /// <param name="timeZoneInfo"></param>
        /// <param name="startDate"></param>
        /// <param name="numberOfDays"></param>
        /// <returns></returns>
        private async Task<List<WidgetChartSeries>> GetReportData(TimeZoneInfo timeZoneInfo, DateTime startDate, int numberOfDays, long dif)
        {
               
                var depositPaymentBreakdownSeries = new List<WidgetChartSeries>();

                var query = _TransactionRepository.GetQuery();

                int completed = (int)TransactionStatus.Completed;

                query = query.Where(t => t.Status == completed && t.CreationDate >= startDate
                && t.CreationDate <= startDate.AddDays(numberOfDays));

                var transactionQuery = (from q in query
                                        group q by new { CreationDate = q.CreationDate.AddSeconds(dif).Date, PaymentType = q.PaymentType } into xGroup
                                        select new
                                        {
                                            CreationDate = xGroup.Key.CreationDate,
                                            TransactionAmount = xGroup.Sum(p => p.TransactionAmount),
                                            PaymentType = xGroup.Key.PaymentType
                                        });

                var d = await _TransactionRepository.GetModelItemsAsync(transactionQuery);

            foreach (PaymentType paymentType in Enum.GetValues(typeof(PaymentType)))
            {
                var completedSeries = new List<WidgetChartSeriesData>();
                var data = d
                .Where(t => t.PaymentType == (int)paymentType)
                .GroupBy(t => t.CreationDate.Date)
                .ToDictionary(t => t.Key,
                    m => new
                    {
                        completedSum = m.Sum(p => p.TransactionAmount),
                        completedCount = m.Count()
                    });

                for (var i = 0; i < numberOfDays; i++)
                {
                    var convertedDate = startDate.ConvertTimeFromUtc(timeZoneInfo).AddDays(i);
                    var item = new WidgetChartSeriesData
                    {
                        Name = convertedDate.ToString("dd/MM"),
                        Value = data.ContainsKey(convertedDate.Date) ? data[convertedDate.Date].completedSum : 0,
                        Extra = data.ContainsKey(convertedDate.Date) ? data[convertedDate.Date].completedCount : 0
                    };
                    completedSeries.Add(item);
                }

                depositPaymentBreakdownSeries.Add(new WidgetChartSeries()
                {
                    Name = Enum.GetName(typeof(PaymentType), (int)paymentType),
                    Series = completedSeries
                });
            }

            return depositPaymentBreakdownSeries;
        }

        /// <summary>
        /// Generates graph report data for transactions
        /// </summary>
        /// <param name="timeZoneInfo"></param>
        /// <param name="startDate"></param>
        /// <param name="numberOfDays"></param>
        /// <returns></returns>
        private List<WidgetChartSeries> GetReportDataDaily(TimeZoneInfo timeZoneInfo, DateTime startDate, long dif)
        {
           
            var depositPaymentBreakdownSeries = new List<WidgetChartSeries>();

            var query = _TransactionRepository.GetQuery();

            int completed = (int)TransactionStatus.Completed;


            var data = query.Where(
                t => t.Status == completed && t.CreationDate >= startDate && t.CreationDate <= startDate.AddDays(1))
                .GroupBy(t => new { Hour = t.CreationDate.AddSeconds(dif).Hour, PaymentType = t.PaymentType }, t => t.TransactionAmount)
                .Select(t => new { Hour = t.Key.Hour, PaymentType = t.Key.PaymentType, Amount = t.Sum(), Count = t.Count() })
                .ToDictionary(t => new { t.Hour, t.PaymentType },
                    m => new HourlyDataForDeposit()
                    {
                        completedSum = m.Amount,
                        completedCount = m.Count
                    });

            foreach (PaymentType paymentType in Enum.GetValues(typeof(PaymentType)))
            {
                var completedSeries = new List<WidgetChartSeriesData>();
                //var dataByPaymentType = data.Where(t=>t == (int)paymentType)
                for (var i = 0; i < 24; i++)
                {
                    var key = data.Keys.ToList().FirstOrDefault(t => t.Hour == i && t.PaymentType == (int)paymentType);
                    HourlyDataForDeposit convertedDate = null;

                    if (key != null)
                    {
                        data.TryGetValue(new { Hour = i, PaymentType = (int)paymentType }, out convertedDate);
                    }
                    var item = new WidgetChartSeriesData
                    {
                        Name = i.ToString("00"),
                        Value = convertedDate != null ? convertedDate.completedSum : 0,
                        Extra = convertedDate != null ? convertedDate.completedSum : 0
                    };
                    completedSeries.Add(item);
                }
                        
                depositPaymentBreakdownSeries.Add(new WidgetChartSeries()
                {
                    Name = Enum.GetName(typeof(PaymentType), (int)paymentType),
                    Series = completedSeries
                });

            }

            return depositPaymentBreakdownSeries;
        }
    }

    class HourlyDataForDeposit
    {
        public decimal sum { get; set; }

        public int count { get; set; }

        public decimal completedSum { get; set; }

        public int completedCount { get; set; }

        public int paymentType { get; set; }
    }
}
