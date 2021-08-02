using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class TransactionWithdrawalHistoryRepository : GenericRepository<TransactionWithdrawalHistory>, ITransactionWithdrawalHistoryRepository
    {
        public TransactionWithdrawalHistoryRepository(PardakhtPayDbContext context,
            IServiceProvider provider):base(context, provider)
        {

        }
    }
}
