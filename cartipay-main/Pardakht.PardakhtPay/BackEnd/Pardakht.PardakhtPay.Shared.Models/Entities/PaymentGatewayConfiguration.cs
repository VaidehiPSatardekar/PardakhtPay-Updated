using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class PaymentGatewayConfiguration : BaseEntity
    {
        public int PaymentGatewayId { get; set; }

        public int ExpireTime { get; set; }

        public string Configuration { get; set; }

        public string PaymentGatewayGuid { get; set; }
    }

    public static class PaymentGatewayConfigurationParameters
    {
        public const string ApiKey = "apiKey";
        public const string AccountId = "accountId";
        public const string Username = "username";
        public const string Password = "pasword";
    }
}
