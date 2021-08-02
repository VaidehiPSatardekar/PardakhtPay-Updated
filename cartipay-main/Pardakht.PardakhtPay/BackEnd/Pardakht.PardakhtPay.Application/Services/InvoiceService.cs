using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Invoice;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class InvoiceService : DatabaseServiceBase<Invoice, IInvoiceManager>, IInvoiceService
    {
        public InvoiceService(IInvoiceManager manager, ILogger<InvoiceService> logger)
            :base(manager, logger)
        {

        }

        public async Task<WebResponse<InvoiceDTO>> GetItemById(int id)
        {
            try
            {
                var item = await Manager.GetItemById(id);

                return new WebResponse<InvoiceDTO>(item);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<InvoiceDTO>(ex);
            }
        }

        public async Task<WebResponse<ListSearchResponse<IEnumerable<InvoiceSearchDTO>>>> Search(InvoiceSearchArgs args)
        {
            try
            {
                var result = await Manager.Search(args);
                return new WebResponse<ListSearchResponse<IEnumerable<InvoiceSearchDTO>>>(result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new WebResponse<ListSearchResponse<IEnumerable<InvoiceSearchDTO>>>(ex);
            }
        }
    }
}
