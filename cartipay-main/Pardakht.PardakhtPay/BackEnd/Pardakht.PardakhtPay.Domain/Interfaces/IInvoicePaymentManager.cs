using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Invoice;

namespace Pardakht.PardakhtPay.Domain.Interfaces
{
    public interface IInvoicePaymentManager : IBaseManager<InvoicePayment>
    {
        Task<ListSearchResponse<IEnumerable<InvoicePaymentDTO>>> Search(InvoicePaymentSearchArgs args);
    }
}
