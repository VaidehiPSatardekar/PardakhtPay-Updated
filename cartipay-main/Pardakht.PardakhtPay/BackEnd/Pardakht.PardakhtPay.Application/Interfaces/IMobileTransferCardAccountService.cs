using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.MobileTransfer;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IMobileTransferCardAccountService: IServiceBase<MobileTransferCardAccount, IMobileTransferCardAccountManager>
    {
        Task<WebResponse<List<MobileTransferCardAccountDTO>>> GetAllItemsAsync();

        Task<WebResponse<MobileTransferCardAccountDTO>> GetItemById(int id);

        Task<WebResponse<MobileTransferCardAccountDTO>> InsertAsync(MobileTransferCardAccountDTO item);

        Task<WebResponse<MobileTransferCardAccountDTO>> UpdateAsync(MobileTransferCardAccountDTO item);
    }
}
