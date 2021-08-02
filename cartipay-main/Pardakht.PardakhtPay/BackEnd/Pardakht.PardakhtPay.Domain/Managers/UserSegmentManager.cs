using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using System.Linq;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class UserSegmentManager : BaseManager<UserSegment, IUserSegmentRepository>, IUserSegmentManager
    {
        ICachedObjectManager _CachedObjectManager = null;

        public UserSegmentManager(IUserSegmentRepository repository,
            ICachedObjectManager cachedObjectManager):base(repository)
        {
            _CachedObjectManager = cachedObjectManager;
        }

        public async Task<List<UserSegment>> GetItemsAsync(int groupId)
        {
            var items = await _CachedObjectManager.GetCachedItems<UserSegment, IUserSegmentRepository>();

            return items.Where(t => t.UserSegmentGroupId == groupId).ToList();
        }
    }
}
