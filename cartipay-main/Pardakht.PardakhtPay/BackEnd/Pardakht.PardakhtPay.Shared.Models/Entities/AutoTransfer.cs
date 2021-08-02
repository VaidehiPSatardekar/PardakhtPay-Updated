using System;
using System.ComponentModel.DataAnnotations;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class AutoTransfer : BaseEntity, ITenantGuid, IOwnerGuid
    {
        [MaxLength(70)]
        public string TenantGuid { get; set; }

        [MaxLength(70)]
        public string OwnerGuid { get; set; }

        public decimal Amount { get; set; }

        public int Status { get; set; }

        public string StatusDescription { get; set; }

        public int RequestId { get; set; }

        public string RequestGuid { get; set; }

        public int CardToCardAccountId { get; set; }

        [MaxLength(70)]
        public string AccountGuid { get; set; }

        public string TransferFromAccount { get; set; }

        public string TransferToAccount { get; set; }

        public int Priority { get; set; }

        public DateTime TransferRequestDate { get; set; }

        public DateTime? TransferredDate { get; set; }

        public DateTime? CancelDate { get; set; }

        public bool IsCancelled { get; set; }
    }
}
