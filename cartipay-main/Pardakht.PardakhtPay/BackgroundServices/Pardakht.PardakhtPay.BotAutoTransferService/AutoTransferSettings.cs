using System;

namespace Pardakht.PardakhtPay.BotAutoTransferService
{
    public class AutoTransferSettings
    {
        public bool Enabled { get; set; }

        public TimeSpan TransferInterval { get; set; }

        public TimeSpan TransferCancelInterval { get; set; }

        public string SplitString { get; set; }

        public int RemovegDigitCount { get; set; }
    }
}
