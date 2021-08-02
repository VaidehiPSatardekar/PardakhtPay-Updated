using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pardakht.UserManagement.Shared.Models.Infrastructure
{
    public class Role : EntityBase
    {
        public bool IsFixed { get; set; }
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; }
        [Required]
        [Column(TypeName = "char(1)")]
        public string RoleHolderTypeId { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string TenantGuid { get; set; }
        [Required]
        [Column(TypeName = "varchar(200)")]
        public string PlatformGuid { get; set; }
        public ICollection<UserPlatformRole> UserPlatformRoles { get;set; }
        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
