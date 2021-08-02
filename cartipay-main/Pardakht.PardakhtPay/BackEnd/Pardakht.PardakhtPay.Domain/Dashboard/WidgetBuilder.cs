using System;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Domain.Dashboard
{
    /// <summary>
    /// Represents the base class of the widget builder classes
    /// </summary>
    public abstract class WidgetBuilder
    {
        //private static readonly DateTime _CurrentDate = DateTime.UtcNow.Date;

        protected  DateTime CurrentDate { get; set; } = DateTime.UtcNow;
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
                return CurrentDate.AddDays(-29);
            }
        }// last 30 days including today

        protected DateTime ThisQuarterStart
        {
            get
            {
                return CurrentDate.AddDays(-89); // last 90 days including today
            }
        }

        protected DateTime ThisWeekMonday
        {
            get
            {
                var date = CurrentDate.AddDays(-(int)CurrentDate.DayOfWeek + (int)DayOfWeek.Monday);

                if(date > DateTime.UtcNow)
                {
                    date = date.AddDays(-7);
                }

                return date;
            }
        }
        protected DateTime LastWeekMonday
        {
            get
            {
                return CurrentDate.AddDays(-(int)CurrentDate.DayOfWeek + (int)DayOfWeek.Monday).AddDays(-7);
            }
        }
        protected DateTime ThisMonth
        {
            get
            {
                return CurrentDate.AddDays(-(CurrentDate.Day - 1));
            }
        }
        protected DateTime LastMonth
        {
            get
            {
                return CurrentDate.AddDays(-(CurrentDate.Day - 1)).AddMonths(-1);
            }
        }


        protected const int DaysInWeek = 7;
        protected const int DaysInMonth = 30;
        protected const int DaysInQuarter = 90;

        protected WidgetBuilder()
        {
        }

        /// <summary>
        /// Builds and returns the dashboard widget
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public abstract Task<DashboardWidget> Build(DashboardQuery query);
    }
}

