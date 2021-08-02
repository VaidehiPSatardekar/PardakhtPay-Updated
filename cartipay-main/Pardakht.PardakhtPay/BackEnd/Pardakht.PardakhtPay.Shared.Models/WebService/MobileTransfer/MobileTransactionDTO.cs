using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.MobileTransfer
{
    public class MobileTransactionDTO
    {
        public Guid AccountGuid { get; set; }

        public string CardNumber { get; set; }

        public string Token { get; set; }

        public decimal Amount { get; set; }

        public string TransactionNo { get; set; }

        public DateTime UTCTransactionDateTime { get; set; }
    }
}
