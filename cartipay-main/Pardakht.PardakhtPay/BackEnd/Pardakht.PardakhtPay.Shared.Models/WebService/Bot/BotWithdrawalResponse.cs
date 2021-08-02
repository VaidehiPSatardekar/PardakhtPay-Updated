using System;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Bot
{
    public class BotWithdrawalResponse
    {
        public int Id { get; set; }

        public string TransferRequestGuid { get; set; }

        [Encrypt]
        public string TransferFromAccount { get; set; }

        [Encrypt]
        public string TransferToAccount { get; set; }

        public decimal TransferBalance { get; set; }

        public int TransferPriority { get; set; }

        public string TransferNotes { get; set; }

        public int TransferStatus { get; set; }

        public string TransferStatusDescription { get; set; }

        public string TransferType { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime TransferRequestDate { get; set; }
    }
}
