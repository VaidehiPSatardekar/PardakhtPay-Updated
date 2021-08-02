using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pardakht.PardakhtPay.Shared.Interfaces
{
    public interface ITimeZoneService
    {
        Task<List<string>> ConvertCalendarLocal(List<DateTime> dates, string fromCode, string toCode);

        Task<DateTime> ConvertCalendar(DateTime date, string fromCode, string toCode);
    }
}
