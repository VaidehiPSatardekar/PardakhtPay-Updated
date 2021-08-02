namespace Pardakht.PardakhtPay.Shared.Models.WebService.PaymentProxy
{
    public class SendPaymentResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public string PaymentReference { get; set; }
    }

    public class SendPaymentErrorResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public string ErrorType { get; set; }
    }
}
