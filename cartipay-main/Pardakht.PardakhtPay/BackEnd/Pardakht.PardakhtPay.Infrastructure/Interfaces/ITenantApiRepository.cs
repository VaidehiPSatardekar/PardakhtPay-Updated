//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using Pardakht.PardakhtPay.Shared.Models.Entities;
//using Pardakht.PardakhtPay.Shared.Interfaces;

//namespace Pardakht.PardakhtPay.Infrastructure.Interfaces
//{
//    public interface ITenantApiRepository : IGenericRepository<TenantApi>
//    {
//        string GetCallbackUrl(string tenantGuid);

//        string GetTenantGuid(string url);

//        Task<int> GetMerchantId(string url);

//        Task<TenantApi> GetTenantApi(string url);
//    }
//}
