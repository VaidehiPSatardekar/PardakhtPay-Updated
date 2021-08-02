using System;
using System.ComponentModel.DataAnnotations;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class BankStatementItem : BaseEntity
    {
        public int RecordId { get; set; }

        public DateTime CreationDate { get; set; }

        public int AccountId { get; set; }

        [Required]
        public string AccountGuid { get; set; }

        public int LoginId { get; set; }

        [Required]
        [StringLength(50)]
        public string LoginGuid { get; set; }

        [StringLength(50)]
        public string TransactionNo { get; set; }

        [StringLength(50)]
        public string CheckNo { get; set; }

        public DateTime TransactionDateTime { get; set; }

        public decimal Debit { get; set; }

        public decimal Credit { get; set; }

        public decimal Balance { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public DateTime InsertDateTime { get; set; }

        public int ConfirmedTransactionId { get; set; }

        [StringLength(100)]
        public string Notes { get; set; }

        public int? WithdrawalId { get; set; }

        //[ForeignKey(nameof(ConfirmedTransactionId))]
        //public Transaction Transaction { get; set; }
    }
}
