using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class MobileTransferCardAccountGroup : BaseEntity, ITenantGuid, IOwnerGuid, IDeletedEntity
    {
        public string TenantGuid { get; set; }

        public string OwnerGuid { get; set; }

        public bool IsDeleted { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
