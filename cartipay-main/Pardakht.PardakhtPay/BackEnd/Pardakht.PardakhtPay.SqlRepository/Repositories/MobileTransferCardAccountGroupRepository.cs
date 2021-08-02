using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class MobileTransferCardAccountGroupRepository : GenericRepository<MobileTransferCardAccountGroup>, IMobileTransferCardAccountGroupRepository
    {
        ICachedObjectManager _CachedObjectManager = null;

        public MobileTransferCardAccountGroupRepository(PardakhtPayDbContext context,
            IServiceProvider provider,
            ICachedObjectManager cachedObjectManager) : base(context, provider)
        {
            _CachedObjectManager = cachedObjectManager;
        }

        protected override void OnAfterSave()
        {
            _CachedObjectManager.ClearCachedItems<MobileTransferCardAccountGroup>();
            _CachedObjectManager.ClearCachedItems<MobileTransferCardAccountGroupItem>();
            _CachedObjectManager.ClearCachedItems<MobileCardAccountUserSegmentRelation>();
        }
    }
}
