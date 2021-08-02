using System.ComponentModel.DataAnnotations;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class RiskyKeyword : BaseEntity
    {
        [StringLength(200)]
        [Required]
        public string Keyword { get; set; }
    }
}
