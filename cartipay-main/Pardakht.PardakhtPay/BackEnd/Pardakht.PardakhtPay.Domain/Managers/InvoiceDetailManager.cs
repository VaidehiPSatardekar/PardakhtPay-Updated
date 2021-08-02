using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class InvoiceDetailManager : BaseManager<InvoiceDetail, IInvoiceDetailRepository>, IInvoiceDetailManager
    {
        public InvoiceDetailManager(IInvoiceDetailRepository repository):base(repository)
        {

        }
    }
}
