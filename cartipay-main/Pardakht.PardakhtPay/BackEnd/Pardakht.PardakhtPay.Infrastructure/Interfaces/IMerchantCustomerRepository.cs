using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Interfaces;

namespace Pardakht.PardakhtPay.Infrastructure.Interfaces
{
    public interface IMerchantCustomerRepository : IGenericRepository<MerchantCustomer>
    {
        Task<MerchantCustomer> GetCustomer(string ownerGuid, string webSiteName, string userId);
    }
}
