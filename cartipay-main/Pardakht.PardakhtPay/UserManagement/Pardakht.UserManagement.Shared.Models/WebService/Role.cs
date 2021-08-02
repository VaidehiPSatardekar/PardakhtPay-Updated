using System.Collections.Generic;

namespace Pardakht.UserManagement.Shared.Models.WebService
{
   public class RoleDto
    {
        public int Id { get; set; }
        public bool IsFixed { get; set; }
        public string Name { get; set; }
        public string RoleHolderTypeId { get; set; }
        public string TenantGuid { get; set; }
        public string TenancyName { get; set; }
        public string PlatformGuid { get; set; }
        public ICollection<PermissionDto> Permissions { get; set; }
    }
}
