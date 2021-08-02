using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.TimeZone
{
    public interface ITimeZoneService
    {
        Task<DateTime> ConvertCalendar(DateTime date, string fromCode, string toCode, string jwtToken);
        Task<DateTime> ConvertCalendar(DateTime date, string fromCode, string toCode);
        Task<List<string>> ConvertCalendarLocal(List<DateTime> dates, string fromCode, string toCode);
        Task<List<string>> ConvertCalendarLocal(List<DateTime> dates, string fromCode, string toCode, string jwtToken);
    }
}
