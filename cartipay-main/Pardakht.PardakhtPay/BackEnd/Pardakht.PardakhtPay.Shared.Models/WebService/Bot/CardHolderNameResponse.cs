using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Bot
{
    public class CardHolderNameResponse
    {
        public int Id { get; set; }

        public DateTime RequestDate { get; set; }

        public string CardORIBANNumber { get; set; }

        public int Type { get; set; }

        public int Status { get; set; }

        public DateTime? UpdateTimeStamp { get; set; }

        public string Name { get; set; }

        public string BankName { get; set; }

        public int LoginIdToGetCallbackURL { get; set; }

        public string CallbackUrl { get; set; }
    }
}
