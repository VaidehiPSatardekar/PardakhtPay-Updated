namespace Pardakht.PardakhtPay.Shared.Models.WebService.Sms
{
    public class SmsVerifyModel
    {
        public string InvoiceKey { get; set; }

        public string VerifyCode { get; set; }

        public bool IsWrongCode { get; set; }

        public int Seconds { get; set; }
    }
}
