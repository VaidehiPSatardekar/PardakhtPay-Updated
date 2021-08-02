using System;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class WithdrawalTransferHistory : BaseEntity
    {
        public int WithdrawalId { get; set; }

        public int TransferId { get; set; }

        public string TransferNotes { get; set; }

        public long Amount { get; set; }

        public int TransferStatus { get; set; }

        public string TransferStatusDescription { get; set; }

        public DateTime RequestedDate { get; set; }

        public DateTime LastCheckDate { get; set; }
    }
}
