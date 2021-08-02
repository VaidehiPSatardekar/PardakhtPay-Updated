using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class ManualTransfer : BaseEntity, IOwnerGuid, ITenantGuid
    {
        public DateTime CreationDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        [MaxLength(70)]
        public string TenantGuid { get; set; }

        [MaxLength(70)]
        public string OwnerGuid { get; set; }

        public int CardToCardAccountId { get; set; }

        [NotMapped]
        public List<int> CardToCardAccountIds { get; set; }

        

        [MaxLength(4000)]
        public string AccountGuid { get; set; }

        public int TransferType { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        //public int TransferStatus { get; set; }

        //public int BankStatementId { get; set; }

        public int TransferAccountId { get; set; }

        public string ToAccountNo { get; set; }

        public string Iban { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Status { get; set; }

        public int Priority { get; set; }

        public DateTime? ProcessedDate { get; set; }

        public DateTime? CancelledDate { get; set; }

        public bool ImmediateTransfer { get; set; }

        public DateTime? ExpectedTransferDate { get; set; }

        public string Comment { get; set; }

        public string CreatorId { get; set; }

        public string UpdaterId { get; set; }

        public string CancellerId { get; set; }

        public bool TransferWholeAmount { get; set; }
    }

    public enum ManualTransferStatus
    {
        Pending = 1,
        Processing = 2,
        PartialSent = 3,
        Sent = 4,
        PartialCompleted = 5,
        Completed = 6,
        Cancelled = 7,
        CancelledByBlockedAccount = 8,
        CancelledByDeletedAccount = 9,
        InsufficientBalanceOrDailyLimit = 10,
        CancelledByBlockedORDeletedAccount = 11
    }
}
