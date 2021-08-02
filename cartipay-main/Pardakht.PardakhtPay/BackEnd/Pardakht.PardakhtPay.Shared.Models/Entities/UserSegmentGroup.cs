using System;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class UserSegmentGroup : BaseEntity, ITenantGuid, IOwnerGuid, IDeletedEntity
    {
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsDefault { get; set; }

        public bool IsMalicious { get; set; }

        public DateTime CreateDate { get; set; }

        public string TenantGuid { get; set; }

        public string OwnerGuid { get; set; }

        public int Order { get; set; }
    }
}
