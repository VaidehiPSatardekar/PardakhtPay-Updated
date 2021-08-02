namespace Pardakht.PardakhtPay.Shared.Models.WebService.PaymentProxy
{
    public class SendOtpRequest
    {
        public string BankName { get; set; }

        public string PaymentGUID { get; set; }

        public string CardNumber { get; set; }

        public string BankSpecificInput { get; set; }

        public string Captcha { get; set; }
    }
}
