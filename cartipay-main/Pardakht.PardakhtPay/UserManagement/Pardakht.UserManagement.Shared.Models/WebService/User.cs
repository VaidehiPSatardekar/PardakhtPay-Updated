using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Shared.Models.WebService
{
    public class ForgotResetRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string PlatformGuid { get; set; }
    }
    public class ForgotResetByUsernameRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string PlatformGuid { get; set; }
    }

    public class PasswordResetRequest
    {
        [Required]
        public string AccountId { get; set; }
        [Required]
        public string PlatformGuid { get; set; }
    }

    public class PasswordResetResponse
    {
        public string NewPassword { get; set; }
        public string Message { get; set; }
    }

    public class PasswordChangeResponse
    {
        public string UserId { get; set; }
        public string Message { get; set; }
    }

    public class PasswordChangeRequest
    {
        [Required(ErrorMessage = "OldPassword")]
        [StringLength(255, ErrorMessage = "OldPassword", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "NewPassword")]
        [StringLength(255, ErrorMessage = "NewPassword", MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }

    public class CreateStaffUserResponse
    {
        public StaffUser StaffUser { get; set; }
        public string Password { get; set; }
    }

    public class StaffUser
    {
        public int Id { get; set; }
        public string AccountId { get; set; }
        [Required]
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TenantGuid { get; set; } // this is the tenant-platform mapping guid
        public int? TenantId { get; set; } // this is the actual tenant that owns the platform mapping
        public UserType UserType { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsDeleted { get; set; }
        public string ParentAccountId { get; set; }
        public ICollection<StaffUserEditPlatformRoleContainer> PlatformRoleMappings { get; set; }
        public ICollection<string> Permissions { get; set; }
        public int? BrandId { get; set; }

        public bool HasPermissionForTask(string permissionCode)
        {
            if (Permissions != null)
            {
                return (Permissions.Any(p => p == permissionCode));
            }

            return false;
        }
    }

    public class StaffUserEditPlatformRoleContainer
    {
        public string PlatformGuid { get; set; }
        public ICollection<int> Roles { get; set; }
    }

    public class BlockStaffUserRequest
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public bool Block { get; set; }
    }

    public class DeleteStaffUserRequest
    {
        [Required]
        public int UserId { get; set; }
    }

    public class AddIdleMinutesRequest
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int AddIdleMinutes { get; set; }
    }

    public class StatusLastLoginRequest
    {

        public string TenantGuid { get; set; }
        public IEnumerable<int> StaffUserIds { get; set; }
        public bool All { get; set; }
        public string TimeZoneId { get; set; }
    }
    public class StatusLastLoginWithPlatformGuid : StatusLastLoginRequest
    {
        public string PlatformGuid { get; set; }
    }
    public class PerformanceTimeRequest
    {
        public string TenantGuid { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool All { get; set; }
        public string TimeZoneId { get; set; }
    }

    public class StaffUserPerformanceTime
    {
        public int Id { get; set; }
        public double IdleTime { get; set; }
        public double ActiveTime { get; set; }
        public double LogonTime { get; set; }
    }

    public class StaffUserStatusLastLogin
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public DateTime LoginDateTime { get; set; }
        public string LoginDateTimeStr { get; set; }
        public double IdleTime { get; set; }
        public double ActiveTime { get; set; }
        public double LogonTime { get; set; }
    }

    public class BlockAllUsersRequest
    {
        public string TenantPlatformMapGuid { get; set; }
        public string PlatformGuid { get; set; }
        public UserType UserType { get; set; } 
        public List<StaffUser> StaffUsers { get; set; }
    }
}
