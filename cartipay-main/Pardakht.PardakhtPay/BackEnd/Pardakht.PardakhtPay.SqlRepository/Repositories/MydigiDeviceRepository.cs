using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class MydigiDeviceRepository: GenericRepository<MydigiDevice>, IMydigiDeviceRepository
    {
        public MydigiDeviceRepository(PardakhtPayDbContext context, IServiceProvider provider)
            :base(context, provider)
        {

        }
    }
}
