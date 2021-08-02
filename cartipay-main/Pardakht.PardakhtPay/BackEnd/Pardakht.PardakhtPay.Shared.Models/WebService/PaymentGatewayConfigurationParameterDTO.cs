using System.ComponentModel.DataAnnotations;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class PaymentGatewayConfigurationParameterDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Label { get; set; }

        public bool IsSecure { get; set; }

        public string Value { get; set; }
    }
}
