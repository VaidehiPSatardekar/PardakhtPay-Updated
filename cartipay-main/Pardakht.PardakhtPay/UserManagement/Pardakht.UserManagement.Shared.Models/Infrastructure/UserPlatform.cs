using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pardakht.UserManagement.Shared.Models.Infrastructure
{
    public class UserPlatform : EntityBase
    {
        [Required]
        public int StaffUserId { get; set; }
        [ForeignKey(nameof(StaffUserId))]
        public StaffUser StaffUser { get; set; }
        [Required]
        [Column(TypeName = "varchar(200)")]
        public string PlatformGuid { get; set; }
        public ICollection<UserPlatformRole> UserPlatformRoles { get; set; }
    }
}
