using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class TransactionRequestContentRepository : GenericRepository<TransactionRequestContent>, ITransactionRequestContentRepository
    {
        public TransactionRequestContentRepository(PardakhtPayDbContext content,
            IServiceProvider provider):base(content, provider)
        {

        }
    }
}
