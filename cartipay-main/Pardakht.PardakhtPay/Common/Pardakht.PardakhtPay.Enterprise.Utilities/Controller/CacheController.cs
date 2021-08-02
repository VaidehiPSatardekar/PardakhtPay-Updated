//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.Tenant;

//namespace Pardakht.PardakhtPay.Enterprise.Utilities.Controller
//{
//    [Route("cache")]
//    [Authorize]
//    [ApiController]
//    public class CacheController : ControllerBase
//    {
//        private ITenantResolverService tenantResolverService;
//        public CacheController(ITenantResolverService tenantResolverService)
//        {
//            this.tenantResolverService = tenantResolverService;
//        }
//        [HttpGet]
//        public async Task<ActionResult> RefreshTenantsCache()
//        {
//            try
//            {
//                await tenantResolverService.GetTenants(true);
//                return Ok();
//            }
//            catch (Exception)
//            {
//                return BadRequest();
//            }
//        }
//    }
//}