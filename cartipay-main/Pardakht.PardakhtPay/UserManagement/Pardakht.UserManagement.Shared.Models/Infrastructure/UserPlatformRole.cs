using System.ComponentModel.DataAnnotations.Schema;

namespace Pardakht.UserManagement.Shared.Models.Infrastructure
{
    public class UserPlatformRole
    {
        public int UserPlatformId { get; set; }
        [ForeignKey(nameof(UserPlatformId))]
        public UserPlatform UserPlatform { get; set; }
        public int RoleId { get; set; }
        [ForeignKey(nameof(RoleId))]
        public Role Role { get; set; }
    }
}
