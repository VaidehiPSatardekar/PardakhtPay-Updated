using System;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class MerchantCustomerRepository : GenericRepository<MerchantCustomer>, IMerchantCustomerRepository
    {
        public MerchantCustomerRepository(PardakhtPayDbContext context,
            IServiceProvider provider):base(context, provider)
        {

        }

        public async Task<MerchantCustomer> GetCustomer(string ownerGuid, string webSiteName, string userId)
        {
            return await GetQuery(t => t.OwnerGuid == ownerGuid && t.WebsiteName == webSiteName && t.UserId == userId).AsNoTracking().FirstOrDefaultAsync();
        }
    }
}
