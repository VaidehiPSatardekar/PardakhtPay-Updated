using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class UnsupportedBin : BaseEntity
    {
        public int ApiType { get; set; }

        public string Bin { get; set; }
    }
}
