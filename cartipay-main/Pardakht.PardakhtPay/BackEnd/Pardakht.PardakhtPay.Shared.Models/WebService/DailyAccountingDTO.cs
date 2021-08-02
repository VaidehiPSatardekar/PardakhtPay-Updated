using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class DailyAccountingDTO
    {
        public string MerchantTitle { get; set; }

        public string AccountNumber { get; set; }

        public DateTime Date { get; set; }

        public int Count { get; set; }

        public decimal? Amount { get; set; }

        public decimal? WithdrawalAmount { get; set; }

        public int WithdrawalCount { get; set; }

        public string TenantGuid { get; set; }

        public string CardNumber { get; set; }

        public string CardHolderName { get; set; }

        public bool IsCustomerPayment { get; set; }

        public decimal DepositPercentage { get; set; }

        public decimal WithdrawalPercentage { get; set; }
    }

    public class AccountingSearchArgs : AgGridSearchArgs
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int GroupType { get; set; }
    }

    public enum AccountingGroupingType
    {
        Merchant = 1,
        Account = 2
    }
}
