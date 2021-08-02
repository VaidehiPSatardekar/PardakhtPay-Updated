using System.ComponentModel.DataAnnotations;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class LanguageDTO : BaseEntityDTO
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
