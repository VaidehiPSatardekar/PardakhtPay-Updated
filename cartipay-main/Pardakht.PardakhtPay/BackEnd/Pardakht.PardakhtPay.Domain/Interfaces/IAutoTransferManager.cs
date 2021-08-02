using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IAutoTransferManager: IBaseManager<AutoTransfer>
    {
        Task<AutoTransfer> Cancel(AutoTransfer item);

        Task<List<AutoTransfer>> GetUncompletedTransfers();

        Task<List<AutoTransfer>> GetPendingTransfers();

        Task<ListSearchResponse<IEnumerable<AutoTransferDTO>>> Search(AutoTransferSearchArgs args);

        Task<AutoTransfer> GetLatestAutoTranferRecord(int cardToCardAccountId);

        Task<AutoTransfer> CheckAutoTransferStatus(AutoTransfer item, bool force = false, int statementId = 0);
    }
}
