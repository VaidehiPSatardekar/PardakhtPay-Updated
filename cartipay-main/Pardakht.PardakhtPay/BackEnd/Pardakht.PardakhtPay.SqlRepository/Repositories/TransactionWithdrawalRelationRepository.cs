using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class TransactionWithdrawalRelationRepository : GenericRepository<TransactionWithdrawalRelation>, ITransactionWithdrawalRelationRepository
    {
        public TransactionWithdrawalRelationRepository(PardakhtPayDbContext context,
            IServiceProvider provider):base(context, provider)
        {

        }
    }
}
