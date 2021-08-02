namespace Pardakht.PardakhtPay.Shared.Models.WebService.Sms
{
    public class SmsServiceRequest
    {
        public string Message { get; set; }
        public string PhoneNumber { get; set; }
        public string UserApiKey { get; set; }
        public string SecretKey { get; set; }
        public string TemplateId { get; set; }
    }
}
