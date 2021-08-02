using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class Withdrawal : BaseEntity, ITenantGuid, IOwnerGuid
    {
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal RemainingAmount { get; set; }

        [MaxLength(70)]
        [Required]
        public string TenantGuid { get; set; }

        public int TransferAccountId { get; set; }

        [StringLength(2000)]
        public string FromAccountNumber { get; set; }

        [StringLength(2000)]
        public string ToAccountNumber { get; set; }

        [StringLength(2000)]
        public string ToIbanNumber { get; set; }

        [StringLength(2000)]
        public string CardNumber { get; set; }

        public int Priority { get; set; }

        [StringLength(2000)]
        public string FirstName { get; set; }

        [StringLength(2000)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string TransferRequestGuid { get; set; }

        [MaxLength(50)]
        public string TransferNotes { get; set; }

        public int TransferStatus { get; set; }

        [MaxLength(2000)]
        public string TransferStatusDescription { get; set; }

        public int TransferType { get; set; }

        public DateTime ExpectedTransferDate { get; set; }

        public DateTime? TransferRequestDate { get; set; }

        public DateTime? TransferDate { get; set; }

        public int? TransferId { get; set; }

        [MaxLength(70)]
        [Required]
        public string OwnerGuid { get; set; }

        public int WithdrawalStatus { get; set; }

        public int MerchantId { get; set; }

        public int? MerchantCustomerId { get; set; }

        [MaxLength(500)]
        public string ReturnUrl { get; set; }

        [StringLength(100)]
        public string Reference { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public DateTime? CancelDate { get; set; }

        [StringLength(50)]
        public string TrackingNumber { get; set; }

        [StringLength(50)]
        public string AccountGuid { get; set; }

        [MaxLength(100)]
        public string Description { get; set; }

        [NotMapped]
        [MaxLength(2000)]
        public string RequestContent { get; set; }

        public int? BankStatementItemId { get; set; }

        public int WithdrawalProcessType { get; set; }

        public int CardToCardTryCount { get; set; }

        [MaxLength(70)]
        public string UpdateUserId { get; set; }
    }

    public enum WithdrawalStatus
    {
        Pending = 1,
        CancelledByUser = 2,
        CancelledBySystem = 3,
        Confirmed = 4,
        PendingBalance = 5,
        Sent = 6,
        Refunded = 7,
        PendingCardToCardConfirmation = 8,
        PartialPaid = 9
    }

    public enum WithdrawalProcessType
    {
        Transfer = 1,
        CardToCard = 2,
        Both = 3
    }
}
