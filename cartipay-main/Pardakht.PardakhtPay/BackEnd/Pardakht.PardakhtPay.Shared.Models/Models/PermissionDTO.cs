using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Shared.Models.Models
{


    public class PermissionDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public string Code { get; set; }
        public bool IsRestricted { get; set; }
        public bool IsSelected { get; set; }
    }

    public class PermissionGroupDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<PermissionDTO> Permissions { get; set; }
    }
}
