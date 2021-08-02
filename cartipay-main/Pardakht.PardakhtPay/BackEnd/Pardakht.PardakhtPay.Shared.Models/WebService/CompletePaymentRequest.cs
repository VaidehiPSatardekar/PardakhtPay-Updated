using System.ComponentModel.DataAnnotations;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class CompletePaymentRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string CustomerCardNumber { get; set; }

        public string Cvv2 { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public string Pin { get; set; }

        public string IpAddress { get; set; }

        public string CaptchaCode { get; set; }
    }
    public class CompleteMobilePaymentRequest
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string CustomerCardNumber { get; set; }

        [Required]
        public string Cvv2 { get; set; }

        [Required]
        public string Month { get; set; }

        [Required]
        public string Year { get; set; }

        [Required]
        public string Pin { get; set; }

        public string IpAddress { get; set; }

        public string CaptchaCode { get; set; }
    }

    public class CancelPaymentRequest
    {
        [Required]
        public string Token { get; set; }
    }
}
