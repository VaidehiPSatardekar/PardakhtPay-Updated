using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Pardakht.UserManagement.Infrastructure.Interfaces;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Infrastructure.Identity
{
    public class SignInManagerWrapper : ISignInManager
    {
        readonly SignInManager<ApplicationUser> _signInManager;

        public bool RequireConfirmedEmail
        {
            get { return _signInManager.Options.SignIn.RequireConfirmedEmail; }
            set { _signInManager.Options.SignIn.RequireConfirmedEmail = value; }
        }

        public SignInManagerWrapper(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            return _signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
        }

        public Task SignOutAsync()
        {
            return _signInManager.SignOutAsync();
        }
    }
}
