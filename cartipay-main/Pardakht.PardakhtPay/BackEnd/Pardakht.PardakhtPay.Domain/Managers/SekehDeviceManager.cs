using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class SekehDeviceManager : BaseManager<SekehDevice, ISekehDeviceRepository>, ISekehDeviceManager
    {
        readonly List<int> SkipNumbers = new List<int>()
        {
            50000,
            30000,
            20000,
            10000,
            1000,
            10
        };
        public SekehDeviceManager(ISekehDeviceRepository repository) : base(repository)
        {

        }

        public async Task<SekehDevice> GetRandomDeviceAsync()
        {
            for (int i = 0; i < SkipNumbers.Count; i++)
            {
                var skip = new Random().Next(0, SkipNumbers[i]);

                var query = GetQuery();

                query = query.Where(t => t.IsRegistered);

                var item = await query.Skip(skip).FirstOrDefaultAsync();

                if (item != null)
                {
                    return item;
                }

            }

            return null;

        }

        public async Task DeActivateSekehDevices(string phoneNumber)
        {
            var sekehDevices = await GetItemsAsync(t => t.IsRegistered && t.PhoneNumber == phoneNumber);

            for (int i = 0; i < sekehDevices.Count; i++)
            {
                var d = sekehDevices[i];
                d.IsRegistered = false;
                d.TryCount = 0;

                await UpdateAsync(d);
            }

            await SaveAsync();
        }

        public async Task RemoveSekehDeviceCustomerRelation(string phoneNumber, int merchantCustomerId)
        {
            var sekehDevices = await GetItemsAsync(t => t.IsRegistered && t.PhoneNumber == phoneNumber && t.MerchantCustomerId == merchantCustomerId);

            for (int i = 0; i < sekehDevices.Count; i++)
            {
                var d = sekehDevices[i];
                d.MerchantCustomerId = 0;

                await UpdateAsync(d);
            }

            await SaveAsync();
        }
    }
}
