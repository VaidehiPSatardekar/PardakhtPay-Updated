using System;

namespace Pardakht.PardakhtPay.Shared.Models.Configuration
{
    public class ManualTransferConfiguration
    {
        public bool Enabled { get; set; }

        public TimeSpan TransferInterval { get; set; }

        public TimeSpan ConfirmationDeadline { get; set; }
    }
}
