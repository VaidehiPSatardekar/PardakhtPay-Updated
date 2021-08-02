using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class UserSegmentRepository : GenericRepository<UserSegment>, IUserSegmentRepository
    {
        ICachedObjectManager _CachedObjectManager = null;

        public UserSegmentRepository(PardakhtPayDbContext context,
            IServiceProvider provider,
            ICachedObjectManager cachedObjectManager)
            :base(context, provider)
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
