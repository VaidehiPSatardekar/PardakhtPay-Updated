using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Invoice;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IInvoicePaymentService : IServiceBase<InvoicePayment, IInvoicePaymentManager>
    {
        Task<WebResponse<ListSearchResponse<IEnumerable<InvoicePaymentDTO>>>> Search(InvoicePaymentSearchArgs args);

        Task<WebResponse<InvoicePaymentDTO>> GetItemById(int id);

        Task<WebResponse<InvoicePaymentDTO>> InsertAsync(InvoicePaymentDTO item);

        Task<WebResponse<InvoicePaymentDTO>> UpdateAsync(InvoicePaymentDTO item);
    }
}
