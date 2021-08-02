using System.Collections.Generic;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string AccountId { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool IsApiKeyUser { get; set; }

        public List<string> RoleGuid { get; set; }

        public string TenantDomainPlatformMapGuid { get; set; }
    }

    public class UserCreateDTO
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool isApiKey { get; set; }

        public string[] RoleGuid { get; set; }

        public string TenantDomainPlatformMapGuid { get; set; }
    }

    public class UserUpdateDTO
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool isApiKey { get; set; }

        public string[] RoleGuid { get; set; }

        public string TenantDomainPlatformMapGuid { get; set; }
    }
}
