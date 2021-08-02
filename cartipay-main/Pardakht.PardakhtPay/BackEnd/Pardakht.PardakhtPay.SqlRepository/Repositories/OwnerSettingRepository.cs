using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class OwnerSettingRepository : GenericRepository<OwnerSetting>, IOwnerSettingRepository
    {
        public OwnerSettingRepository(PardakhtPayDbContext context,
            IServiceProvider provider)
            :base(context, provider)
        {

        }
    }
}
