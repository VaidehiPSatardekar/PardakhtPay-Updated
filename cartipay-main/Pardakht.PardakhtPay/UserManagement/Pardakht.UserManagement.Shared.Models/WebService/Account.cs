using System.ComponentModel.DataAnnotations;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Shared.Models.WebService
{
    public class ApiKeyLoginRequest
    {
        [Required(ErrorMessage = "ApiKey is missing")]
        public string ApiKey { get; set; }
        [Required(ErrorMessage = "PlatformGuid is missing")]
        public string PlatformGuid { get; set; }
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "Username is missing")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is missing")]
        public string Password { get; set; }

        public string LoginAsUsername { get; set; }

        public int? TenantId { get; set; }
        public int? BrandId { get; set; }
    }

    public class LoginAsRequest
    {
        [Required(ErrorMessage = "Username is missing")]
        public string UserName { get; set; }
    }

    public class LoginResponse
    {
        public StaffUser User { get; set; }
        public JsonWebToken Token { get; set; }
    }

    public class UserRegister
    {
        [Required]
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TenantGuid { get; set; }
        public string PlatformGuid { get; set; }
        public int[] RoleIds { get; set; }
    }
}
