using System.Collections.Generic;

namespace Pardakht.UserManagement.Shared.Models.WebService
{
    public class UserPlatformDto
    {
        public int Id { get; set; }
        public int StaffUserId { get; set; }
        public string PlatformGuid { get; set; }
        public ICollection<UserPlatformRoleDto> UserPlatformRoles { get; set; }
    }
    public class UserPlatformRoleDto
    {
        public int UserPlatformId { get; set; }
        public int RoleId { get; set; }
    }
}
