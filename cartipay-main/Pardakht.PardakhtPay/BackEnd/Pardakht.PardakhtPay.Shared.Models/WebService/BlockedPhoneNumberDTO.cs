using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class BlockedPhoneNumberDTO : BaseEntityDTO
    {
        public string PhoneNumber { get; set; }

        public DateTime BlockedDate { get; set; }
    }
}
