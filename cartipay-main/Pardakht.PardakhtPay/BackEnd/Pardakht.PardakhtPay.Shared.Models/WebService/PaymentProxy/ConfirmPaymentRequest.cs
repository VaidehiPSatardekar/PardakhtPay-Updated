namespace Pardakht.PardakhtPay.Shared.Models.WebService.PaymentProxy
{
    public class ConfirmPaymentRequest
    {
        public string BankName { get; set; }

        public string PaymentGUID { get; set; }

        public string BankSpecificInput { get; set; }
    }
}
