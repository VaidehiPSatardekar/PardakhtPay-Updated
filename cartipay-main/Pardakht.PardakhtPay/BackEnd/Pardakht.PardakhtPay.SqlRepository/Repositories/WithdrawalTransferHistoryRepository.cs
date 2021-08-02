using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class WithdrawalTransferHistoryRepository : GenericRepository<WithdrawalTransferHistory>, IWithdrawalTransferHistoryRepository
    {
        public WithdrawalTransferHistoryRepository(PardakhtPayDbContext context,
            IServiceProvider provider):base(context, provider)
        {

        }

        public async Task<List<InvoiceDetail>> GetInvoiceDetails(string ownerGuid, DateTime startDate, DateTime endDate)
        {
            var repository = ServiceProvider.GetRequiredService<IWithdrawalRepository>();

            var completed = (int)TransferStatus.Complete;

            var query = GetQuery(t => t.LastCheckDate >= startDate && t.LastCheckDate < endDate && t.TransferStatus == completed);

            var withdrawalQuery = repository.GetQuery(t => t.OwnerGuid == ownerGuid);

            var details = (from q in query
                           join w in withdrawalQuery on q.WithdrawalId equals w.Id
                           group q by new { MerchantId = w.MerchantId } into gr
                           select new InvoiceDetail()
                           {
                               MerchantId = gr.Key.MerchantId,
                               TotalAmount = gr.Sum(t => t.Amount),
                               EndDate = endDate,
                               ItemTypeId = (int)InvoiceDetailItemType.Withdrawal,
                               StartDate = startDate,
                               TotalCount = gr.Count()
                           });

            return await details.AsNoTracking().ToListAsync();
        }
    }
}
