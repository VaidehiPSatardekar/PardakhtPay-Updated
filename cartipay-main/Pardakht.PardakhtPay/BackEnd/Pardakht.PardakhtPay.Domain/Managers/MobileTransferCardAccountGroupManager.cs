using System;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using System.Linq;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class MobileTransferCardAccountGroupManager : BaseManager<MobileTransferCardAccountGroup, IMobileTransferCardAccountGroupRepository>, IMobileTransferCardAccountGroupManager
    {
        ICachedObjectManager _CachedObjectManager = null;
        IMobileCardAccountUserSegmentRelationRepository _UserSegmentRelationRepository = null;

        public MobileTransferCardAccountGroupManager(IMobileTransferCardAccountGroupRepository repository,
            ICachedObjectManager cachedObjectManager,
            IMobileCardAccountUserSegmentRelationRepository userSegmentRelationRepository) : base(repository)
        {
            _CachedObjectManager = cachedObjectManager;
            _UserSegmentRelationRepository = userSegmentRelationRepository;
        }

        public async Task<MobileTransferCardAccountGroupItem> GetRandomActiveAccount(Merchant merchant)
        {
            MobileTransferCardAccountGroupItem item = null;

            if (!merchant.MobileTransferAccountGroupId.HasValue)
            {
                return null;
            }

            var items = await _CachedObjectManager.GetCachedItems<MobileTransferCardAccountGroupItem, IMobileTransferCardAccountGroupItemRepository>();

            var activeItems = items.Where(t => t.GroupId == merchant.MobileTransferAccountGroupId && t.ItemStatus == MobileTransferCardAccountGroupItemStatus.Active).ToList();

            if (activeItems.Count == 0)
            {
                return null;
            }

            var accounts = await _CachedObjectManager.GetCachedItems<MobileTransferCardAccount, IMobileTransferCardAccountRepository>();

            if (activeItems.Count == 1)
            {
                item = activeItems[0];
            }

            var random = new Random();
            var next = random.Next(0, activeItems.Count);

            item = activeItems[next];

            item.Account = accounts.FirstOrDefault(t => t.Id == item.ItemId);

            return item;
        }
    }
}
