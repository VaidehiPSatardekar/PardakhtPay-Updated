using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Bot
{
    public class BotQueueItem
    {
        public string TransactionCode { get; set; }

        public DateTime? LastTryDateTime { get; set; }

        public int TryCount { get; set; }

        public string TenantGuid { get; set; }
    }

    public class CallbackQueueItem
    {
        public string TransactionCode { get; set; }

        public DateTime? LastTryDateTime { get; set; }

        public int TryCount { get; set; }

        public string TenantGuid { get; set; }
    }

    public class WithdrawalQueueItem
    {
        public int Id { get; set; }

        public DateTime? LastTryDateTime { get; set; }

        public int TryCount { get; set; }

        public string TenantGuid { get; set; }
    }

    public class MobileTransferQueueItem
    {
        public string TransactionCode { get; set; }

        public DateTime? LastTryDateTime { get; set; }

        public int TryCount { get; set; }

        public string TenantGuid { get; set; }
    }
}
