using System;
using System.ComponentModel.DataAnnotations.Schema;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class ManualTransferDetail : BaseEntity
    {
        public int ManualTransferId { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public string TransferGuid { get; set; }

        public int? TransferId { get; set; }

        public string TransferNotes { get; set; }

        public string TrackingNumber { get; set; }

        public int TransferStatus { get; set; }

        public DateTime? TransferDate { get; set; }

        public DateTime? TransferRequestDate { get; set; }

        public int? BankStatementId { get; set; }
    }
}
