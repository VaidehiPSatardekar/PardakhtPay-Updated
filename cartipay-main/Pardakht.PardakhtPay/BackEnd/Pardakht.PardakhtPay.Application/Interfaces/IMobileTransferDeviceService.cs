using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.MobileTransfer;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IMobileTransferDeviceService: IServiceBase<MobileTransferDevice, IMobileTransferDeviceManager>
    {
        Task<WebResponse<List<MobileTransferDeviceDTO>>> GetAllItemsAsync();

        Task<WebResponse<MobileTransferDeviceDTO>> GetItemById(int id);

        Task<WebResponse<MobileTransferDeviceDTO>> InsertAsync(MobileTransferDeviceDTO item);

        Task<WebResponse<MobileTransferDeviceDTO>> UpdateAsync(MobileTransferDeviceDTO item);

        Task<WebResponse<MobileTransferDeviceDTO>> SendSmsAsync(int id);

        Task<WebResponse<MobileTransferDeviceDTO>> ActivateAsync(int id, string verificationCode);

        Task<WebResponse<MobileTransferDeviceDTO>> CheckStatusAsync(int id);

        Task<WebResponse<MobileTransferDeviceDTO>> RemoveAsync(int id);
    }
}
