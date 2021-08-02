using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class Payment780DeviceManager : BaseManager<Payment780Device, IPayment780DeviceRepository>, IPayment780DeviceManager
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
        public Payment780DeviceManager(IPayment780DeviceRepository repository) : base(repository)
        {

        }

        public async Task<Payment780Device> GetRandomDeviceAsync()
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

        public async Task DeActivateDevices(string phoneNumber)
        {
            var Payment780Devices = await GetItemsAsync(t => t.IsRegistered && t.PhoneNumber == phoneNumber);

            for (int i = 0; i < Payment780Devices.Count; i++)
            {
                var d = Payment780Devices[i];
                d.IsRegistered = false;
                d.TryCount = 0;

                await UpdateAsync(d);
            }

            await SaveAsync();
        }

        public async Task RemoveDeviceCustomerRelation(string phoneNumber, int merchantCustomerId)
        {
            var Payment780Devices = await GetItemsAsync(t => t.IsRegistered && t.PhoneNumber == phoneNumber && t.MerchantCustomerId == merchantCustomerId);

            for (int i = 0; i < Payment780Devices.Count; i++)
            {
                var d = Payment780Devices[i];
                d.MerchantCustomerId = 0;

                await UpdateAsync(d);
            }

            await SaveAsync();
        }
    }
}
