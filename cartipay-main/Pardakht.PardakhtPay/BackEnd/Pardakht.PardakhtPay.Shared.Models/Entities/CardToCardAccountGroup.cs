using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class CardToCardAccountGroup : BaseEntity, ITenantGuid, IOwnerGuid, IDeletedEntity
    {
        public string Name { get; set; }

        public string TenantGuid { get; set; }

        public string OwnerGuid { get; set; }

        public bool IsDeleted { get; set; }
    }
}
