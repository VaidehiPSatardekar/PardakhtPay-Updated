using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IBlockedPhoneNumberService : IServiceBase<BlockedPhoneNumber, IBlockedPhoneNumberManager>
    {
        Task<WebResponse<List<BlockedPhoneNumberDTO>>> GetAllItemsAsync();

        Task<WebResponse<BlockedPhoneNumberDTO>> GetItemById(int id);

        Task<WebResponse<BlockedPhoneNumberDTO>> InsertAsync(BlockedPhoneNumberDTO item);

        Task<WebResponse<BlockedPhoneNumberDTO>> UpdateAsync(BlockedPhoneNumberDTO item);
    }
}
