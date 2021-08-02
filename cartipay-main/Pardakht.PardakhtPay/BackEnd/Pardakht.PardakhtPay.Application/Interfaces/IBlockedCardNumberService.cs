using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IBlockedCardNumberService : IServiceBase<BlockedCardNumber, IBlockedCardNumberManager>
    {
        Task<WebResponse<List<BlockedCardNumberDTO>>> GetAllItemsAsync();

        Task<WebResponse<BlockedCardNumberDTO>> GetItemById(int id);

        Task<WebResponse<BlockedCardNumberDTO>> InsertAsync(BlockedCardNumberDTO item);

        Task<WebResponse<BlockedCardNumberDTO>> UpdateAsync(BlockedCardNumberDTO item);
    }
}
