using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.BankBot;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IManualTransferService : IServiceBase<ManualTransfer, IManualTransferManager>
    {
        Task<WebResponse<ManualTransferDTO>> GetItemById(int id);

        Task<WebResponse<ListSearchResponse<List<ManualTransferDTO>>>> Search(ManualTransferSearchArgs args);

        Task<WebResponse<ManualTransferDTO>> InsertAsync(ManualTransferDTO item);

        Task<WebResponse<ManualTransferDTO>> UpdateAsync(ManualTransferDTO item);

        Task<WebResponse<ManualTransferDTO>> CancelAsync(int id);

        Task<WebResponse<List<ManualTransferDetailDTO>>> GetDetails(int id);

        Task<WebResponse<BankBotTransferReceiptResponse>> GetTransferReceipt(int id);

        Task<WebResponse<ManualTransferDetailDTO>> CancelTransferDetail(int detailId);

        Task<WebResponse<ManualTransferDetailDTO>> RetryTransferDetail(int detailId);

        Task<WebResponse<ManualTransferDetailDTO>> SetAsCompletedTransferDetail(int detailId);
    }
}
