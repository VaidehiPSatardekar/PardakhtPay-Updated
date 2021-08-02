using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class InvoicePayment : BaseEntity, IOwnerGuid, ITenantGuid, IDeletedEntity
    {
        [StringLength(70)]
        public string OwnerGuid { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        [StringLength(100)]
        public string PaymentReference { get; set; }

        public DateTime CreateDate { get; set; }

        [StringLength(70)]
        public string CreateUserId { get; set; }

        public DateTime? UpdateDate { get; set; }

        [StringLength(70)]
        public string UpdateUserId { get; set; }

        [StringLength(70)]
        public string TenantGuid { get; set; }

        public bool IsDeleted { get; set; }
    }
}
