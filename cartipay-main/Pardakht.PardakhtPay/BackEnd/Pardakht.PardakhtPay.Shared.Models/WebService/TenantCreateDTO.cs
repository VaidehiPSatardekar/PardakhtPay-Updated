using System.ComponentModel.DataAnnotations;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class TenantCreateDTO : BaseEntityDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        //[Required]
        //[MaxLength(100)]
        //public string ContactName { get; set; }

        //[Required]
        //[MaxLength(100)]
        //public string ContactPhoneNumber { get; set; }

        //[Required]
        //[MaxLength(100)]
        //public string ContactEmail { get; set; }

        [MaxLength(200, ErrorMessage = "Username must has lower than 2000 characters")]
        public string TenantAdminUsername { get; set; }

        [MaxLength(200, ErrorMessage = "Admin email must has lower than 2000 characters")]
        public string TenantAdminEmail { get; set; }

        [MaxLength(200, ErrorMessage = "Admin password must has lower than 2000 characters")]
        public string TenantAdminPassword { get; set; }

        public string TenantAdminFirstName { get; set; }

        public string TenantAdminLastName { get; set; }
    }
}
