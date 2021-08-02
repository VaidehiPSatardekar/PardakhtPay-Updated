using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IWithdrawalTransferHistoryManager : IBaseManager<WithdrawalTransferHistory>
    {
        Task<List<InvoiceDetail>> GetInvoiceDetails(string ownerGuid, DateTime startDate, DateTime endDate);
    }
}
