using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class InvoiceOwnerSetting : BaseEntity, IOwnerGuid, ITenantGuid, IDeletedEntity
    {
        public bool IsActive { get; set; }

        [StringLength(70)]
        public string OwnerGuid { get; set; }

        public DateTime StartDate { get; set; }

        public int InvoicePeriod { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal PardakhtPayDepositRate { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal PardakhtPalDepositRate { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal PardakhtPalWithdrawalRate { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal WithdrawalRate { get; set; }

        [StringLength(70)]
        public string TenantGuid { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string CreateUserId { get; set; }

        public string UpdateUserId { get; set; }

        public bool IsDeleted { get; set; }
    }

    public enum InvoicePeriods
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3
    }
}
