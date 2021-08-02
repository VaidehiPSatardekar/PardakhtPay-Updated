using System;

namespace Pardakht.PardakhtPay.Shared.Models.Configuration
{
    public class CardHolderNameConfiguration
    {
        public bool Enabled { get; set; }

        public TimeSpan Interval { get; set; }

        public int Count { get; set; }
    }
}
