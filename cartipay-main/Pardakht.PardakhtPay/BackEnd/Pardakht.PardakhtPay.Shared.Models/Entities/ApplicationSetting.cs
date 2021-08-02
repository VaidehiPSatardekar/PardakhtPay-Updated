using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class ApplicationSetting : BaseEntity
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}
