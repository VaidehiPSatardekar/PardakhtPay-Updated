using System;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Domain.Dashboard
{
    public abstract class ChartWidgetBuilder
    {
        //private static readonly DateTime _CurrentDate = DateTime.Now;

        protected DateTime CurrentDate { get; set; } = DateTime.Now;
        protected DateTime Yesterday
        {
            get
            {
               return CurrentDate.AddDays(-1);
            }
        }
        protected DateTime ThisWeekStart
        {
            get
            {
                return CurrentDate.AddDays(-6);
            }
        }

        protected DateTime ThisMonthStart
        {
            get
            {
                return CurrentDate.AddDays(-29); // last 30 days including today
            }
        }

        protected DateTime ThisQuarterStart
        {
            get
            {
                return CurrentDate.AddDays(-89); // last 90 days including today
            }
        }

        protected const int DaysInWeek = 7;
        protected const int DaysInMonth = 30;
        protected const int DaysInQuarter = 90;

        protected ChartWidgetBuilder()
        {
        }

        public abstract Task<DashboardChartWidget> Build(DashboardQuery query);
    }
}
