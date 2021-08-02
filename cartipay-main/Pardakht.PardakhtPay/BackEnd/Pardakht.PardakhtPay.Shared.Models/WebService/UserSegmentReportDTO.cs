using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class UserSegmentReportDTO
    {
        public int UserSegmentId { get; set; }

        public string OwnerGuid { get; set; }

        public decimal Amount { get; set; }
    }

    public class UserSegmentReportSearchArgs
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string TimeZoneInfoId { get; set; }

        public TimeZoneInfo TimeZoneInfo
        {
            get
            {
                if (!string.IsNullOrEmpty(TimeZoneInfoId))
                {
                    return System.TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfoId);
                }

                return TimeZoneInfo.Utc;
            }
        }
    }
}
