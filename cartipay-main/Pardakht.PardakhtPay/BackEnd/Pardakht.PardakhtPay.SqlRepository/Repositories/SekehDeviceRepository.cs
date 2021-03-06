using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class SekehDeviceRepository : GenericRepository<SekehDevice>, ISekehDeviceRepository
    {
        public SekehDeviceRepository(PardakhtPayDbContext context, IServiceProvider provider)
            :base(context, provider)
        {

        }
    }
}
