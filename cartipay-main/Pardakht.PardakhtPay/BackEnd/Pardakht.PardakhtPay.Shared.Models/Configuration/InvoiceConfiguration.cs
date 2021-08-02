using System;

namespace Pardakht.PardakhtPay.Shared.Models.Configuration
{
    public class InvoiceConfiguration
    {
        public bool Enabled { get; set; }

        public TimeSpan Interval { get; set; }
    }
}
