using System.ComponentModel.DataAnnotations;

namespace Pardakht.PardakhtPay.RestApi.Model
{
    public class OtpModel
    {
        [Required]
        public string Token { get; set; }

        public string CardNo { get; set; }

        public int Amount { get; set; }

        public string Captcha { get; set; }
    }
}
