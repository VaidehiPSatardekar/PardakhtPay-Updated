namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class DashboardTransactionByTypeDTO
    {
        public string Title { get; set; }

        public decimal? TransactionSum { get; set; }

        public int? TransactionCount { get; set; }

        public decimal? WithdrawalSum { get; set; }

        public int? WithdrawalCount { get; set; }
    }
}
