using System;
using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Models.TimeZone
{
    public class TimeZoneRequest
    {
        public string fromAreaCode { get; set; }

        public string toAreaCode { get; set; }

        public DateTime? DateTime { get; set; }

        public List<DateTime> DateTimeList { get; set; }
    }
     
    public class TimeZoneLocalResponse
    {
        public string DateTime { get; set; }

        public List<string> DateTimeList { get; set; }
    }
}
