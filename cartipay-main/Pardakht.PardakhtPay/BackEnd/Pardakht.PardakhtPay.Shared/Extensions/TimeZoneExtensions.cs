using System;
using Pardakht.PardakhtPay.Shared.Models;

namespace Pardakht.PardakhtPay.Shared.Extensions
{
    public static class TimeZoneExtension
    {
        public static DateTime ConvertTimeFromUtc(this DateTime datetime, TimeZoneInfo timeZoneInfo)
        {
            var date = datetime;

            while (timeZoneInfo.IsInvalidTime(date))
            {
                date = date.AddMinutes(-1);
            }

            return TimeZoneInfo.ConvertTimeFromUtc(date, timeZoneInfo);
        }

        public static DateTime ConvertTimeToUtc(this DateTime datetime, TimeZoneInfo timeZoneInfo)
        {
            var date = datetime;

            while (timeZoneInfo.IsInvalidTime(date))
            {
                date = date.AddMinutes(-1);
            }

            return TimeZoneInfo.ConvertTimeToUtc(date, timeZoneInfo);
        }

        public static DateTime Yesterday(this DateTime now)
        {
            return now.AddDays(-1);
        }

        public static DateTime ThisMonday(this DateTime now)
        {
            return now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
        }

        public static DateTime LastMonday(this DateTime now)
        {
            return now.ThisMonday().AddDays(-7);
        }

        public static DateTime ThisMonthStart(this DateTime now)
        {
            return new DateTime(now.Year, now.Month, 1, now.Hour, now.Minute, 0);
        }

        public static DateTime ThisMonthEnd(this DateTime now)
        {
            return now.ThisMonthStart().AddMonths(1);
        }

        public static DateTime LastMonthStart(this DateTime now)
        {
            return now.ThisMonthStart().AddMonths(-1);
        }

        public static string GetCalendarCode(this TimeZoneInfo info)
        {
            if (info.Id.StartsWith("Iran"))
            {
                return "ir";
            }

            return Helper.Utc;
        }

        public static string GetTimeZoneCode(this TimeZoneInfo info)
        {
            if (info.Id.StartsWith("Iran"))
            {
                return "ir2";
            }

            return Helper.Utc;
        }

        public static TimeZoneInfo GetTimeZoneInfo(this string code)
        {
            if(code == "ir2")
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
            }

            return TimeZoneInfo.Utc;
        }
    }
}
