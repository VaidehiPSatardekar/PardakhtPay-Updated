using System;

namespace Pardakht.PardakhtPay.Shared.Models.Configuration
{
    public class QueueConfiguration
    {
        public int MaxTryCount { get; set; }

        public int MaxCallbackTryCount { get; set; }

        public int MaxWithdrawalCallbackTryCount { get; set; }

        public int MaxMobileTransferTryCount { get; set; }

        public TimeSpan Delay { get; set; }

        public TimeSpan CallbackDelay { get; set; }

        public TimeSpan WithdrawalCallbackDelay { get; set; }

        public TimeSpan MobileTransferDelay { get; set; }

        public string QueueName { get; set; }

        public string CallbackQueueName { get; set; }

        public string WithdrawalCallbackQueueName { get; set; }

        public string MobileTransferQueueName { get; set; }

        public string QueueConnectionStringName { get; set; }
    }
}
