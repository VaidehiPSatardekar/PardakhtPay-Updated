using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Shared.Models.Configuration
{
    [Setting(Key = ApplicationSettingKeys.Malicious)]
    public class MaliciousCustomerSettings : BaseEntityDTO
    {
        public string FakeCardNumber { get; set; }

        public string FakeCardHolderName { get; set; }
    }
}
