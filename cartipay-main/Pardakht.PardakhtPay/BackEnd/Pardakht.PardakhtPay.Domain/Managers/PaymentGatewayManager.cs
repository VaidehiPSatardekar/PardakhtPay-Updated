using Pardakht.PardakhtPay.Domain.Base;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.Domain.Managers
{
    public class PaymentGatewayManager : BaseManager<PaymentGateway, IPaymentGatewayRepository>, IPaymentGatewayManager
    {
        public PaymentGatewayManager(IPaymentGatewayRepository repository) : base(repository)
        {

        }
    }
}
