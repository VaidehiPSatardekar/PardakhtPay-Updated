using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class WithdrawalTransferHistoryDTO : BaseEntityDTO
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
