namespace Pardakht.UserManagement.Shared.Models.Infrastructure
{
    public class RolePermission : EntityBase
    {
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public int PermissionId { get; set; }
        public Permission Permission { get; set; }
        //public string TenantGuid { get; set; }
        //public string PlatformGuid { get; set; }
    }
}
