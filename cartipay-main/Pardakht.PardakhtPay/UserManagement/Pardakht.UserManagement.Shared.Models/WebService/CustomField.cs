namespace Pardakht.UserManagement.Shared.Models.WebService
{
    public class CustomFieldDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsMandatory { get; set; }
        public int CustomFieldTypeId { get; set; }
        public string Regex { get; set; }
    }
}
