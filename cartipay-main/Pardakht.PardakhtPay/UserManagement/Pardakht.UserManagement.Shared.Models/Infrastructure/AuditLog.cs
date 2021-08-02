using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pardakht.UserManagement.Shared.Models.Infrastructure
{
    public enum AuditType
    {
        User,
        Group,
        Role,
        Tenant,
        Ticket,
        Provider,
        Rule,
        CannedResponse,
        Sla,
        TicketCategory,
        Product,
        Domain,
        CloudFlare
    }

    public enum AuditActionType
    {
        UserCreated = 0,
        UserUpdated = 1,
        UserLogin = 2,
        UserLogout = 3,
        UserForgotPassword = 4,
        UserResetPassword = 5,
        UserBlocked = 6,
        UserUnblocked = 7,
        UserDeleted = 8,

        UserPerformanceActivity = 20,

        GroupCreated = 100,

        RoleCreated = 200,
        RoleUpdated = 201,
        TicketCreated = 202,
        TicketUpdated = 203,
        DomainCreated = 204,
        DomainUpdated = 205,
        CloudFlareCreated = 206,
        CloudFlareUpdated = 207,
        StaffUserCreated = 238,
        StaffUserUpdated = 239,
        StaffUserDeleted = 240,
        StaffUserLoginAs = 260

    }

    public class AuditLog : EntityBase
    {
        public AuditType Type { get; set; }
        public AuditActionType ActionType { get; set; }
        public int TypeId { get; set; }
        public string Message { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public StaffUser StaffUser { get; set; }
        public DateTime DateTime { get; set; }
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
