using System.ComponentModel.DataAnnotations;

namespace Pardakht.PardakhtPay.Shared.Models.Models
{
    /// <summary>
    /// Defines base entity with Id
    /// </summary>
    public interface IEntity
    {
        [Key]
        int Id { get; set; }
    }
}
