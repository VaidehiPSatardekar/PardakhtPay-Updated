using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pardakht.UserManagement.Shared.Models.Infrastructure
{
    public class Platform : EntityBase
    {
        [Required]
        [Column(TypeName = "varchar(100)")]
        public string Name { get; set; }
        [Required]
        [Column(TypeName = "varchar(200)")]
        public string PlatformGuid { get; set; }
        [Required]
        public string JwtKey { get; set; }
    }
}
