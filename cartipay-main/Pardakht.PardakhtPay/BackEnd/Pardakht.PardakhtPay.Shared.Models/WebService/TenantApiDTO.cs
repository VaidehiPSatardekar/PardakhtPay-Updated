namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class TenantApiDTO : BaseEntityDTO
    {
        public string TenantGuid { get; set; }

        public int MerchantId { get; set; }

        public string ApiUrl { get; set; }

        public bool IsServiceUrl { get; set; }

        public bool IsPaymentUrl { get; set; }
    }
}
