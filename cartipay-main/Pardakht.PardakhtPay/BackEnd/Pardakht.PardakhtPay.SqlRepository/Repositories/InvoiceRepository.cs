using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(PardakhtPayDbContext context,
            IServiceProvider provider):base(context, provider)
        {

        }

        public async Task<Invoice> GetLastInvoiceAsync(string ownerGuid)
        {
            return await GetQuery(t => t.OwnerGuid == ownerGuid).OrderByDescending(t => t.EndDate).AsNoTracking().FirstOrDefaultAsync();
        }
    }
}
