namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class DashboardAccountStatusDTO
    {
        public int BankLoginId { get; set; }

        public int BankAccountId { get; set; }

        public string AccountGuid { get; set; }

        public string FriendlyName { get; set; }

        public string AccountNo { get; set; }

        public string BankName { get; set; }

        public bool IsBlocked { get; set; }

        public string Status { get; set; }

        public string CardHolderName { get; set; }

        public string CardNumber { get; set; }

        public long AccountBalance { get; set; }

        public long BlockedBalance { get; set; }

        public long NormalWithdrawable { get; set; }

        public long PayaWithdrawable { get; set; }

        public long SatnaWithdrawable { get; set; }

        public long TotalDepositToday { get; set; }

        public long TotalWithdrawalToday { get; set; }

        public decimal? PendingWithdrawalAmount { get; set; }
    }
}
