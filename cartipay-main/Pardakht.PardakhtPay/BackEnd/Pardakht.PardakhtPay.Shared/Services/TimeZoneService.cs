using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Pardakht.PardakhtPay.Shared.Extensions;
using Pardakht.PardakhtPay.Shared.Interfaces;
using System.Globalization;
using Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.Helpers;

namespace Pardakht.PardakhtPay.Shared.Services
{
    public class TimeZoneService : ITimeZoneService
    {
        public async Task<DateTime> ConvertCalendar(DateTime date, string fromCode, string toCode)
        {
            var timeZoneInfo = toCode.GetTimeZoneInfo();

            return TimeZoneInfo.ConvertTimeFromUtc(date, timeZoneInfo);
        }

        public async Task<List<string>> ConvertCalendarLocal(List<DateTime> dates, string fromCode, string toCode)
        {
            if(toCode == "ir")
            {
                List<string> newDates = new List<string>();

                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");

                var calendar = new PersianCalendar();

                for (int i = 0; i < dates.Count; i++)
                {
                    var date = TimeZoneInfo.ConvertTime(dates[i].ToLocalTime() , timeZoneInfo);
                    
                    var month = calendar.GetMonth(date);
                    var year = calendar.GetYear(date);
                    var day = calendar.GetDayOfMonth(date);

                    newDates.Add($"{year}-{month.ToString().PadLeft(2, '0')}-{day.ToString().PadLeft(1, '0')}T{date.Hour.ToString().PadLeft(2, '0')}:{date.Minute.ToString().ToString().PadLeft(2, '0')}:{date.Second.ToString().ToString().PadLeft(2, '0')}");
                }

                return newDates;
            }

            return dates.Select(t => t.ToString()).ToList();
        }
    }
}
