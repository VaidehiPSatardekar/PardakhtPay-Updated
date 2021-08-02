using System;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class TransferAccount : BaseEntity, ITenantGuid, IOwnerGuid, IDeletedEntity
    {
        public DateTime CreationDate { get; set; }

        public string TenantGuid { get; set; }

        public string AccountNo { get; set; }

        //public string AccountHolderName { get; set; }

        public string AccountHolderFirstName { get; set; }

        public string AccountHolderLastName { get; set; }

        public string Iban { get; set; }

        public string FriendlyName { get; set; }

        public bool IsActive { get; set; }

        public string OwnerGuid { get; set; }

        public bool IsDeleted { get; set; }

        //[ForeignKey(nameof(TenantId))]
        //public Tenant Tenant { get; set; }
    }
}
