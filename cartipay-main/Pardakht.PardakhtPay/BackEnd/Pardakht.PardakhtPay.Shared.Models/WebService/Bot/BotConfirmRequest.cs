using System;
using System.Collections.Generic;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Bot
{
    public class BotConfirmRequest
    {
        [Encrypt]
        public string ApiKey { get; set; }

        public string TransactionCode { get; set; }

        //[Encrypt]
        //public string DestinationAccountNumber { get; set; }

        //[Encrypt]
        //public string DestinationCardNumber { get; set; }

        public DateTime DateOfTransaction { get; set; }

        [Encrypt]
        public string SourceCardNumber { get; set; }

        public int TransactionId { get; set; }

        public decimal Amount { get; set; }

        public decimal? MinimumTransactionAmount { get; set; }

        public List<BankBotConfirmAccount> CardToCardAccounts { get; set; }
    }

    public class BankBotConfirmAccount
    {
        public string AccountNumber { get; set; }

        public string CardNumber { get; set; }

        public string AccountGuid { get; set; }

        public DateTime? BlockedDate { get; set; }
    }
}
