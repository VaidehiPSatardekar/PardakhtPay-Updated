using System.ComponentModel.DataAnnotations;

namespace Pardakht.UserManagement.Shared.Models.Infrastructure
{
    public interface IEntity
    {
        [Key]
        int Id { get; set; }
    }
}