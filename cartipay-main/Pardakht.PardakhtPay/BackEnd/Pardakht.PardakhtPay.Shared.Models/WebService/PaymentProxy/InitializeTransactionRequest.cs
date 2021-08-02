namespace Pardakht.PardakhtPay.Shared.Models.WebService.PaymentProxy
{
    public class InitializeTransactionRequest
    {
        public string BankName { get; set; }

        public long PaymentAmount { get; set; }

        public string PaymentReference { get; set; }

        public string MerchantId { get; set; }

        public string RedirectURL { get; set; }

        public string BankSpecificInput { get; set; }

        public string TerminalId { get; set; }

        public string MerchantKey { get; set; }
    }
}
