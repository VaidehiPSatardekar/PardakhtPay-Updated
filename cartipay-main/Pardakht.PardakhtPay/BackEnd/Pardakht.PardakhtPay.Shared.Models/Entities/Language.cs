using System.ComponentModel.DataAnnotations;
using Pardakht.PardakhtPay.Shared.Models.Models;

namespace Pardakht.PardakhtPay.Shared.Models.Entities
{
    public class Language : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string DisplayName { get; set; }

        [Required]
        [MaxLength(5)]
        public string ShortCode { get; set; }

        [Required]
        [MaxLength(50)]
        public string Flag { get; set; }

        public bool Rtl { get; set; }
    }
}
