namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class DailyAccountInformation
    {
        public int BankLoginId { get; set; }

        public int BankAccountId { get; set; }

        public string FriendlyName { get; set; }

        public string LoginGuid { get; set; }

        public string AccountGuid { get; set; }

        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public decimal TotalDeposit { get; set; }

        public decimal TotalWithdrawal { get; set; }

        public decimal? PendingWithdrawalAmount { get; set; }
    }
}
