using System;

namespace Pardakht.PardakhtPay.Shared.Models.Configuration
{
    public class WithdrawalConfiguration
    {
        public bool Enabled { get; set; }

        public TimeSpan TransferInterval { get; set; }

        public TimeSpan ConfirmationDeadline { get; set; }

        public TimeSpan CardToCardDeadline { get; set; }

        public int MaxCardToCardTryCount { get; set; }

        public int MaxWithdrawalAmount { get; set; }
        

        //public TimeSpan TransferCancelInterval { get; set; }
    }
}
