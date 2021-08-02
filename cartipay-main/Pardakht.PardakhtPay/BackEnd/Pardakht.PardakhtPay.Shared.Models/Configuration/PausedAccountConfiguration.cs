using System;

namespace Pardakht.PardakhtPay.Shared.Models.Configuration
{
    public class PausedAccountConfiguration
    {
        public bool Enabled { get; set; }

        public TimeSpan Interval { get; set; }

        public TimeSpan CheckCardNumberInterval { get; set; }
    }
}
