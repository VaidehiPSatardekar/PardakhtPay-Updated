using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IMerchantService : IServiceBase<Merchant, IMerchantManager>
    {
        Task<WebResponse<List<MerchantDTO>>> GetAllItemsAsync();

        Task<WebResponse<MerchantDTO>> InsertAsync(MerchantDTO item);

        Task<WebResponse<MerchantCreateDTO>> InsertWithAccountsAsync(MerchantCreateDTO item);

        Task<WebResponse<MerchantDTO>> UpdateAsync(MerchantDTO item);

        Task<WebResponse<MerchantUpdateDTO>> GetMerchantById(int id);

        Task<WebResponse<IEnumerable<Merchant>>> Search(string term);

        Task<WebResponse<MerchantUpdateDTO>> UpdateWithAccountsAsync(MerchantUpdateDTO item);
    }
}
