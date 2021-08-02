using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.Bot
{
    public class BotWithdrawalRequest
    {
        public int WithdrawalId { get; set; }

        [Encrypt]
        public string TransferFromAccount { get; set; }

        [Encrypt]
        public string TransferToAccount { get; set; }

        public decimal TransferBalance { get; set; }

        public int TransferPriority { get; set; }

        public int TransferType { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
