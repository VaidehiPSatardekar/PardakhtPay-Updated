using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Bot
{
    public class BotBlockedCardDetail
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public string AccountNo { get; set; }

        public string CardNumber { get; set; }

        public DateTime TimeStamp { get; set; }
    }
}
