namespace Pardakht.UserManagement.Shared.Models.Configuration
{
    public class LockoutSettings
    {
        public int DefaultLockoutTimeSpan { get; set; } = 30;
        public int MaxFailedAccessAttempts { get; set; } = 5;
        public bool AllowedForNewUsers { get; set; } = true;
    }
}
