namespace Pardakht.UserManagement.Shared.Models.Infrastructure
{
    public class RefreshToken
    {
        public ApplicationUser ApplicationUser { get; set; }
        public string Token { get; set; }
        public bool Revoked { get; set; }
    }
}
