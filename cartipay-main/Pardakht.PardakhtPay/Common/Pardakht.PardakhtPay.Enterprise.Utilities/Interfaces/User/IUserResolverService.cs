using System.Threading.Tasks;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.User;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.User
{
    public interface IUserResolverService
    {
        Task<StaffUser> GetUser(string accountId, string tenantGuid, string platformGuid);
    }
}
