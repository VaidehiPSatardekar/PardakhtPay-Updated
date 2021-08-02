using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Invoice;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IInvoiceService : IServiceBase<Invoice, IInvoiceManager>
    {
        Task<WebResponse<ListSearchResponse<IEnumerable<InvoiceSearchDTO>>>> Search(InvoiceSearchArgs args);

        Task<WebResponse<InvoiceDTO>> GetItemById(int id);
    }
}
