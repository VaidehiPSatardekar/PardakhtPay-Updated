using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class MerchantRepository : GenericRepository<Merchant>, IMerchantRepository
    {
        ICachedObjectManager _CachedObjectManager = null;

        public MerchantRepository(PardakhtPayDbContext context,
            ICachedObjectManager cachedObjectManager,
            IServiceProvider provider) :base(context, provider)
        {
            _CachedObjectManager = cachedObjectManager;
        }

        protected override void OnAfterSave()
        {
            _CachedObjectManager.ClearCachedItems<Merchant>();
        }
    }
}
