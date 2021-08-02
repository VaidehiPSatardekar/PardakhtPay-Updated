using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pardakht.UserManagement.Shared.Models.Infrastructure
{
    public class Permission : EntityBase
    {
        [Column(TypeName = "varchar(50)")]
        [Required]
        public string Code { get; set; }
        public bool IsRestricted { get; set; }
        [Column(TypeName = "varchar(100)")]
        [Required]
        public string Name { get; set; }
        public int PermissionGroupId { get; set; }
        public PermissionGroup PermissionGroup { get; set; }
        [Column(TypeName = "varchar(200)")]
        public string PlatformGuid { get; set; } // TODO: this should be deleted as it should be dependent on permissiongroup?
    }
}
