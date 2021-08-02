using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class PaymentGatewayRepository : GenericRepository<PaymentGateway>, IPaymentGatewayRepository
    {
        public PaymentGatewayRepository(PardakhtPayDbContext context,
            IServiceProvider provider) : base(context, provider)
        {

        }
    }
}
