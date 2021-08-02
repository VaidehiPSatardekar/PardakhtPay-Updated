//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using Pardakht.PardakhtPay.Shared.Models.Entities;
//using Pardakht.PardakhtPay.Shared.Models.WebService;

//namespace Pardakht.PardakhtPay.Application.Interfaces
//{
//    public interface ITenantCacheService
//    {
//        Task<List<TenantCacheDTO>> LoadTenants();

//        Task<List<TenantSearchDTO>> GetTenants();

//        Task<string> GetTenantConnectionString(string tenantPlatfromGuid);

//        Task<string> GetCallbackUrl(string tenantPlatformGuid);

//        Task<string> GetTenantGuid(string url);

//        Task<int> GetMerchantIdFromUrl(string url);

//        Task<TenantApi> GetTenantApi(string url);
//    }
//}
