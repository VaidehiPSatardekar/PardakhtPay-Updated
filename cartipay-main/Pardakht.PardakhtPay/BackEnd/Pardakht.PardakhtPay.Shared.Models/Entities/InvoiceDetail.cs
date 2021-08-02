using System;
using System.ComponentModel.DataAnnotations.Schema;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class InvoiceDetail : BaseEntity
    {
        public DateTime CreateDate { get; set; }

        public int InvoiceId { get; set; }

        public int MerchantId { get; set; }

        public int ItemTypeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int TotalCount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Rate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }
    }

    public enum InvoiceDetailItemType
    {
        PardakhtPayDeposit = 1,
        PardakhtPalDeposit = 2,
        PardakhtPalWithdrawal = 3,
        Withdrawal = 4
    }
}
