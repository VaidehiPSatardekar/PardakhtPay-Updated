using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Pardakht.UserManagement.Infrastructure.Interfaces
{
    public interface ISignInManager
    {
        bool RequireConfirmedEmail { get; set; }
        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);
        Task SignOutAsync();
    }
}
