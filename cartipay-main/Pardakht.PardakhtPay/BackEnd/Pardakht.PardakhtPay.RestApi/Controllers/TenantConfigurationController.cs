//using System;
//using System.Collections.Generic;
//using System.Linq;
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
//    /// Manages tenant configurations
//    /// </summary>
//    [Route("api/tenantconfiguration")]
//    [ApiController]
//    [Authorize]
//    public class TenantConfigurationController : PardakhtPayBaseController
//    {
//        ITenantApiService _Service = null;

//        /// <summary>
//        /// Initialize a new instance of this class
//        /// </summary>
//        /// <param name="logger"></param>
//        /// <param name="service"></param>
//        public TenantConfigurationController(ILogger<TenantConfigurationController> logger,
//            ITenantApiService service):base(logger)
//        {
//            _Service = service;


//        }

//        /// <summary>
//        /// Returns all tenant configuration
//        /// </summary>
//        /// <returns>List of tenant configurations.</returns>
//        /// <response code="500">if there is an internal server error</response>
//        [HttpGet]
//        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TenantApiDTO>))]
//        public async Task<IActionResult> Get()
//        {
//            var result = await _Service.GetAllItemsAsync();

//            return ReturnWebResponse(result);
//        }

//        /// <summary>
//        /// Returns the configuration with given id
//        /// </summary>
//        /// <param name="id">Id of the configuration</param>
//        /// <returns>Tenant url configuration. <see cref="TenantApiDTO"/></returns>
//        /// <response code="500">if there is an internal server error</response>
//        [HttpGet("{id}")]
//        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TenantApiDTO))]
//        [Authorize(Roles = "CP-URL-CONFIGURATION")]
//        public async Task<IActionResult> Get(int id)
//        {
//            var result = await _Service.GetItemById(id);

//            return ReturnWebResponse(result);
//        }

//        /// <summary>
//        /// Creates new tenant url configuration
//        /// </summary>
//        /// <param name="item">Parameters. <see cref="TenantApiDTO"/></param>
//        /// <returns>Created confugiration entity. <see cref="TenantApiDTO"/></returns>
//        /// <response code="400">If parameters are invalid</response>
//        /// <response code="500">if there is an internal server error</response>
//        [HttpPost]
//        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TenantApiDTO))]
//        [Authorize(Roles = "CP-URL-CONFIGURATION")]
//        public async Task<IActionResult> Post([FromBody]TenantApiDTO item)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest();
//            }

//            var result = await _Service.InsertAsync(item);

//            return ReturnWebResponse(result);
//        }

//        /// <summary>
//        /// Updates a new tenant url configuration
//        /// </summary>
//        /// <param name="id">Id of the configuration</param>
//        /// <param name="item">Parameters. <see cref="TenantApiDTO"/></param>
//        /// <returns>Updated configuration entity. <see cref="TenantApiDTO"/></returns>
//        /// <response code="400">If parameters are invalid</response>
//        /// <response code="500">if there is an internal server error</response>
//        [HttpPut("{id}")]
//        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TenantApiDTO))]
//        [Authorize(Roles = "CP-URL-CONFIGURATION")]
//        public async Task<IActionResult> Put(int id, [FromBody]TenantApiDTO item)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            if (id != item.Id)
//            {
//                return BadRequest("Entity and identity is different");
//            }

//            var result = await _Service.UpdateAsync(item);

//            return ReturnWebResponse(result);
//        }

//        //[HttpDelete("{id}")]
//        //public async Task<IActionResult> Delete(int id)
//        //{
//        //    var result = await _Service.DeleteAsync(id);

//        //    if (result.Success)
//        //    {
//        //        return Json("Operation completed successfully");
//        //    }

//        //    return BadRequest(result.Message);
//        //}
//    }
//}