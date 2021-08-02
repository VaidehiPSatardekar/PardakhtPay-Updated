using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class TransferAccountRepository : GenericRepository<TransferAccount>, ITransferAccountRepository
    {
        public TransferAccountRepository(PardakhtPayDbContext context,
            IServiceProvider provider) :base(context, provider)
        {

        }
    }
}
