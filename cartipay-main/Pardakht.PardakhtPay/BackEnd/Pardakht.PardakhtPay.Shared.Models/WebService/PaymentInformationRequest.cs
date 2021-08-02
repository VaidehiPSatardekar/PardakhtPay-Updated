using System.ComponentModel.DataAnnotations;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class PaymentInformationRequest
    {
        [Required]
        public string Token { get; set; }

        public string IpAddress { get; set; }

        public string DeviceKey { get; set; }
    }
}
