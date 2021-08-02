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
    public class MerchantTransactionReportWidgetBuilder : WidgetBuilder, IMerchantTransactionReportWidgetBuilder
    {
        ITransactionManager _TransactionManager;
        ITransactionRepository _TransactionRepository = null;
        IMerchantRepository _MerchantRepository = null;
        ITimeZoneService _TimeZoneService = null;

        public MerchantTransactionReportWidgetBuilder(ITransactionManager transactionManager,
            ITransactionRepository transactionRepository,
            IMerchantRepository merchantRepository,
            ITimeZoneService timeZoneService)
        {
            _TransactionManager = transactionManager;
            _TransactionRepository = transactionRepository;
            _MerchantRepository = merchantRepository;
            _TimeZoneService = timeZoneService;
        }

        public override async Task<DashboardWidget> Build(DashboardQuery query)
        {
            var timeZoneCode = query.TimeZoneInfo.GetTimeZoneCode();

            DateTime startDate = await _TimeZoneService.ConvertCalendar(DateTime.Now, string.Empty, timeZoneCode);

            startDate = await _TimeZoneService.ConvertCalendar(startDate.Date, timeZoneCode, Helper.Utc);

            CurrentDate = startDate;

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

            await Task.Run(() =>
            {

                var completed = (int)TransactionStatus.Completed;
                IQueryable<Transaction> transactionQuery = _TransactionRepository.GetQuery();
                transactionQuery = transactionQuery.Where(t => t.Status == completed);

                IQueryable<Merchant> merchantQuery = _MerchantRepository.GetQuery();

                if (!string.IsNullOrEmpty(query.TenantGuid))
                {
                    merchantQuery = merchantQuery.Where(t => t.TenantGuid == query.TenantGuid);
                    transactionQuery = transactionQuery.Where(t => t.TenantGuid == query.TenantGuid);
                }

                DateTime? endDate = null;

                switch (query.DateType)
                {
                    case DatePeriodType.Today:
                        startDate = CurrentDate;
                        endDate = startDate.AddDays(1);

                        transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate && t.CreationDate < endDate);
                        break;

                    case DatePeriodType.Yesterday:
                        startDate = CurrentDate.AddDays(-1);
                        endDate = CurrentDate;

                        transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate && t.CreationDate < endDate);
                        break;
                    case DatePeriodType.ThisWeek:

                        startDate = ThisWeekMonday;

                        transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate);
                        break;
                    case DatePeriodType.LastWeek:
                        startDate = LastWeekMonday;
                        endDate = ThisWeekMonday;

                        transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate && t.CreationDate < endDate);
                        break;
                    case DatePeriodType.ThisMonth:
                        startDate = ThisMonth;

                        transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate);
                        break;
                    case DatePeriodType.LastMonth:
                        startDate = LastMonth;
                        endDate = ThisMonth;

                        transactionQuery = transactionQuery.Where(t => t.CreationDate >= startDate && t.CreationDate < endDate);
                        break;
                    case DatePeriodType.All:
                        break;
                    default:
                        throw new NotImplementedException($"Merchant Transaction report widget builder has not implemented this date type : {query.DateType}");
                }

                var items = (from q in transactionQuery
                             join m in merchantQuery on q.MerchantId equals m.Id
                             group q by m.Title into xGroup
                             select new { Title = xGroup.Key, Sum = xGroup.Sum(p => p.TransactionAmount), Count = xGroup.Count() }).ToList();

                items.ForEach(item =>
                {

                    var widgetDataCore = new WidgetDataCore()
                    {
                        Label = item.Title,
                        Count = new Dictionary<string, object>()
                        {
                            {
                                "Amount", item.Sum.ToString()
                            },
                            {
                                "Count", item.Count.ToString()
                            }
                        }
                    };

                    widget.Data.CoreData.Add(widgetDataCore);
                });
            });

            return widget;
        }
    }
}

