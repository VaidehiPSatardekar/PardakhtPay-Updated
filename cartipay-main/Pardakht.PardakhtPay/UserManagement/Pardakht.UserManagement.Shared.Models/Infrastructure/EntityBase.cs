using System.ComponentModel.DataAnnotations;

namespace Pardakht.UserManagement.Shared.Models.Infrastructure
{
    public class EntityBase : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        //public long? CreatorUserId { get; set; }
        //public DateTime? CreationTime { get; set; }
        //public long? DeleterUserId { get; set; }
        //public DateTime? DeletionTime { get; set; }
        //public bool? IsActive { get; set; }
        //public bool? IsDeleted { get; set; }
        //public long? LastModifierUserId { get; set; }
        //public DateTime? LastModificationTime { get; set; }

    }
}
