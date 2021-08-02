using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class SadadPspDeviceRepository : GenericRepository<SadadPspDevice>, ISadadPspDeviceRepository
    {
        public SadadPspDeviceRepository(PardakhtPayDbContext context,
            IServiceProvider provider)
            :base(context, provider)
        {

        }
    }
}
