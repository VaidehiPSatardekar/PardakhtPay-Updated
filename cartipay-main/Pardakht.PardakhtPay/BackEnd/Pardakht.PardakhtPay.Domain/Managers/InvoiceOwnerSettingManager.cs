using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class InvoiceOwnerSettingManager : BaseManager<InvoiceOwnerSetting, IInvoiceOwnerSettingRepository>, IInvoiceOwnerSettingManager
    {
        public InvoiceOwnerSettingManager(IInvoiceOwnerSettingRepository repository):base(repository)
        {

        }
    }
}
