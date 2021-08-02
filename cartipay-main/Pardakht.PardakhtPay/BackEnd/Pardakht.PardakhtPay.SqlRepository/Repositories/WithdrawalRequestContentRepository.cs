using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class WithdrawalRequestContentRepository : GenericRepository<WithdrawalRequestContent>, IWithdrawalRequestContentRepository
    {
        public WithdrawalRequestContentRepository(PardakhtPayDbContext context,
            IServiceProvider provider):base(context, provider)
        {

        }
    }
}
