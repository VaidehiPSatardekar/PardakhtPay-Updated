using System;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Interfaces;
using System.Threading.Tasks;

namespace Pardakht.PardakhtPay.Infrastructure.Interfaces
{
    public interface IBankStatementItemRepository : IGenericRepository<BankStatementItem>
    {
        BankStatementItem GetItemByRecordId(int recordId);

        Task<decimal> GetTotalCreditAmount(string accountGuid, DateTime startDate, DateTime endDate);
    }
}
