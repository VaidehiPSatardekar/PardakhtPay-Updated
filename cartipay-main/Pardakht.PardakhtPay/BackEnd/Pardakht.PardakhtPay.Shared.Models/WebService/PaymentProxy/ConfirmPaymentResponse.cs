namespace Pardakht.PardakhtPay.Shared.Models.WebService.PaymentProxy
{
    public class ConfirmPaymentResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public double ConfirmPayResult { get; set; }
    }
}
