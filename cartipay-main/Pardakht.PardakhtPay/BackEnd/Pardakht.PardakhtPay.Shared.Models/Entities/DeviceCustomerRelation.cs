using System;
using System.ComponentModel.DataAnnotations;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class DeviceMerchantCustomerRelation : BaseEntity, ITenantGuid, IOwnerGuid
    {
        [StringLength(200)]
        [Required]
        public string DeviceKey { get; set; }

        public int MerchantCustomerId { get; set; }

        public DateTime CreateDate { get; set; }

        public int TransactionId { get; set; }

        [StringLength(200)]
        [Required]
        public string TenantGuid { get; set; }

        [StringLength(200)]
        [Required]
        public string OwnerGuid { get; set; }
    }
}
