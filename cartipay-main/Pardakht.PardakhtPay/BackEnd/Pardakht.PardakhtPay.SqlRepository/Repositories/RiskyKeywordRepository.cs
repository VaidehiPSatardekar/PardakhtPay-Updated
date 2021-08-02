using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class RiskyKeywordRepository : GenericRepository<RiskyKeyword>, IRiskyKeywordRepository
    {
        public RiskyKeywordRepository(PardakhtPayDbContext context, IServiceProvider provider):base(context, provider)
        {

        }
    }
}
