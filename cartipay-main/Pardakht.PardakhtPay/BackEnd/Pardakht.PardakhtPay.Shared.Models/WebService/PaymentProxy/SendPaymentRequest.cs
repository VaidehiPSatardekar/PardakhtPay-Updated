using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.WebService.PaymentProxy
{
    public class SendPaymentRequest
    {
        public string BankName { get; set; }

        public string PaymentGUID { get; set; }

        [Encrypt]
        public string CardNumber { get; set; }

        [Encrypt]
        public string CardPIN { get; set; }

        [Encrypt]
        public string CardCvv2 { get; set; }

        [Encrypt]
        public string CardExpireYear { get; set; }

        public string CardExpireMonth { get; set; }

        public string BankSpecificInput { get; set; }

        public string Captcha { get; set; }
    }
}
