using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class ManualTransferSourceCardDetailsRepository : GenericRepository<ManualTransferSourceCardDetails>, IManualTransferSourceCardDetailsRepository
    {
        public ManualTransferSourceCardDetailsRepository(PardakhtPayDbContext context, IServiceProvider provider):base(context, provider)
        {

        }
    }
}
