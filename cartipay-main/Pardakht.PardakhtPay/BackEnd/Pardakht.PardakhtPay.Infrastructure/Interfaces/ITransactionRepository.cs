using System;
using System.Collections.Generic;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Interfaces;
using System.Threading.Tasks;

namespace Pardakht.PardakhtPay.Infrastructure.Interfaces
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        Task<List<InvoiceDetail>> GetInvoiceDetails(string ownerGuid, DateTime startDate, DateTime endDate);

        Task<decimal> GetTotalPaymentAmountForPaymentGateway(MobileTransferCardAccount account, DateTime date);
    }
}
