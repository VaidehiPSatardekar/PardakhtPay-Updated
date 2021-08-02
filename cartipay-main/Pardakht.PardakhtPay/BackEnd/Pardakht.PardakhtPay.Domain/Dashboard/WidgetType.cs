namespace Pardakht.PardakhtPay.Domain.Dashboard
{
    /// <summary>
    /// Defines the dashboard widget types
    /// </summary>
    public enum WidgetType
    {
        /// <summary>
        /// Sales report for dashboard transactions
        /// </summary>
        SalesReport = 1,
        /// <summary>
        /// Sales report groupping with merchant
        /// </summary>
        SalesForMerchants = 2,
        Accounting = 3,
        AccountStatusReport = 4,
        DepositBreakDownReport = 5,
        WithdrawPaymentWidget =6
    }
}
