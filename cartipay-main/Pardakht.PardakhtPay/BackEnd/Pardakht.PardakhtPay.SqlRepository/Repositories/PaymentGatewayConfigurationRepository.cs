using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class PaymentGatewayConfigurationRepository : GenericRepository<PaymentGatewayConfiguration>, IPaymentGatewayConfigurationRepository
    {
        public PaymentGatewayConfigurationRepository(PardakhtPayDbContext context,
            IServiceProvider provider) : base(context, provider)
        {

        }
    }
}
