using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class AutoTransferService : DatabaseServiceBase<AutoTransfer, IAutoTransferManager>, IAutoTransferService
    {
        public AutoTransferService(IAutoTransferManager manager, ILogger<AutoTransferService> logger) : base(manager, logger)
        {

        }

        public async Task<WebResponse<ListSearchResponse<IEnumerable<AutoTransferDTO>>>> Search(AutoTransferSearchArgs args)
        {
            try
            {
                var response = await Manager.Search(args);

                return new WebResponse<ListSearchResponse<IEnumerable<AutoTransferDTO>>>(response);
            }
            catch (Exception ex)
            {
                return new WebResponse<ListSearchResponse<IEnumerable<AutoTransferDTO>>>()
                {
                    Exception = ex,
                    Payload = null,
                    Success = false
                };
            }
        }

        public async Task CheckWithTransferRequestId(int transferRequestId, int statementId)
        {
            try
            {
                AutoTransfer autoTransfer = await Manager.GetItemAsync(t => t.RequestId == transferRequestId);

                if (autoTransfer == null)
                {
                    return;
                }

                var oldStatus = autoTransfer.Status;

                await Manager.CheckAutoTransferStatus(autoTransfer, true, statementId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
            }
        }

    }
}
