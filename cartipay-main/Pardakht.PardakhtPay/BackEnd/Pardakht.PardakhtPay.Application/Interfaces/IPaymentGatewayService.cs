using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IPaymentGatewayService : IServiceBase<PaymentGateway, IPaymentGatewayManager>
    {
        Task<WebResponse<List<PaymentGatewayDTO>>> GetAllItemsAsync();

        Task<WebResponse<PaymentGatewayDTO>> InsertAsync(PaymentGatewayDTO item);

        Task<WebResponse<PaymentGatewayDTO>> UpdateAsync(PaymentGatewayDTO item);

        Task<WebResponse<PaymentGatewayDTO>> GetById(int id);
    }
}
