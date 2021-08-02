using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface ICurrencyService : IServiceBase<Currency, ICurrencyManager>
    {
        Task<WebResponse<List<CurrencyDTO>>> GetAllItemsAsync();

        Task<WebResponse<CurrencyDTO>> InsertAsync(CurrencyDTO item);

        Task<WebResponse<CurrencyDTO>> UpdateAsync(CurrencyDTO item);

        Task<WebResponse<CurrencyDTO>> GetItemById(int id);
    }
}
