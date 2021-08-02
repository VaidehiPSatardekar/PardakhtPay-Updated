using System.ComponentModel.DataAnnotations;

namespace Pardakht.UserManagement.Shared.Models.Infrastructure
{
    public class ClaimType : IEntity
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
