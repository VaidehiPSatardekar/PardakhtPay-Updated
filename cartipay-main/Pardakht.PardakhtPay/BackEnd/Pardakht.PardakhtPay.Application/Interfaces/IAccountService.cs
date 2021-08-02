using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IAccountService
    {
        Task<WebResponse<List<UserDTO>>> GetUsers(string authorizationToken, string origin);
    }
}
