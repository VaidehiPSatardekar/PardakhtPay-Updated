using System;
using System.ComponentModel.DataAnnotations;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class Payment780Device : BaseEntity
    {
        public int MerchantCustomerId { get; set; }

        public int ExternalId { get; set; }

        [StringLength(100)]
        public string PhoneNumber { get; set; }

        public bool IsRegistered { get; set; }

        public int TryCount { get; set; }

        public DateTime? RegistrationDate { get; set; }
    }
}
