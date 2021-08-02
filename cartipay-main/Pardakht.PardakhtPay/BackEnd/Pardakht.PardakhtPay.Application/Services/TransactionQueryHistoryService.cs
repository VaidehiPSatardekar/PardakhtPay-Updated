using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class TransactionQueryHistoryService : DatabaseServiceBase<TransactionQueryHistory, ITransactionQueryHistoryManager>, ITransactionQueryHistoryService
    {
        public TransactionQueryHistoryService(ITransactionQueryHistoryManager manager, ILogger<TransactionQueryHistoryService> logger):base(manager, logger)
        {

        }
    }
}
