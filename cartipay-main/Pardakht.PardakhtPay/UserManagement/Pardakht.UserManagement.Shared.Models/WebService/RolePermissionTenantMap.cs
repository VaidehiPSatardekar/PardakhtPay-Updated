namespace Pardakht.UserManagement.Shared.Models.WebService
{
   public  class RolePermissionTenantMapDto
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public int TenantId { get; set; }
    }


    public class RolePermissionTenantMapByTenant
    {
        public int RoleId { get; set; }
        public int[] PermissionId { get; set; }
    }
}
