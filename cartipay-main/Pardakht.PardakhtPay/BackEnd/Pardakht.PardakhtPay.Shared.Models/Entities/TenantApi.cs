using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class TenantApi : BaseEntity
    {
        public string TenantGuid { get; set; }

        public int MerchantId { get; set; }

        public string ApiUrl { get; set; }

        public bool IsServiceUrl { get; set; }

        public bool IsPaymentUrl { get; set; }
    }
}
