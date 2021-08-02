using System.Threading.Tasks;

namespace Pardakht.UserManagement.Application.JwtToken
{
    public interface IJwtTokenService
    {
        Task<string> CreateToken(Shared.Models.WebService.StaffUser model, string platformGuid);

    }
}
