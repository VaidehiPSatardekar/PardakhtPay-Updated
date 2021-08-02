using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class CardToCardAccount : BaseEntity, ITenantGuid, IOwnerGuid
    {
        [Required]
        public string LoginGuid { get; set; }

        [Required]
        public string AccountGuid { get; set; }

        //[Required]
        public string CardNumber { get; set; }

        [Required]
        public string CardHolderName { get; set; }

        //[Required]
        public string SafeAccountNumber { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TransferThreshold { get; set; }

        public bool IsActive { get; set; }

        public bool IsTransferThresholdActive { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? TransferThresholdLimit { get; set; }

        public string TenantGuid { get; set; }

        public string OwnerGuid { get; set; }

        public int LoginType { get; set; }

        public bool SwitchOnLimit { get; set; }

        public bool SwitchIfHasReserveAccount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? SwitchLimitAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? SwitchCreditDailyLimit { get; set; }
    }
}
