using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface ITransactionWithdrawalRelationManager : IBaseManager<TransactionWithdrawalRelation>
    {
        Task<Withdrawal> GetWithdrawal(Transaction transaction, Merchant merchant);

        Task DeleteRelation(Transaction transaction);

        Task CompleteWithdrawal(Transaction transaction);

        Task<List<long>> GetSuggestedWithdrawalAmounts(long amount, Merchant merchant);

        Task<List<long>> GetSuggestedWithdrawalAmountsWithZeroAmount(Merchant merchant);
    }
}
