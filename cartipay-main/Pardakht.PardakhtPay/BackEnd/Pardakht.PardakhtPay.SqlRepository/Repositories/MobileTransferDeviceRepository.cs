using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using System.Linq;
using Pardakht.PardakhtPay.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class MobileTransferDeviceRepository : GenericRepository<MobileTransferDevice>, IMobileTransferDeviceRepository
    {
        ICachedObjectManager _CachedObjectManager = null;

        public MobileTransferDeviceRepository(PardakhtPayDbContext context,
            IServiceProvider provider,
            ICachedObjectManager cachedObjectManager) :base(context, provider)
        {
            _CachedObjectManager = cachedObjectManager;
        }

        protected override void OnAfterSave()
        {
            _CachedObjectManager.ClearCachedItems<MobileTransferDevice>();
        }

        public override async Task<List<MobileTransferDevice>> GetCacheItems()
        {
            var date = DateTime.UtcNow.AddHours(Helper.MobileDeviceBlockPeriod);

            var items = await GetQuery(t => t.IsActive && t.Status == (int)MobileTransferDeviceStatus.PhoneNumberVerified && (!t.LastBlockDate.HasValue || t.LastBlockDate.Value < date))
                .OrderBy(t => t.LastBlockDate)
                .ThenByDescending(t => t.VerifiedDate)
                .Take(10)
                .AsNoTracking().ToListAsync();

            return items.OrderBy(p => p.GetHashCode()).ToList();
        }
    }
}
