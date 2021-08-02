using System;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class AutoTransferRepository : GenericRepository<AutoTransfer>, IAutoTransferRepository
    {
        public AutoTransferRepository(PardakhtPayDbContext context, IServiceProvider provider):base(context, provider)
        {

        }

        public async Task<AutoTransfer> GetLatesAutoTranfer(int cardToCardAccountId)
        {
            return await Context.AutoTransfers.Where(t => t.CardToCardAccountId == cardToCardAccountId).OrderByDescending(p => p.Id).FirstOrDefaultAsync();
        }
    }
}
