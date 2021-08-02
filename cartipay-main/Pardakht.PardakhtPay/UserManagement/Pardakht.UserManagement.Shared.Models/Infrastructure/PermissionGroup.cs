using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pardakht.UserManagement.Shared.Models.Infrastructure
{
    public class PermissionGroup : EntityBase
    {
        [Column(TypeName = "varchar(100)")]
        [Required]
        public string Name { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string PlatformGuid { get; set; }
        public ICollection<Permission> Permissions { get; set; }
    }
}
