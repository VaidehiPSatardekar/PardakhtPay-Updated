using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class Payment780DeviceRepository : GenericRepository<Payment780Device>, IPayment780DeviceRepository
    {
        public Payment780DeviceRepository(PardakhtPayDbContext context,
            IServiceProvider provider)
            :base(context, provider)
        {

        }
    }
}
