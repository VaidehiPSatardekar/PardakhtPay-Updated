using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class TransactionQueryHistoryManager : BaseManager<TransactionQueryHistory, ITransactionQueryHistoryRepository>, ITransactionQueryHistoryManager
    {
        public TransactionQueryHistoryManager(ITransactionQueryHistoryRepository repository):base(repository)
        {

        }
    }
}
