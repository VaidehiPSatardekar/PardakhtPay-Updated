namespace Pardakht.UserManagement.Shared.Models.WebService
{
    public class TenantCustomFieldDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CustomFieldTenantMapId { get; set; }
        public string CustomFieldTenantMapGuid { get; set; }

        public string CustomFieldValue { get; set; }
    }
}
