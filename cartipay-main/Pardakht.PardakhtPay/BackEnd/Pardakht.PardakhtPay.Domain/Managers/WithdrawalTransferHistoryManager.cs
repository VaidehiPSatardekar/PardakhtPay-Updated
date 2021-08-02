using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class WithdrawalTransferHistoryManager : BaseManager<WithdrawalTransferHistory, IWithdrawalTransferHistoryRepository>, IWithdrawalTransferHistoryManager
    {
        public WithdrawalTransferHistoryManager(IWithdrawalTransferHistoryRepository repository):base(repository)
        {

        }

        public async Task<List<InvoiceDetail>> GetInvoiceDetails(string ownerGuid, DateTime startDate, DateTime endDate)
        {
            return await Repository.GetInvoiceDetails(ownerGuid, startDate, endDate);
        }
    }
}
