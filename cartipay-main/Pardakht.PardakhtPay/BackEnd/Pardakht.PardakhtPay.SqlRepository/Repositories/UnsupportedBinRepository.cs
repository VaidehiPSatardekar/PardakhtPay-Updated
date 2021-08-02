using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class UnsupportedBinRepository : GenericRepository<UnsupportedBin>, IUnsupportedBinRepository
    {
        public UnsupportedBinRepository(PardakhtPayDbContext context,
            IServiceProvider provider):base(context, provider)
        {

        }
    }
}
