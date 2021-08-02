using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class PaymentGatewayConfigurationDTO : BaseEntityDTO
    {
        public int PaymentGatewayId { get; set; }

        public int ExpireTime { get; set; }

        public string PaymentGatewayGuid { get; set; }

        public List<PaymentGatewayConfigurationParameterDTO> ParameterItems { get; set; }
    }
}
