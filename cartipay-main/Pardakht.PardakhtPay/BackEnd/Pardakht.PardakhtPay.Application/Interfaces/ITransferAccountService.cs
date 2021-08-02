using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface ITransferAccountService : IServiceBase<TransferAccount, ITransferAccountManager>
    {
        Task<WebResponse<List<TransferAccountDTO>>> GetAllItemsAsync();

        Task<WebResponse<TransferAccountDTO>> InsertAsync(TransferAccountDTO item);

        Task<WebResponse<TransferAccountDTO>> UpdateAsync(TransferAccountDTO item);

        Task<WebResponse<TransferAccountDTO>> GetItemById(int id);
    }
}
