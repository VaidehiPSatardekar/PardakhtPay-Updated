namespace Pardakht.UserManagement.Shared.Models.WebService
{
    public class AuditLogStaffUserTimePerformance
    {
        public int IdleTime { get; set; }
        public int ActiveTime { get; set; }
        public int LogonTime { get; set; }
        public bool IsActive { get; set; }
        public string TenantPlatformMapGuid { get; set; }
        public string PlatformGuid { get; set; }
    }
}
