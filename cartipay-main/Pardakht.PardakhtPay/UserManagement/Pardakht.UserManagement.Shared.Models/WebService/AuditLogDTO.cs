using System.ComponentModel.DataAnnotations;

namespace Pardakht.UserManagement.Shared.Models.WebService
{
    public class AuditLogDTO
    {
        public int IdleTime { get; set; }
        public int ActiveTime { get; set; }
        public int LogonTime { get; set; }
        public bool IsActive { get; set; }

        [StringLength(100)]
        public string TenantPlatformMapGuid { get; set; }
        [StringLength(100)]
        public string PlatformGuid { get; set; }
    }
}
