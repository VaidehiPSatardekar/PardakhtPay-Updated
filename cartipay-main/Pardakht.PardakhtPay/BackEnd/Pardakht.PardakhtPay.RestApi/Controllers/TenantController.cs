//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Pardakht.PardakhtPay.Application.Interfaces;
//using Pardakht.PardakhtPay.Shared.Models.WebService;

//namespace Pardakht.PardakhtPay.RestApi.Controllers
//{
//    /// <summary>
//    /// Manages tenant operations
//    /// </summary>
//    //[Route("api/tenant")]
//    [ApiController]
//    [ApiExplorerSettings(IgnoreApi = true)]
//    public class TenantController : PardakhtPayBaseController
//    {
//        ITenantService _Service = null;

//        /// <summary>
//        /// Initialize a new instance of this class
//        /// </summary>
//        /// <param name="service"></param>
//        /// <param name="logger"></param>
//        public TenantController(ITenantService service,
//            ILogger<TenantController> logger) : base(logger)
//        {
//            _Service = service;
//        }

//        /// <summary>
//        /// Returns tenants
//        /// </summary>
//        /// <returns>List of <see cref="TenantSearchDTO"/></returns>
//        /// <response code="500">if there is an internal server error</response>
//        [HttpGet("api/tenant/gettenants")]
//        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TenantSearchDTO>))]
//        [Authorize]
//        public async Task<IActionResult> Search()
//        {
//            var response = await _Service.Search(Request.Headers["Authorization"]);

//            if (response.Success)
//            {
//                return Ok(response.Payload);
//            }

//            return StatusCode(StatusCodes.Status500InternalServerError);
//        }

//        //[HttpGet]
//        //[HttpDelete]
//        //[Authorize]
//        //[Route("api/tenantplatform/{*remaining}")]
//        //public async Task<IActionResult> HttpGetDelete()
//        //{
//        //    try
//        //    {
//        //        return await _TenantManagementFunctions.GenericRequest(null, User, Request);
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Logger.LogError("HttpGetDelete Error", ex);
//        //        return StatusCode(StatusCodes.Status500InternalServerError);
//        //    }
//        //}

//        //[HttpPut]
//        //[HttpPost]
//        //[Authorize]
//        //[Route("api/tenantplatform/{*remaining}")]
//        //public async Task<IActionResult> HttpPostPut([FromBody] object request)
//        //{
//        //    try
//        //    {
//        //        return await _TenantManagementFunctions.GenericRequest(request, User, Request);
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Logger.LogError("HttpPostPut Error", ex);
//        //        return StatusCode(StatusCodes.Status500InternalServerError);
//        //    }
//        //}
//    }
//}