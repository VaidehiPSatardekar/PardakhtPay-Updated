using System.Collections.Generic;

namespace Pardakht.UserManagement.Shared.Models.WebService
{
    public class PermissionGroupDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PlatformGuid { get; set; }
        //public string PlatformName { get; set; }
        public ICollection<PermissionDto> Permissions { get; set; }
    }
}
