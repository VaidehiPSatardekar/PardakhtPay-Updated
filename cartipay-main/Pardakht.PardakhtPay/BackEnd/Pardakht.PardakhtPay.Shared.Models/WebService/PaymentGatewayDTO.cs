using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class PaymentGatewayDTO : BaseEntityDTO
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public List<PaymentGatewayParameterDTO> ParameterItems { get; set; }
    }
}
