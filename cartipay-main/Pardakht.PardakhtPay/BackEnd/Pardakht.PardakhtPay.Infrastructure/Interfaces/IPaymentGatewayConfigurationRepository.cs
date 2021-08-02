﻿using Pardakht.PardakhtPay.Shared.Models.Entities;
using Pardakht.PardakhtPay.Shared.Interfaces;

namespace Pardakht.PardakhtPay.Infrastructure.Interfaces
{
    public interface IPaymentGatewayConfigurationRepository : IGenericRepository<PaymentGatewayConfiguration>
    {
    }
}
