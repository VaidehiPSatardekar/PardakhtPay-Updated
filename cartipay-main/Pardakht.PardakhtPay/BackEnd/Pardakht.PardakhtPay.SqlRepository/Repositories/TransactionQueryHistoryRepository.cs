using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class TransactionQueryHistoryRepository : GenericRepository<TransactionQueryHistory>, ITransactionQueryHistoryRepository
    {
        public TransactionQueryHistoryRepository(PardakhtPayDbContext context,
            IServiceProvider provider) :base(context, provider)
        {

        }
    }
}
