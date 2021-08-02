using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface ITransferRequestService
    {
        Task<WebResponse> UpdateTransactionStatus(TransferRequestResponse response);

    }
}
