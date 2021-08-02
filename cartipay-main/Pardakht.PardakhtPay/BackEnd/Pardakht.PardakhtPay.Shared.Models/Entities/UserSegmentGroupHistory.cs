using System;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class UserSegmentGroupHistory : BaseEntity
    {
        public DateTime CreationDate { get; set; }

        public int MerchantCustomerId { get; set; }

        public int? OldSegmentGroupId { get; set; }

        public int? NewSegmentGroupId { get; set; }
    }
}
