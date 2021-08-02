using System;

namespace Pardakht.PardakhtPay.Shared.Models.Models
{
    /// <summary>
    /// Represents a class which is used for our entities and has contains create and update information of this class
    /// </summary>
    public abstract class BaseCreateUpdateEntity : BaseCreateEntity, IUpdatedEntity, IUpdateUserEntity
    {
        public DateTime? UpdatedDate { get; set; }

        public int? UpdateUserId { get; set; }
    }
}
