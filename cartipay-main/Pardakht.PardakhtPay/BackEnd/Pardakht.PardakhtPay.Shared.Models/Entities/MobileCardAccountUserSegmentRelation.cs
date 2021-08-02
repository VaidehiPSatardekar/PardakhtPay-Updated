using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class MobileCardAccountUserSegmentRelation : BaseEntity
    {
        public int MobileTransferCardAccountGroupItemId { get; set; }

        public int UserSegmentGroupId { get; set; }
    }
}
