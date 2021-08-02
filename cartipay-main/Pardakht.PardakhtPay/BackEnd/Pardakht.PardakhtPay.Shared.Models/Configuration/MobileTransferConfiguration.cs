using System;

namespace Pardakht.PardakhtPay.Shared.Models.Configuration
{
    public class MobileTransferConfiguration
    {
        public bool Enabled { get; set; }

        public bool Mock { get; set; }

        public string ServiceUrl { get; set; }

        public string ApiKey { get; set; }

        public TimeSpan StartTimeInterval { get; set; }

        public TimeSpan EndTimeInterval { get; set; }

        public TimeSpan CheckInterval { get; set; }

        public int[] ApiTypes { get; set; }
    }
}
