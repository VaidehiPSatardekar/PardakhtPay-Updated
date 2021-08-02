using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.MobileTransfer;

namespace Pardakht.PardakhtPay.Shared.Interfaces
{
    public interface IMobileTransferService
    {
        Task<MobileTransferResponse> SendSMSAsync(MobileTransferSendSmsModel model);

        Task<MobileTransferResponse> ActivateDeviceAsync(MobileTransferActivateDeviceModel model);

        Task<MobileTransferResponse> CheckDeviceStatusAsync(MobileTransferSendSmsModel model);

        Task<MobileTransferResponse> CheckStatusAsync(MobileTransferCheckStatusModel model);

        Task<MobileTransferResponse> RemoveDeviceAsync(MobileTransferRemoveDeviceModel model);

        Task<MobileTransferResponse> StartTransferAsync(MobileTransferStartTransferModel model);

        Task<MobileTransferResponse> SendOtpPinAsync(MobileTransferStartTransferModel model);

        Task<MobileTransferResponse> CheckTransferStatusAsync(MobileTransferStartTransferModel model);

        Task<MobileTransferResponse> GetCardOwnerNameAsync(MobileTransferStartTransferModel model);
    }
}
