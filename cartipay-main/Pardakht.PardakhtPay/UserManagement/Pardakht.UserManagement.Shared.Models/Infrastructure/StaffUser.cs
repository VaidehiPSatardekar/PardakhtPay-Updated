using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pardakht.UserManagement.Shared.Models.Infrastructure
{
    public class StaffUser : EntityBase
    {
        [Column(TypeName = "varchar(225)")]
        public string AccountId { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string Username { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string FirstName { get; set; }
        [Column(TypeName = "nvarchar(200)")]
        public string LastName { get; set; }
        [Column(TypeName = "nvarchar(400)")]
        public string Email { get; set; }
        public UserType UserType { get; set; }
        public int? TenantId { get; set; }
        [Column(TypeName = "varchar(225)")]
        public string ParentAccountId { get; set; }
        public ICollection<UserPlatform> UserPlatforms { get; set; }
        public ICollection<UserSuspension> UserSuspensions { get; set; }
        public bool IsDeleted { get; set; }
        public int? BrandId { get; set; }
    }

    public enum UserType
    {
        StaffUser = 0,
        ApiUser = 1,
        AffiliateUser=2
    }
}
