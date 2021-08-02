using System;

namespace Pardakht.PardakhtPay.Shared.Models.Models
{
    /// <summary>
    /// Represents a class which is used for base entity and create information about entity
    /// </summary>
    public abstract class BaseCreateEntity : BaseEntity, ICreatorUserEntity, ICreationDateEntity
    {
        public int CreatorUserId { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
