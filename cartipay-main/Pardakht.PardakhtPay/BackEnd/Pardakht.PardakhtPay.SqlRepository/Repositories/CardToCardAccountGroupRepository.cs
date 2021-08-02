using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class CardToCardAccountGroupRepository : GenericRepository<CardToCardAccountGroup>, ICardToCardAccountGroupRepository
    {
        ICachedObjectManager _CachedObjectManager = null;

        public CardToCardAccountGroupRepository(PardakhtPayDbContext context,
            ICachedObjectManager cachedObjectManager,
            IServiceProvider provider):base(context, provider)
        {
            _CachedObjectManager = cachedObjectManager;
        }

        protected override void OnAfterSave()
        {
            _CachedObjectManager.ClearCachedItems<CardToCardAccountGroup>();
            _CachedObjectManager.ClearCachedItems<CardToCardAccountGroupItem>();
            _CachedObjectManager.ClearCachedItems<CardToCardUserSegmentRelation>();
        }
    }
}
