using System;
using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Bot
{
    public class BotConfirmationResponse
    {
        public bool IsPresentInStatement { get; set; }

        public string Message { get; set; }

        public List<string> TransactonNumberOfStatements { get; set; }

        public DateTime? TransactionDateTime { get; set; }

        public bool IsConfirmedBefore { get; set; }

        public decimal? Debit { get; set; }

        public decimal? Credit { get; set; }

        public decimal? Balance { get; set; }

        public List<int> StatementIds { get; set; }
    }
}
