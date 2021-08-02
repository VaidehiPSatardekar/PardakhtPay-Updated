using System;
using System.ComponentModel.DataAnnotations;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class BlockedCardNumber : BaseEntity
    {
        [MaxLength(20)]
        [Required]
        public string CardNumber { get; set; }

        public DateTime BlockedDate { get; set; }

        [MaxLength(70)]
        [Required]
        public string InsertUserId { get; set; }
    }
}
