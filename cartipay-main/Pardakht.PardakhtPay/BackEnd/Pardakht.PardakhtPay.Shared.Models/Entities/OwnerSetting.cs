using System;
using System.ComponentModel.DataAnnotations;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class OwnerSetting : BaseEntity, IOwnerGuid
    {
        [StringLength(70)]
        public string OwnerGuid { get; set; }

        public DateTime CreateDate { get; set; }

        [StringLength(70)]
        public string CreateUserId { get; set; }

        public DateTime UpdateDate { get; set; }

        [StringLength(70)]
        public string UpdateUserId { get; set; }

        public bool WaitAmountForCurrentWithdrawal { get; set; }
    }
}
