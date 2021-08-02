using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class Invoice : BaseEntity, IOwnerGuid, ITenantGuid
    {
        public DateTime CreateDate { get; set; }

        public DateTime DueDate { get; set; }

        [StringLength(70)]
        public string OwnerGuid { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [StringLength(70)]
        public string TenantGuid { get; set; }
    }
}
