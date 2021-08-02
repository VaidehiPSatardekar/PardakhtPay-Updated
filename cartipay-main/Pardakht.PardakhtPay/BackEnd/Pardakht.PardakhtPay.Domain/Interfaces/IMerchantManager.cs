using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IMerchantManager : IBaseManager<Merchant>
    {
        Task<IEnumerable<Merchant>> Search(string term);

        Task<Merchant> GetMerchantByApiKey(string key);

        Task<Merchant> GetMerchantByClearApiKey(string key);
    }
}
