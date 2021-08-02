using System;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class SadadPspDevice : BaseEntity
    {
        public int MerchantCustomerId { get; set; }

        public int ExternalId { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsRegistered { get; set; }

        public int TryCount { get; set; }

        public DateTime? RegistrationDate { get; set; }
    }
}
