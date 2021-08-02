using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class BlockedCardNumberDTO : BaseEntityDTO
    {
        public string CardNumber { get; set; }

        public DateTime BlockedDate { get; set; }
    }
}
