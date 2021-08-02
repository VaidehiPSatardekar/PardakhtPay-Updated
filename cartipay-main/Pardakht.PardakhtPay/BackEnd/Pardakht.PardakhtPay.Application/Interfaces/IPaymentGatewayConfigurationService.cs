using System.Collections.Generic;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Interfaces
{
    public interface IPaymentGatewayConfigurationService : IServiceBase<PaymentGatewayConfiguration, IPaymentGatewayConfigurationManager>
    {
        Task<WebResponse<List<PaymentGatewayConfigurationDTO>>> GetAllItemsAsync();

        Task<WebResponse<PaymentGatewayConfigurationDTO>> InsertAsync(PaymentGatewayConfigurationDTO item);

        Task<WebResponse<PaymentGatewayConfigurationDTO>> UpdateAsync(PaymentGatewayConfigurationDTO item);

        Task<WebResponse<PaymentGatewayConfigurationDTO>> GetItemById(int id);
    }
}
