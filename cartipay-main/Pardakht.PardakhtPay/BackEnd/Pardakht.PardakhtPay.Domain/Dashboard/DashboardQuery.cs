using System;
using System.ComponentModel;

namespace Pardakht.PardakhtPay.Domain.Dashboard
{
    public class DashboardQuery
    {
        /// <summary>
        /// Merchant Id
        /// </summary>
        /// <example>10</example>
        [Description("The Merchant ID")]
        public int? MerchantId { get; set; }

        [Description("The filtered tenant guid which defines at tenant management system")]
        public string TenantGuid { get; set; }

        /// <summary>
        /// DateType
        /// </summary>
        /// <example>add datetype here</example>
        [Description("The Date type should be for; 'dt':'Today','dy':'Yesterday','dtw':'This Week','dlw':'Last Week','dtm':'This Month','dlm':'Last Month','all':'All'")]
        public string DateType { get; set; }

        /// <summary>
        /// Argument
        /// </summary>
        /// <example>Add Argument type </example>
        [Description("The database query argument")]
        public string Argument { get; set; }

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

    public class DashboardPagingQuery: DashboardQuery
    {
        public int StartRow { get; set; }

        public int EndRow { get; set; }

        public string SortColumn { get; set; }

        public string SortOrder { get; set; }
    }
}
