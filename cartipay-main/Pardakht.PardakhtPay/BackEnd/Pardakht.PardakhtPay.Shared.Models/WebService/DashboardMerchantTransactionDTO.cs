namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class DashboardMerchantTransactionDTO
    {
        public string Title { get; set; }

        public decimal? TransactionSum { get; set; }

        public int? TransactionCount { get; set; }

        public decimal? WithdrawalSum { get; set; }

        public int? WithdrawalCount { get; set; }
    }

    public class DashboardPaymentTypeBreakDown
    {
        public string Title { get; set; }

        public decimal? TransactionSum { get; set; }

        public int? TransactionCount { get; set; }

        public decimal? WithdrawalSum { get; set; }

        public int? WithdrawalCount { get; set; }
    }
}
