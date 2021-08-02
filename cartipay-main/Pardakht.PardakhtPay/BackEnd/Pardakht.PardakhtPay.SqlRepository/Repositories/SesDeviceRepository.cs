using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class SesDeviceRepository : GenericRepository<SesDevice>, ISesDeviceRepository
    {
        public SesDeviceRepository(PardakhtPayDbContext context, IServiceProvider provider):base(context, provider)
        {

        }
    }
}
