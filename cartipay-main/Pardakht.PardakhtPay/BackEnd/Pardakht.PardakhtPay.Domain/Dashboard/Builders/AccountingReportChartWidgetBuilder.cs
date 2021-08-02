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
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Domain.Dashboard.Builders
{
    public class AccountingReportChartWidgetBuilder : ChartWidgetBuilder, IAccountingReportChartWidgetBuilder
    {
        ITransactionManager _TransactionManager;
        ITransactionRepository _TransactionRepository = null;
        IWithdrawalRepository _WithdrawalRepository = null;
        IBankStatementItemRepository _BankStatementItemRepository = null;
        CurrentUser _CurrentUser = null;
        ITimeZoneService _TimeZoneService = null;


        public AccountingReportChartWidgetBuilder(ITransactionManager transactionManager,
            ITransactionRepository transactionRepository,
            IBankStatementItemRepository bankStatementItemRepository,
            CurrentUser currentUser,
            IWithdrawalRepository withdrawalRepository,
            ITimeZoneService timeZoneService)
        {
            _TransactionManager = transactionManager;
            _TransactionRepository = transactionRepository;
            _WithdrawalRepository = withdrawalRepository;
            _BankStatementItemRepository = bankStatementItemRepository;
            _CurrentUser = currentUser;
            _TimeZoneService = timeZoneService;
        }

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
                Title = "ACCOUNTING.GENERAL.ACCOUNTING",
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

            await Task.Run(() =>
            {
                if (query.DateType == DatePeriodType.Daily)
                {
                    series = GetReportDataDaily(query.TimeZoneInfo, startDate, dif);
                }
                else
                {
                    series = GetReportData(query.TimeZoneInfo, startDate, numberOfDays, dif);
                }
            });

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
        private List<WidgetChartSeries> GetReportData(TimeZoneInfo timeZoneInfo, DateTime startDate, int numberOfDays, long dif)
        {
            var series = new List<WidgetChartSeriesData>();

            var completedSeries = new List<WidgetChartSeriesData>();

            var statementQuery = _BankStatementItemRepository.GetQuery(t => _CurrentUser.LoginGuids.Contains(t.LoginGuid)
                && t.TransactionDateTime >= startDate
                && t.TransactionDateTime < startDate.AddDays(numberOfDays)).Select(t => new { t.TransactionDateTime, t.Credit, t.Debit });

            var transactionQuery = (from q in statementQuery
                                    group q by new { TransactionDateTime = q.TransactionDateTime.AddSeconds(dif).Date } into xGroup
                                    select new
                                    {
                                        TransactionDateTime = xGroup.Key.TransactionDateTime,
                                        Credit = xGroup.Sum(p => p.Credit),
                                        Debit = xGroup.Sum(p => p.Debit)
                                    }
         ).ToList();

            var data = transactionQuery
                .GroupBy(t => t.TransactionDateTime)
                .ToDictionary(t => t.Key,
                    m => new
                    {
                        credit = m.Sum(p => p.Credit),
                        debit = m.Sum(p => p.Debit)
                    });

            for (var i = 0; i < numberOfDays; i++)
            {
                var convertedDate = startDate.ConvertTimeFromUtc(timeZoneInfo).AddDays(i);
                var item = new WidgetChartSeriesData
                {
                    Name = convertedDate.ToString("dd/MM"),
                    Value = data.ContainsKey(convertedDate.Date) ? data[convertedDate.Date].credit : 0
                };
                series.Add(item);
            }

            for (var i = 0; i < numberOfDays; i++)
            {
                var convertedDate = startDate.ConvertTimeFromUtc(timeZoneInfo).AddDays(i);
                var item = new WidgetChartSeriesData
                {
                    Name = convertedDate.ToString("dd/MM"),
                    Value = data.ContainsKey(convertedDate.Date) ? data[convertedDate.Date].debit : 0
                };
                completedSeries.Add(item);
            }

            return new List<WidgetChartSeries>()
            {
                new WidgetChartSeries()
                {
                    Name = "Bank Statement Credit",
                    Series = series
                } ,
                new WidgetChartSeries()
                {
                    Name = "Bank Statement Debit",
                    Series = completedSeries
                }
            };
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
            var series = new List<WidgetChartSeriesData>();

            var completedSeries = new List<WidgetChartSeriesData>();

            var statementQuery = _BankStatementItemRepository.GetQuery();

            var data = statementQuery.Where(t => _CurrentUser.LoginGuids.Contains(t.LoginGuid)
               && t.TransactionDateTime >= startDate
               && t.TransactionDateTime <= startDate.AddDays(1))
                .GroupBy(t => t.TransactionDateTime.AddSeconds(dif).Hour, t => new { t.Credit, t.Debit })
                .Select(t => new { Hour = t.Key, Credit = t.Sum(p => p.Credit), Debit = t.Sum(p => p.Debit) })//.ToList()

               .ToDictionary(t => t.Hour,
                   m => new AccountingHourlyData
                   {
                       credit = m.Credit,
                       debit = m.Debit
                   });

            for (var i = 0; i < 24; i++)
            {
                data.TryGetValue(i, out AccountingHourlyData convertedDate);
                var item = new WidgetChartSeriesData
                {
                    Name = i.ToString("00"),
                    Value = convertedDate != null ? convertedDate.credit : 0,
                };
                series.Add(item);
            }

            for (var i = 0; i < 24; i++)
            {
                data.TryGetValue(i, out AccountingHourlyData convertedDate);
                var item = new WidgetChartSeriesData
                {
                    Name = i.ToString("00"),
                    Value = convertedDate != null ? convertedDate.debit : 0
                };
                completedSeries.Add(item);
            }

            return new List<WidgetChartSeries>()
            {
               new WidgetChartSeries()
                {
                    Name = "Bank Statement Credit",
                    Series = series
                } ,
                new WidgetChartSeries()
                {
                    Name = "Bank Statement Debit",
                    Series = completedSeries
                }
            };
        }
    }

    class AccountingHourlyData
    {
        public decimal credit { get; set; }

        public decimal debit { get; set; }
    }
}
