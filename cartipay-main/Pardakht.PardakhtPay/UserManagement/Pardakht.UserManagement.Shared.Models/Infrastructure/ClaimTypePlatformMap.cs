using System.ComponentModel.DataAnnotations;

namespace Pardakht.UserManagement.Shared.Models.Infrastructure
{
    public class ClaimTypePlatformMap : IEntity
    {
        [Key]
        public int Id { get; set; }
        public int ClaimTypeId { get; set; }
        public ClaimType ClaimType { get; set; }
        public int PlatformId { get; set; }
     //   public Platform Platform { get; set; }

    }
}
