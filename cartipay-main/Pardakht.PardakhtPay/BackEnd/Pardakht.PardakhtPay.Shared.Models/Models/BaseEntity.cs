using System.ComponentModel.DataAnnotations;

namespace Pardakht.PardakhtPay.Shared.Models.Models
{
    /// <summary>
    /// Represents and abstract class which is used for our entities
    /// </summary>
    public abstract class BaseEntity : IEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
