using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class CardToCardUserSegmentRelation : BaseEntity
    {
        public int CardToCardAccountGroupItemId { get; set; }

        public int UserSegmentGroupId { get; set; }
    }
}
