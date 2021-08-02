using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class BankStatementItemRepository : GenericRepository<BankStatementItem>, IBankStatementItemRepository
    {
        public BankStatementItemRepository(PardakhtPayDbContext context, IServiceProvider provider):base(context, provider)
        {

        }

        public BankStatementItem GetItemByRecordId(int recordId)
        {
            var item = GetQuery().Where(t => t.RecordId == recordId).FirstOrDefault();
            return item;
        }

        public async Task<decimal> GetTotalCreditAmount(string accountGuid, DateTime startDate, DateTime endDate)
        {
            return await GetQuery().Where(t => t.AccountGuid == accountGuid).Where(t => t.TransactionDateTime >= startDate && t.TransactionDateTime <= endDate).SumAsync(t => t.Credit);
        }
    }
}
