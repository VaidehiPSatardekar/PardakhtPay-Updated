using Microsoft.Extensions.Logging;
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

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class ReportManager : IReportManager
    {
        IBankStatementItemRepository _BankStatementItemRepository = null;
        CurrentUser _CurrentUser = null;
        ILogger _Logger = null;
        ITimeZoneService _TimeZoneService = null;
        ITransactionRepository _TransactionRepository = null;
        IOwnerBankLoginRepository _OwnerBankLoginRepository = null;
        IWithdrawalRepository _WithdrawalRepository = null;
        IManualTransferRepository _ManualTransferRepository = null;
        IManualTransferDetailRepository _ManualTransferDetailRepository = null;
        IAutoTransferRepository _AutoTransferRepository = null;
        IWithdrawalTransferHistoryRepository _WithdrawalTransferHistoryRepository = null;
        IAesEncryptionService _EncryptionService = null;

        public ReportManager(IBankStatementItemRepository bankStatementItemRepository,
            CurrentUser currentUser,
            ILogger<ReportManager> logger,
            ITimeZoneService timeZoneService,
            ITransactionRepository transactionRepository,
            IOwnerBankLoginRepository ownerBankLoginRepository,
            IWithdrawalRepository withdrawalRepository,
            IManualTransferRepository manualTransferRepository,
            IManualTransferDetailRepository manualTransferDetailRepository,
            IAutoTransferRepository autoTransferRepository,
            IWithdrawalTransferHistoryRepository withdrawalTransferHistoryRepository,
            IAesEncryptionService encryptionService)
        {
            _BankStatementItemRepository = bankStatementItemRepository;
            _CurrentUser = currentUser;
            _Logger = logger;
            _TimeZoneService = timeZoneService;
            _TransactionRepository = transactionRepository;
            _OwnerBankLoginRepository = ownerBankLoginRepository;
            _WithdrawalRepository = withdrawalRepository;
            _ManualTransferRepository = manualTransferRepository;
            _ManualTransferDetailRepository = manualTransferDetailRepository;
            _AutoTransferRepository = autoTransferRepository;
            _WithdrawalTransferHistoryRepository = withdrawalTransferHistoryRepository;
            _EncryptionService = encryptionService;
        }

        public async Task<DashboardChartWidget> GetDepositWithdrawalWidget(TransactionReportSearchArgs args)
        {
            var widget = new DashboardChartWidget
            {
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

            var timeZoneCode = args.TimeZoneInfo.GetTimeZoneCode();

            DateTime startDate = await _TimeZoneService.ConvertCalendar(args.StartDate, string.Empty, timeZoneCode);
            DateTime endDate = await _TimeZoneService.ConvertCalendar(args.EndDate, string.Empty, timeZoneCode);

            startDate = await _TimeZoneService.ConvertCalendar(startDate.Date, timeZoneCode, Helper.Utc);
            endDate = await _TimeZoneService.ConvertCalendar(endDate.Date, timeZoneCode, Helper.Utc);

            endDate = endDate.AddDays(1);

            var series = await GetDepositWithdrawalReportData(args.TimeZoneInfo, startDate, endDate);

            widget.MainChart.Add("data", series);

            return widget;
        }

        public async Task<DashboardChartWidget> GetWithdrawalPaymentWidget(WithdrawalPaymentReportArgs args)
        {
            var widget = new DashboardChartWidget
            {
                Title = "REPORTS.WITHDRAWAL-PAYMENT-BREAKDOWN",
                XAxis = true,
                YAxis = true,
                Gradient = false,
                Legend = false,
                ShowXAxisLabel = false,
                XAxisLabel = "Dates",
                ShowYAxisLabel = false,
                YAxisLabel = "Amount",
                Scheme = new Scheme { Domain = new List<string> { "#42BFF7", "#C6ECFD", "#C7B42C", "#AAAAAA", "#B023AB" } },
                MainChart = new Dictionary<string, List<WidgetChartSeries>>()
            };

            var timeZoneCode = args.TimeZoneInfo.GetTimeZoneCode();

            DateTime startDate = await _TimeZoneService.ConvertCalendar(args.StartDate, string.Empty, timeZoneCode);
            DateTime endDate = await _TimeZoneService.ConvertCalendar(args.EndDate, string.Empty, timeZoneCode);

            startDate = await _TimeZoneService.ConvertCalendar(startDate.Date, timeZoneCode, Helper.Utc);
            endDate = await _TimeZoneService.ConvertCalendar(endDate.Date, timeZoneCode, Helper.Utc);

            endDate = endDate.AddDays(1);

            var series = await GetWithdrawalPaymentReportData(args.TimeZoneInfo, startDate, endDate);

            widget.MainChart.Add("data", series);

            return widget;
        }

        public async Task<List<TenantBalanceDTO>> GetTenantBalances(TenantBalanceSearchDTO model)
        {
            var ownerLogins = await _OwnerBankLoginRepository.GetItemsAsync(t => !t.IsDeleted);


            var items = new List<TenantBalanceDTO>();

            for (int i = 0; i < ownerLogins.Count; i++)
            {
                var item = new TenantBalanceDTO();
                item.OwnerGuid = ownerLogins[i].OwnerGuid;

                items.Add(item);
            }

            return items.GroupBy(t => new { t.BankName, t.OwnerGuid }).Select(t => new TenantBalanceDTO() { BankName = t.Key.BankName, OwnerGuid = t.Key.OwnerGuid, Amount = t.Sum(p => p.Amount) }).ToList(); ;
        }

        public async Task<List<UserSegmentReportDTO>> GetUserSegmentReport(UserSegmentReportSearchArgs args)
        {
            var completed = (int)TransactionStatus.Completed;

            var transactionQuery = _TransactionRepository.GetQuery(t => t.UserSegmentGroupId.HasValue && t.Status == completed);

            var timeZoneCode = args.TimeZoneInfo.GetTimeZoneCode();

            var startDate = await _TimeZoneService.ConvertCalendar(args.StartDate, string.Empty, timeZoneCode);
            var endDate = await _TimeZoneService.ConvertCalendar(args.EndDate, string.Empty, timeZoneCode);

            startDate = await _TimeZoneService.ConvertCalendar(startDate.Date, timeZoneCode, Helper.Utc);
            endDate = await _TimeZoneService.ConvertCalendar(endDate.Date, timeZoneCode, Helper.Utc);

            endDate = endDate.AddDays(1);

            var query = (from t in transactionQuery
                         where t.CreationDate >= startDate && t.CreationDate <= endDate
                         group t.TransactionAmount by new { t.OwnerGuid, t.UserSegmentGroupId } into g
                         select new UserSegmentReportDTO
                         {
                             Amount = g.Sum(),
                             OwnerGuid = g.Key.OwnerGuid,
                             UserSegmentId = g.Key.UserSegmentGroupId.Value
                         });

            return await _TransactionRepository.GetModelItemsAsync(query);
        }

        /// <summary>
        /// Generates graph report data for transactions
        /// </summary>
        /// <param name="timeZoneInfo"></param>
        /// <param name="startDate"></param>
        /// <param name="numberOfDays"></param>
        /// <returns></returns>
        private async Task<List<WidgetChartSeries>> GetStatementReportData(TimeZoneInfo timeZoneInfo, DateTime startDate, DateTime endDate, List<string> accountGuids)
        {
            int numberOfDays = endDate.Subtract(startDate).Days;

            var series = new List<WidgetChartSeriesData>();

            var completedSeries = new List<WidgetChartSeriesData>();

            var manualSeries = new List<WidgetChartSeriesData>();

            var autoTransferSeries = new List<WidgetChartSeriesData>();

            var withdrawalSeries = new List<WidgetChartSeriesData>();

            var statementQuery = _BankStatementItemRepository.GetQuery(t => accountGuids.Contains(t.AccountGuid)
                && t.TransactionDateTime >= startDate
                && t.TransactionDateTime < endDate).Select(t => new { t.TransactionDateTime, t.Credit, t.Debit });

            var transactionQuery = (from q in statementQuery
                                    group q by new { q.TransactionDateTime.Year, q.TransactionDateTime.Month, q.TransactionDateTime.Day, q.TransactionDateTime.Hour, Minute = q.TransactionDateTime.Minute < 30 ? 0 : 30 } into xGroup
                                    select new
                                    {
                                        TransactionDateTime = xGroup.Key,
                                        Credit = xGroup.Sum(p => p.Credit),
                                        Debit = xGroup.Sum(p => p.Debit)
                                    });

            var d = await _TransactionRepository.GetModelItemsAsync(transactionQuery);

            var data = d
                .GroupBy(t => new DateTime(t.TransactionDateTime.Year, t.TransactionDateTime.Month, t.TransactionDateTime.Day, t.TransactionDateTime.Hour, t.TransactionDateTime.Minute, 0).ConvertTimeFromUtc(timeZoneInfo).Date)
                .ToDictionary(t => t.Key,
                    m => new
                    {
                        credit = m.Sum(p => p.Credit),
                        debit = m.Sum(p => p.Debit)
                    });

            var manualTransferQuery = _ManualTransferRepository.GetQuery(t => accountGuids.Contains(t.AccountGuid));

            var manualTransferDetailQuery = _ManualTransferDetailRepository.GetQuery(t => t.TransferStatus == (int)TransferStatus.Complete);

            var manualQuery = (from m in manualTransferQuery
                               join md in manualTransferDetailQuery on m.Id equals md.ManualTransferId
                               where md.TransferDate >= startDate && md.TransferDate <= md.TransferDate
                               group md by new { md.TransferDate.Value.Year, md.TransferDate.Value.Month, md.TransferDate.Value.Day, md.TransferDate.Value.Hour, Minute = md.TransferDate.Value.Minute < 30 ? 0 : 30 }
                               into g
                               select new
                               {
                                   Amount = g.Sum(p => p.Amount),
                                   TransactionDateTime = g.Key
                               });

            var ma = await _ManualTransferRepository.GetModelItemsAsync(manualQuery);

            var manualData = ma
                .GroupBy(t => new DateTime(t.TransactionDateTime.Year, t.TransactionDateTime.Month, t.TransactionDateTime.Day, t.TransactionDateTime.Hour, t.TransactionDateTime.Minute, 0).ConvertTimeFromUtc(timeZoneInfo).Date)
                .ToDictionary(t => t.Key,
                    m => new
                    {
                        amount = m.Sum(p => p.Amount)
                    });

            var autoTransferQuery = _AutoTransferRepository.GetQuery(t =>
                        t.Status == (int)TransferStatus.Complete
                        && t.TransferredDate >= startDate
                        && t.TransferredDate <= endDate
                        && accountGuids.Contains(t.AccountGuid)
                    );

            var autoQuery = (from a in autoTransferQuery
                             group a by new { a.TransferredDate.Value.Year, a.TransferredDate.Value.Month, a.TransferredDate.Value.Day, a.TransferredDate.Value.Hour, Minute = a.TransferredDate.Value.Minute < 30 ? 0 : 30 }
                             into g
                             select new
                             {
                                 Amount = g.Sum(t => t.Amount),
                                 TransactionDateTime = g.Key
                             });

            var at = await _AutoTransferRepository.GetModelItemsAsync(autoQuery);

            var autoData = at
                .GroupBy(t => new DateTime(t.TransactionDateTime.Year, t.TransactionDateTime.Month, t.TransactionDateTime.Day, t.TransactionDateTime.Hour, t.TransactionDateTime.Minute, 0).ConvertTimeFromUtc(timeZoneInfo).Date)
                .ToDictionary(t => t.Key,
                    m => new
                    {
                        amount = m.Sum(p => p.Amount)
                    });

            var historyQuery = _WithdrawalTransferHistoryRepository.GetQuery();
            var transferCompleted = (int)TransferStatus.Complete;
            var withdrawalQuery = _WithdrawalRepository.GetQuery(t => accountGuids.Contains(t.AccountGuid));

            var wdq = (from t in withdrawalQuery
                       join hq in historyQuery on t.Id equals hq.WithdrawalId
                       where !string.IsNullOrEmpty(t.FromAccountNumber) && hq.TransferStatus == transferCompleted
                       group Convert.ToDecimal(hq.Amount) by new { hq.LastCheckDate.Year, hq.LastCheckDate.Month, hq.LastCheckDate.Day, hq.LastCheckDate.Hour, Minute = hq.LastCheckDate.Minute < 30 ? 0 : 30 } into g
                       select new
                       {
                           Amount = g.Sum(p => p),
                           TransactionDateTime = g.Key
                       });

            var wd = await _WithdrawalRepository.GetModelItemsAsync(wdq);

            var withdrawalData = wd
                                .GroupBy(t => new DateTime(t.TransactionDateTime.Year, t.TransactionDateTime.Month, t.TransactionDateTime.Day, t.TransactionDateTime.Hour, t.TransactionDateTime.Minute, 0).ConvertTimeFromUtc(timeZoneInfo).Date)
                                .ToDictionary(t => t.Key,
                                    m => new
                                    {
                                        amount = m.Sum(p => p.Amount)
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

            for (var i = 0; i < numberOfDays; i++)
            {
                var convertedDate = startDate.ConvertTimeFromUtc(timeZoneInfo).AddDays(i);
                var item = new WidgetChartSeriesData
                {
                    Name = convertedDate.ToString("dd/MM"),
                    Value = manualData.ContainsKey(convertedDate.Date) ? manualData[convertedDate.Date].amount : 0
                };
                manualSeries.Add(item);
            }

            for (var i = 0; i < numberOfDays; i++)
            {
                var convertedDate = startDate.ConvertTimeFromUtc(timeZoneInfo).AddDays(i);
                var item = new WidgetChartSeriesData
                {
                    Name = convertedDate.ToString("dd/MM"),
                    Value = autoData.ContainsKey(convertedDate.Date) ? autoData[convertedDate.Date].amount : 0
                };
                autoTransferSeries.Add(item);
            }

            for (var i = 0; i < numberOfDays; i++)
            {
                var convertedDate = startDate.ConvertTimeFromUtc(timeZoneInfo).AddDays(i);
                var item = new WidgetChartSeriesData
                {
                    Name = convertedDate.ToString("dd/MM"),
                    Value = withdrawalData.ContainsKey(convertedDate.Date) ? withdrawalData[convertedDate.Date].amount : 0
                };
                withdrawalSeries.Add(item);
            }

            return new List<WidgetChartSeries>()
            {
                new WidgetChartSeries()
                {
                    Name = "Bank Statement Credit",
                    Series = series
                },
                new WidgetChartSeries()
                {
                    Name = "Bank Statement Debit",
                    Series = completedSeries
                },
                new WidgetChartSeries()
                {
                    Name = "Manual Transfers",
                    Series = manualSeries
                },
                new WidgetChartSeries()
                {
                    Name = "Auto Transfers",
                    Series = autoTransferSeries
                },
                new WidgetChartSeries()
                {
                    Name = "Withdrawals",
                    Series = withdrawalSeries
                }
            };
        }

        private async Task<List<WidgetChartSeries>> GetDepositWithdrawalReportData(TimeZoneInfo timeZoneInfo, DateTime startDate, DateTime endDate)
        {
            int numberOfDays = endDate.Subtract(startDate).Days + 1;

            var completedSeries = new List<WidgetChartSeriesData>();
            var withdrawalSeries = new List<WidgetChartSeriesData>();
            var unconfirmedSeries = new List<WidgetChartSeriesData>();
            var pendingWithdrawalSeries = new List<WidgetChartSeriesData>();

            var query = _TransactionRepository.GetQuery();
            var unconfirmedQuery = _TransactionRepository.GetQuery();

            int completed = (int)TransactionStatus.Completed;

            query = query.Where(t => t.Status == completed && t.CreationDate >= startDate
                && t.CreationDate <= endDate);

            unconfirmedQuery = unconfirmedQuery.Where(t => t.Status != completed && t.CreationDate >= startDate && t.CreationDate <= endDate);

            var transactionQuery = (from q in query
                                    group q by new { q.CreationDate.Year, q.CreationDate.Month, q.CreationDate.Day, q.CreationDate.Hour, Minute = q.CreationDate.Minute < 30 ? 0 : 30 } into xGroup
                                    select new
                                    {
                                        CreationDate = xGroup.Key,
                                        TransactionAmount = xGroup.Sum(p => p.TransactionAmount)
                                    });

            var d = await _TransactionRepository.GetModelItemsAsync(transactionQuery);

            var data = d
                .GroupBy(t => new DateTime(t.CreationDate.Year, t.CreationDate.Month, t.CreationDate.Day, t.CreationDate.Hour, t.CreationDate.Minute, 0).ConvertTimeFromUtc(timeZoneInfo).Date)
                .ToDictionary(t => t.Key,
                    m => new
                    {
                        completedSum = m.Sum(p => p.TransactionAmount)
                    });

            var unConfirmedTransactionQuery = (from q in unconfirmedQuery
                                               group q by new { q.CreationDate.Year, q.CreationDate.Month, q.CreationDate.Day, q.CreationDate.Hour, Minute = q.CreationDate.Minute < 30 ? 0 : 30 } into xGroup
                                               select new
                                               {
                                                   CreationDate = xGroup.Key,
                                                   TransactionAmount = xGroup.Sum(p => p.TransactionAmount)
                                               });

            var un = await _TransactionRepository.GetModelItemsAsync(unConfirmedTransactionQuery);

            var unconfirmedData = un
                .GroupBy(t => new DateTime(t.CreationDate.Year, t.CreationDate.Month, t.CreationDate.Day, t.CreationDate.Hour, t.CreationDate.Minute, 0).ConvertTimeFromUtc(timeZoneInfo).Date)
                .ToDictionary(t => t.Key,
                    m => new
                    {
                        completedSum = m.Sum(p => p.TransactionAmount)
                    });

            var statuses = new int[] { (int)WithdrawalStatus.Pending, (int)WithdrawalStatus.PartialPaid, (int)WithdrawalStatus.PendingBalance, (int)WithdrawalStatus.PendingCardToCardConfirmation, (int)WithdrawalStatus.Sent };

            var wQuery = _WithdrawalRepository.GetQuery(t => statuses.Contains(t.WithdrawalStatus) && t.TransferDate != null && t.ExpectedTransferDate >= startDate && t.ExpectedTransferDate <= endDate);

            var withdrawalQuery = (from q in wQuery
                                   group q by new { q.ExpectedTransferDate.Year, q.ExpectedTransferDate.Month, q.ExpectedTransferDate.Day, q.ExpectedTransferDate.Hour, Minute = q.ExpectedTransferDate.Minute < 30 ? 0 : 30 } into xGroup
                                   select new
                                   {
                                       TransferDate = xGroup.Key,
                                       RemainingAmount = xGroup.Sum(p => p.RemainingAmount)
                                   });

            var w = await _WithdrawalRepository.GetModelItemsAsync(withdrawalQuery);

            var withdrawalData = w
                .GroupBy(t => new DateTime(t.TransferDate.Year, t.TransferDate.Month, t.TransferDate.Day, t.TransferDate.Hour, t.TransferDate.Minute, 0).ConvertTimeFromUtc(timeZoneInfo).Date)
                .ToDictionary(t => t.Key,
                    m => new
                    {
                        pendingAmount = m.Sum(p => p.RemainingAmount)
                    });

            var cWithdrawalQuery = _WithdrawalRepository.GetQuery(t => (t.WithdrawalStatus == (int)WithdrawalStatus.Confirmed || t.WithdrawalStatus == (int)WithdrawalStatus.PartialPaid) && t.TransferDate != null && t.ExpectedTransferDate >= startDate && t.ExpectedTransferDate <= endDate);

            var confirmedWithdrawalQuery = (from q in cWithdrawalQuery
                                            group q by new { q.TransferDate.Value.Year, q.TransferDate.Value.Month, q.TransferDate.Value.Day, q.TransferDate.Value.Hour, Minute = q.TransferDate.Value.Minute < 30 ? 0 : 30 } into xGroup
                                   select new
                                   {
                                       TransferDate = xGroup.Key,
                                       Amount = xGroup.Sum(p => p.Amount - p.RemainingAmount)
                                   });

            var c = await _WithdrawalRepository.GetModelItemsAsync(confirmedWithdrawalQuery);

            var confirmedWithdrawalData = c
                .GroupBy(t => new DateTime(t.TransferDate.Year, t.TransferDate.Month, t.TransferDate.Day, t.TransferDate.Hour, t.TransferDate.Minute, 0).ConvertTimeFromUtc(timeZoneInfo).Date)
                .ToDictionary(t => t.Key,
                    m => new
                    {
                        amount = m.Sum(p => p.Amount)
                    });

            for (var i = 0; i < numberOfDays; i++)
            {
                var convertedDate = startDate.ConvertTimeFromUtc(timeZoneInfo).AddDays(i);
                var item = new WidgetChartSeriesData
                {
                    Name = convertedDate.ToString("dd/MM"),
                    Value = data.ContainsKey(convertedDate.Date) ? data[convertedDate.Date].completedSum : 0
                };
                completedSeries.Add(item);

                item = new WidgetChartSeriesData
                {
                    Name = convertedDate.ToString("dd/MM"),
                    Value = confirmedWithdrawalData.ContainsKey(convertedDate.Date) ? confirmedWithdrawalData[convertedDate.Date].amount : 0
                };
                withdrawalSeries.Add(item);

                var unconfirmedItem = new WidgetChartSeriesData()
                {
                    Name = convertedDate.ToString("dd/MM"),
                    Value = unconfirmedData.ContainsKey(convertedDate.Date) ? unconfirmedData[convertedDate.Date].completedSum : 0
                };

                unconfirmedSeries.Add(unconfirmedItem);

                var pendingItem = new WidgetChartSeriesData()
                {
                    Name = convertedDate.ToString("dd/MM"),
                    Value = withdrawalData.ContainsKey(convertedDate.Date) ? withdrawalData[convertedDate.Date].pendingAmount : 0
                };

                pendingWithdrawalSeries.Add(pendingItem);
            }

            return new List<WidgetChartSeries>()
            {
                new WidgetChartSeries()
                {
                    Name = "Completed Deposits",
                    Series = completedSeries
                },
                new WidgetChartSeries()
                {
                    Name = "Unconfirmed Deposits",
                    Series = unconfirmedSeries
                },
                new WidgetChartSeries()
                {
                    Name = "Completed Withdrawals",
                    Series = withdrawalSeries
                },
                new WidgetChartSeries()
                {
                    Name = "Pending Withdrawals",
                    Series = pendingWithdrawalSeries
                }
            };
        }
        private async Task<List<WidgetChartSeries>> GetWithdrawalPaymentReportData(TimeZoneInfo timeZoneInfo, DateTime startDate, DateTime endDate)
        {
            int numberOfDays = endDate.Subtract(startDate).Days + 1;

            var transferSeries = new List<WidgetChartSeriesData>();
            var pardakhtPalSeries = new List<WidgetChartSeriesData>();

            var query = _TransactionRepository.GetQuery();

            int completed = (int)TransactionStatus.Completed;

            query = query.Where(t => t.Status == completed
            && t.WithdrawalId.HasValue
            && t.CreationDate >= startDate
                && t.CreationDate <= endDate);

            var transactionQuery = (from q in query
                                    group q by new { q.CreationDate.Year, q.CreationDate.Month, q.CreationDate.Day, q.CreationDate.Hour, Minute = q.CreationDate.Minute < 30 ? 0 : 30 } into xGroup
                                    select new
                                    {
                                        CreationDate = xGroup.Key,
                                        TransactionAmount = xGroup.Sum(p => p.TransactionAmount)
                                    });

            var d = await _TransactionRepository.GetModelItemsAsync(transactionQuery);

            var data = d
                .GroupBy(t => new DateTime(t.CreationDate.Year, t.CreationDate.Month, t.CreationDate.Day, t.CreationDate.Hour, t.CreationDate.Minute, 0).ConvertTimeFromUtc(timeZoneInfo).Date)
                .ToDictionary(t => t.Key,
                    m => new
                    {
                        completedSum = m.Sum(p => p.TransactionAmount)
                    });

            var historyQuery = _WithdrawalTransferHistoryRepository.GetQuery();
            var transferCompleted = (int)TransferStatus.Complete;
            var withdrawalQuery = _WithdrawalRepository.GetQuery();

            var wdq = (from t in withdrawalQuery
                       join hq in historyQuery on t.Id equals hq.WithdrawalId
                       where !string.IsNullOrEmpty(t.FromAccountNumber) && hq.TransferStatus == transferCompleted
                       group Convert.ToDecimal(hq.Amount) by new { hq.LastCheckDate.Year, hq.LastCheckDate.Month, hq.LastCheckDate.Day, hq.LastCheckDate.Hour, Minute = hq.LastCheckDate.Minute < 30 ? 0 : 30 } into g
                       select new
                       {
                           Amount = g.Sum(p => p),
                           TransactionDateTime = g.Key
                       });

            var wd = await _WithdrawalRepository.GetModelItemsAsync(wdq);

            var withdrawalData = wd
                                .GroupBy(t => new DateTime(t.TransactionDateTime.Year, t.TransactionDateTime.Month, t.TransactionDateTime.Day, t.TransactionDateTime.Hour, t.TransactionDateTime.Minute, 0).ConvertTimeFromUtc(timeZoneInfo).Date)
                                .ToDictionary(t => t.Key,
                                    m => new
                                    {
                                        amount = m.Sum(p => p.Amount)
                                    });

            for (var i = 0; i < numberOfDays; i++)
            {
                var convertedDate = startDate.ConvertTimeFromUtc(timeZoneInfo).AddDays(i);
                var item = new WidgetChartSeriesData
                {
                    Name = convertedDate.ToString("dd/MM"),
                    Value = data.ContainsKey(convertedDate.Date) ? data[convertedDate.Date].completedSum : 0
                };
                pardakhtPalSeries.Add(item);

                var item2 = new WidgetChartSeriesData
                {
                    Name = convertedDate.ToString("dd/MM"),
                    Value = withdrawalData.ContainsKey(convertedDate.Date) ? withdrawalData[convertedDate.Date].amount : 0
                };
                transferSeries.Add(item2);

                var total = item.Value + item2.Value;

                if(total > 0)
                {
                    item.Extra = item.Value / total;
                    item2.Extra = item2.Value / total;
                }
            }

            return new List<WidgetChartSeries>()
            {
                new WidgetChartSeries()
                {
                    Name = "PardakhtPal Payments",
                    Series = pardakhtPalSeries
                },
                new WidgetChartSeries()
                {
                    Name = "Transfers",
                    Series = transferSeries
                }
            };
        }

        public async Task<DashboardChartWidget> GetDepositByAccountIdWidget(TransactionReportSearchArgs args)
        {
            var widget = new DashboardChartWidget
            {
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

            var timeZoneCode = args.TimeZoneInfo.GetTimeZoneCode();

            DateTime startDate = await _TimeZoneService.ConvertCalendar(args.StartDate, string.Empty, timeZoneCode);
            DateTime endDate = await _TimeZoneService.ConvertCalendar(args.EndDate, string.Empty, timeZoneCode);

            startDate = await _TimeZoneService.ConvertCalendar(startDate.Date, timeZoneCode, Helper.Utc);
            endDate = await _TimeZoneService.ConvertCalendar(endDate.Date, timeZoneCode, Helper.Utc);

            endDate = endDate.AddDays(1);

            var series = await GetDepositByAccountIdReportData(args.TimeZoneInfo, startDate, endDate);

            widget.MainChart.Add("data", series);

            return widget;
        }

        private async Task<List<WidgetChartSeries>> GetDepositByAccountIdReportData(TimeZoneInfo timeZoneInfo, DateTime startDate, DateTime endDate)
        {
            int numberOfDays = endDate.Subtract(startDate).Days + 1;

            List<WidgetChartSeries> depositBreakDownSeries = new List<WidgetChartSeries>();

            var baseQuery = _TransactionRepository.GetQuery();
            int completed = (int)TransactionStatus.Completed;

            var paymentTypes = new int[] { (int)PaymentType.SamanBank, (int)PaymentType.MeliBank, (int)PaymentType.Zarinpal, (int)PaymentType.Mellat, (int)PaymentType.Novin };

            #region PaymentType-CardToCard / Pament Getway
            var query = baseQuery.Where(t => t.Status == completed && t.CreationDate >= startDate
                && t.CreationDate <= endDate && t.WithdrawalId == null && (paymentTypes.Contains(t.PaymentType)));
            var transactionQuery = (from q in query
                                    group q by new
                                    {
                                        q.CreationDate.Year,
                                        q.CreationDate.Month,
                                        q.CreationDate.Day,
                                        q.CreationDate.Hour,
                                        Minute = q.CreationDate.Minute < 30 ? 0 : 30,
                                        AccountNumber = q.AccountNumber
                                    } into xGroup
                                    select new
                                    {
                                        CreationDate = xGroup.Key,
                                        TransactionAmount = xGroup.Sum(p => p.TransactionAmount),
                                        AccountNumber = xGroup.Key.AccountNumber
                                    });
            var d = await _TransactionRepository.GetModelItemsAsync(transactionQuery);
            var data = d
              .GroupBy(t =>
               new { CreationDate = new DateTime(t.CreationDate.Year, t.CreationDate.Month, t.CreationDate.Day, t.CreationDate.Hour, t.CreationDate.Minute, 0).ConvertTimeFromUtc(timeZoneInfo).Date, AccountNumber = t.AccountNumber }, t => t.TransactionAmount)
              .Select(t => new { CreationDate = t.Key.CreationDate, AccountNumber = t.Key.AccountNumber, Amount = t.Sum(), Count = t.Count() })
              .ToDictionary(t => new { t.CreationDate, t.AccountNumber },
                   m => new DataForDepositBreakDown()
                   {
                       completedSum = m.Amount
                   });

            var accountNumberList = d.Select(c => c.AccountNumber).Distinct();

            foreach (var accountNumber in accountNumberList)
            {
                var completedSeries = new List<WidgetChartSeriesData>();
                for (var i = 0; i < numberOfDays; i++)
                {
                    var convertedDate = startDate.ConvertTimeFromUtc(timeZoneInfo).AddDays(i);
                    var key = data.Keys.ToList().FirstOrDefault(t => t.CreationDate == convertedDate && t.AccountNumber == accountNumber);
                    DataForDepositBreakDown convertedDateFromDictionary = null;
                    if (key != null)
                    {
                        data.TryGetValue(new { CreationDate = convertedDate, AccountNumber = accountNumber }, out convertedDateFromDictionary);
                    }
                    var item = new WidgetChartSeriesData
                    {
                        Name = convertedDate.ToString("dd/MM"),
                        Value = convertedDateFromDictionary != null ? convertedDateFromDictionary.completedSum : 0
                    };
                    completedSeries.Add(item);
                }


                depositBreakDownSeries.Add(new WidgetChartSeries()
                {
                    Name = _EncryptionService.DecryptToString(accountNumber),
                    Series = completedSeries
                });

            }
            #endregion

            //#region PaymentType-Mobile
            //var queryForCardNumber = baseQuery.Where(t => t.Status == completed && t.CreationDate >= startDate
            //    && t.CreationDate <= endDate && t.WithdrawalId == null && t.PaymentType == (int)PaymentType.Mobile);
            //var transactionQueryForCardNumber = (from q in queryForCardNumber
            //                        group q by new
            //                        {
            //                            q.CreationDate.Year,
            //                            q.CreationDate.Month,
            //                            q.CreationDate.Day,
            //                            q.CreationDate.Hour,
            //                            Minute = q.CreationDate.Minute < 30 ? 0 : 30,
            //                            CardNumber = q.CardNumber
            //                        } into xGroup
            //                        select new
            //                        {
            //                            CreationDate = xGroup.Key,
            //                            TransactionAmount = xGroup.Sum(p => p.TransactionAmount),
            //                            CardNumber = xGroup.Key.CardNumber
            //                        });
            //var dForCardNumber = await _TransactionRepository.GetModelItemsAsync(transactionQueryForCardNumber);
            //var dataForCardNumber = dForCardNumber
            //  .GroupBy(t =>
            //   new { CreationDate = new DateTime(t.CreationDate.Year, t.CreationDate.Month, t.CreationDate.Day, t.CreationDate.Hour, t.CreationDate.Minute, 0).ConvertTimeFromUtc(timeZoneInfo).Date, CardNumber = t.CardNumber }, t => t.TransactionAmount)
            //  .Select(t => new { CreationDate = t.Key.CreationDate, CardNumber = t.Key.CardNumber, Amount = t.Sum(), Count = t.Count() })
            //  .ToDictionary(t => new { t.CreationDate, t.CardNumber },
            //       m => new DataForDepositBreakDown()
            //       {
            //           completedSum = m.Amount
            //       });

            //var cardNumberList = dForCardNumber.Select(c => c.CardNumber).Distinct();

            //foreach (var cardNumber in cardNumberList)
            //{
            //    var completedSeries = new List<WidgetChartSeriesData>();
            //    for (var i = 0; i < numberOfDays; i++)
            //    {
            //        var convertedDate = startDate.ConvertTimeFromUtc(timeZoneInfo).AddDays(i);
            //        var key = dataForCardNumber.Keys.ToList().FirstOrDefault(t => t.CreationDate == convertedDate && t.CardNumber == cardNumber);
            //        DataForDepositBreakDown convertedDateFromDictionary = null;
            //        if (key != null)
            //        {
            //            dataForCardNumber.TryGetValue(new { CreationDate = convertedDate, CardNumber = cardNumber }, out convertedDateFromDictionary);
            //        }
            //        var item = new WidgetChartSeriesData
            //        {
            //            Name = convertedDate.ToString("dd/MM"),
            //            Value = convertedDateFromDictionary != null ? convertedDateFromDictionary.completedSum : 0
            //        };
            //        completedSeries.Add(item);
            //    }

            
            //    depositBreakDownSeries.Add(new WidgetChartSeries()
            //    {
            //        Name = _EncryptionService.DecryptToString(cardNumber),
            //        Series = completedSeries
            //    });

            //}

            //#endregion






            return depositBreakDownSeries;
        }

        public async Task<ListSearchResponse<List<DepositBreakDownReport>>> GetDepositBreakdownList(TransactionReportSearchArgs args)
        {
            var timeZoneCode = args.TimeZoneInfo.GetTimeZoneCode();

            DateTime startDate = await _TimeZoneService.ConvertCalendar(args.StartDate, string.Empty, timeZoneCode);
            DateTime endDate = await _TimeZoneService.ConvertCalendar(args.EndDate, string.Empty, timeZoneCode);

            startDate = await _TimeZoneService.ConvertCalendar(startDate.Date, timeZoneCode, Helper.Utc);
            endDate = await _TimeZoneService.ConvertCalendar(endDate.Date, timeZoneCode, Helper.Utc);

            endDate = endDate.AddDays(1);

            int numberOfDays = endDate.Subtract(startDate).Days + 1;

            List<WidgetChartSeries> depositBreakDownSeries = new List<WidgetChartSeries>();

            var baseQuery = _TransactionRepository.GetQuery();
            int completed = (int)TransactionStatus.Completed;

            var paymentTypes = new int[] { (int)PaymentType.SamanBank, (int)PaymentType.MeliBank, (int)PaymentType.Zarinpal, (int)PaymentType.Mellat, (int)PaymentType.Novin };

            #region PaymentType-CardToCard / Pament Getway
            var query = baseQuery.Where(t => t.Status == completed && t.CreationDate >= startDate
                && t.CreationDate <= endDate && t.WithdrawalId == null && (paymentTypes.Contains(t.PaymentType)));

            var transactionQuery = (from q in query
                                    group q by new
                                    {
                                        q.CreationDate.Year,
                                        q.CreationDate.Month,
                                        q.CreationDate.Day,
                                        q.CreationDate.Hour,
                                        Minute = q.CreationDate.Minute < 30 ? 0 : 30,
                                        PaymentType = q.PaymentType
                                    } into xGroup
                                    select new
                                    {
                                        CreationDate = xGroup.Key,
                                        TransactionAmount = xGroup.Sum(p => p.TransactionAmount),
                                        PaymentType = xGroup.Key.PaymentType
                                    });
            var d = await _TransactionRepository.GetModelItemsAsync(transactionQuery);
            var data = d
              .GroupBy(t =>
               new { CreationDate = new DateTime(t.CreationDate.Year, t.CreationDate.Month, t.CreationDate.Day, t.CreationDate.Hour, t.CreationDate.Minute, 0).ConvertTimeFromUtc(args.TimeZoneInfo).Date, PaymentType = t.PaymentType }, t => t.TransactionAmount)
              .Select(t => new { CreationDate = t.Key.CreationDate, PaymentType = t.Key.PaymentType, Amount = t.Sum(), Count = t.Count() })
              .ToDictionary(t => new { t.CreationDate, t.PaymentType },
                   m => new DepositBreakDownData()
                   {
                       Amount = m.Amount
                   });



            var paymentTypeWiseData = data
            .GroupBy(t =>
            new { CreationDate = t.Key.CreationDate, PaymentType = t.Key.PaymentType }, t => t.Value.Amount)
            .Select(t => new
            {
                BreakDownDate = t.Key.CreationDate,
                SamanBank =t.Where(c=> t.Key.PaymentType  == (int)PaymentType.SamanBank).Sum(),
                MeliBank = t.Where(c => t.Key.PaymentType == (int)PaymentType.MeliBank).Sum(),
                Zarinpal = t.Where(c => t.Key.PaymentType == (int)PaymentType.Zarinpal).Sum(),
                Mellat = t.Where(c => t.Key.PaymentType == (int)PaymentType.Mellat).Sum(),
                Novin = t.Where(c => t.Key.PaymentType == (int)PaymentType.Novin).Sum()
            });

            //var paymentTypeWiseData = 
            //.GroupBy(t =>
            //new { CreationDate = new DateTime(t.CreationDate.Year, t.CreationDate.Month, t.CreationDate.Day, t.CreationDate.Hour, t.CreationDate.Minute, 0).ConvertTimeFromUtc(args.TimeZoneInfo).Date, PaymentType = t.PaymentType }, t => t.TransactionAmount)
            //.Select(t => new {
            //    BreakDownDate = t.Key.CreationDate,
            //    SamanBank = t.Where(c => (int)PaymentType.SamanBank == 3).Sum(),
            //    MeliBank = t.Where(c => (int)PaymentType.MeliBank == 4).Sum(),
            //    Zarinpal = t.Where(c => (int)PaymentType.Zarinpal == 5).Sum(),
            //    Mellat = t.Where(c => (int)PaymentType.Mellat == 6).Sum(),
            //    Novin = t.Where(c => (int)PaymentType.Novin == 7).Sum()
            //});

            //return paymentTypeWiseData.Cast<DepositBreakDownReport>().ToList();
            var depositBreakDownReportList = new List<DepositBreakDownReport>();
            
            foreach (var paymentTypeData in paymentTypeWiseData)
            {
                var depositBreakDownReport = new DepositBreakDownReport();
                depositBreakDownReport.BreakDownDate = paymentTypeData.BreakDownDate;
                depositBreakDownReport.SamanBank = paymentTypeData.SamanBank;
                depositBreakDownReport.MeliBank = paymentTypeData.MeliBank;
                depositBreakDownReport.Zarinpal = paymentTypeData.Zarinpal;
                depositBreakDownReport.Mellat = paymentTypeData.Mellat;
                depositBreakDownReport.Novin = paymentTypeData.Novin;
                depositBreakDownReportList.Add(depositBreakDownReport);
            }
            var totalCount = depositBreakDownReportList.Count;
            return new ListSearchResponse<List<DepositBreakDownReport>>()
            {
                Items = depositBreakDownReportList,
                Success = true,
                Paging = new PagingHeader(totalCount, 0, 0, 0)
            };

            



                #endregion
                //var depositBreakDownReportList = new List<DepositBreakDownReport>();
                //var depositBreakDownReport = new DepositBreakDownReport();

                //var paymentTypeList = d.Select(c => c.PaymentType).Distinct();

                //foreach (var paymentType in paymentTypeList)
                //{
                //    for (var i = 0; i < numberOfDays; i++)
                //    {
                //        var convertedDate = startDate.ConvertTimeFromUtc(args.TimeZoneInfo).AddDays(i);
                //        var key = data.Keys.ToList().FirstOrDefault(t => t.CreationDate == convertedDate);
                //        depositBreakDownReport = null;
                //        if (key != null)
                //        {
                //            data.TryGetValue(new { CreationDate = convertedDate, PaymentType = paymentType }, out depositBreakDownReport);
                //        }

                //        if (depositBreakDownReport != null)
                //        {
                //            depositBreakDownReport.BreakDownDate = convertedDate;
                //            depositBreakDownReport.PaymentType = Enum.GetName(typeof(PaymentType), paymentType);
                //            depositBreakDownReport.Amount = depositBreakDownReport.Amount;

                //            depositBreakDownReportList.Add(depositBreakDownReport);
                //        }
                //    }
                //}

                //return depositBreakDownReportList;
        }


        class DataForDepositBreakDown { 
        
            public DateTime creationDate { get; set; }
            public decimal sum { get; set; }

            public decimal completedSum { get; set; }

            public int cardNumber { get; set; }
            public int AccountNumber { get; set; }
        }

    }
}
