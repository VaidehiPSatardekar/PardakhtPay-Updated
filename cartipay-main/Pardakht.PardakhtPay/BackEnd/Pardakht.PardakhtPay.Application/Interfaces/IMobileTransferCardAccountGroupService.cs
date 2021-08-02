using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.MobileTransfer;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IMobileTransferCardAccountGroupService: IServiceBase<MobileTransferCardAccountGroup, IMobileTransferCardAccountGroupManager>
    {
        Task<WebResponse<List<MobileTransferCardAccountGroupDTO>>> GetAllItemsAsync();

        Task<WebResponse<MobileTransferCardAccountGroupDTO>> GetItemById(int id);

        Task<WebResponse<MobileTransferCardAccountGroupDTO>> InsertAsync(MobileTransferCardAccountGroupDTO item);

        Task<WebResponse<MobileTransferCardAccountGroupDTO>> UpdateAsync(MobileTransferCardAccountGroupDTO item);

        Task ClearCache();
    }
}
