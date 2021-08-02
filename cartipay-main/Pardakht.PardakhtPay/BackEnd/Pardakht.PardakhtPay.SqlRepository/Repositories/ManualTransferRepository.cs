using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class ManualTransferRepository : GenericRepository<ManualTransfer>, IManualTransferRepository
    {
        public ManualTransferRepository(PardakhtPayDbContext context, IServiceProvider provider):base(context, provider)
        {

        }
    }
}
