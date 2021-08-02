using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class BlockedPhoneNumberRepository : GenericRepository<BlockedPhoneNumber>, IBlockedPhoneNumberRepository
    {
        ICachedObjectManager _CachedObjectManager = null;

        public BlockedPhoneNumberRepository(PardakhtPayDbContext context,
            IServiceProvider provider,
            ICachedObjectManager cachedObjectManager) :base(context, provider)
        {
            _CachedObjectManager = cachedObjectManager;
        }

        protected override void OnAfterSave()
        {
            _CachedObjectManager.ClearCachedItems<BlockedPhoneNumber>();
        }
    }
}
