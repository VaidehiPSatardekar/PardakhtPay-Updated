namespace Pardakht.UserManagement.Shared.Models.WebService
{
   public class PermissionDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsRestricted { get; set; }
    }
}
