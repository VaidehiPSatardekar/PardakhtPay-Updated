using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class TransactionWithdrawalHistoryManager : BaseManager<TransactionWithdrawalHistory, ITransactionWithdrawalHistoryRepository>, ITransactionWithdrawalHistoryManager
    {
        public TransactionWithdrawalHistoryManager(ITransactionWithdrawalHistoryRepository repository):base(repository)
        {

        }
    }
}
