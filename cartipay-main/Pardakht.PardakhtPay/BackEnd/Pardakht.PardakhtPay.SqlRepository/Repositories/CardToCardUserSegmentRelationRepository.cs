using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class CardToCardUserSegmentRelationRepository : GenericRepository<CardToCardUserSegmentRelation>, ICardToCardUserSegmentRelationRepository
    {
        ICachedObjectManager _CachedObjectManager = null;

        public CardToCardUserSegmentRelationRepository(PardakhtPayDbContext context,
            ICachedObjectManager cachedObjectManager,
            IServiceProvider provider)
            :base(context, provider)
        {
            _CachedObjectManager = cachedObjectManager;
        }

        protected override void OnAfterSave()
        {
            _CachedObjectManager.ClearCachedItems<CardToCardUserSegmentRelation>();
        }
    }
}
