using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Invoice;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IInvoiceOwnerSettingService : IServiceBase<InvoiceOwnerSetting, IInvoiceOwnerSettingManager>
    {
        Task<WebResponse<List<InvoiceOwnerSettingDTO>>> GetAllItemsAsync();

        Task<WebResponse<InvoiceOwnerSettingDTO>> GetInvoiceOwnerSettingById(int id);

        Task<WebResponse<InvoiceOwnerSettingDTO>> InsertAsync(InvoiceOwnerSettingDTO item);

        Task<WebResponse<InvoiceOwnerSettingDTO>> UpdateAsync(InvoiceOwnerSettingDTO item);
    }
}
