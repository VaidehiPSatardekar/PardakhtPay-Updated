using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IAutoTransferService : IServiceBase<AutoTransfer, IAutoTransferManager>
    {
        Task<WebResponse<ListSearchResponse<IEnumerable<AutoTransferDTO>>>> Search(AutoTransferSearchArgs args);

        Task CheckWithTransferRequestId(int transferRequestId, int statementId);
    }
}
