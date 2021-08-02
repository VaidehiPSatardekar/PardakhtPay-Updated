using System.ComponentModel.DataAnnotations;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class PaymentGateway : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [MaxLength(30)]
        public string Code { get; set; }

        [Required]
        public string Parameters { get; set; }
    }

    public static class PaymentGatewayCodes
    {
        public const string CardToCard = "C2C";
    }
}
