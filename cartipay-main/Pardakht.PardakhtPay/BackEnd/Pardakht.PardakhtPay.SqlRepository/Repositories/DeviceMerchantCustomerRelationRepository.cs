using System;
using Pardakht.PardakhtPay.Infrastructure.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Entities;

namespace Pardakht.PardakhtPay.SqlRepository.Repositories
{
    public class DeviceMerchantCustomerRelationRepository : GenericRepository<DeviceMerchantCustomerRelation>, IDeviceMerchantCustomerRelationRepository
    {
        public DeviceMerchantCustomerRelationRepository(PardakhtPayDbContext context, IServiceProvider provider):base(context, provider)
        {

        }
    }
}
