using System;
using System.ComponentModel.DataAnnotations;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class BlockedPhoneNumber : BaseEntity
    {
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public DateTime BlockedDate { get; set; }

        public string InsertUserId { get; set; }
    }
}
