using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class UserSegmentGroupRepository : GenericRepository<UserSegmentGroup>, IUserSegmentGroupRepository
    {
        ICachedObjectManager _CachedObjectManager = null;

        public UserSegmentGroupRepository(PardakhtPayDbContext context,
            IServiceProvider provider,
            ICachedObjectManager cachedObjectManager) :base(context, provider)
        {
            _CachedObjectManager = cachedObjectManager;
        }

        protected override void OnAfterSave()
        {
            _CachedObjectManager.ClearCachedItems<UserSegment>();
            _CachedObjectManager.ClearCachedItems<UserSegmentGroup>();
        }
    }
}
