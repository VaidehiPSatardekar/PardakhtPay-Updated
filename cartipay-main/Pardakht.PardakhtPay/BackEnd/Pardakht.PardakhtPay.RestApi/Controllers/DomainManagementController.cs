//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.GenericManagementApi;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;

//namespace Pardakht.PardakhtPay.RestApi.Controllers
//{
//    [ApiController]
//    [ApiExplorerSettings(IgnoreApi = true)]
//    public class DomainManagementController : ControllerBase
//    {
//        private readonly ILogger<DomainManagementController> logger;
//        private readonly IGenericManagementFunctions<DomainManagementSettings> domainManagementFunctions;

//        public DomainManagementController(IGenericManagementFunctions<DomainManagementSettings> domainManagementFunctions, ILogger<DomainManagementController> logger)
//        {
//            this.domainManagementFunctions = domainManagementFunctions;
//            this.logger = logger;
//        }

//        [HttpGet]
//        [Authorize]
//        [Route("api/domain/{*remaining}")]
//        public async Task<IActionResult> HttpGet()
//        {
//            try
//            {
//                return await domainManagementFunctions.GenericRequest(null, User, Request);
//            }
//            catch (Exception ex)
//            {
//                logger.LogError($"DomainManagementController.HttpGet: an error occurred - {ex}");
//                return StatusCode(StatusCodes.Status500InternalServerError);
//            }
//        }

//        [HttpPost]
//        [Authorize]
//        [Route("api/domain/{*remaining}")]
//        public async Task<IActionResult> HttpPost([FromBody] object request, string remaining)
//        {
//            try
//            {
//                return await domainManagementFunctions.GenericRequest(request, User, Request);
//            }
//            catch (Exception ex)
//            {
//                logger.LogError($"DomainManagementController.HttpPost: an error occurred creating domain - {ex}");
//                return StatusCode(StatusCodes.Status500InternalServerError);
//            }
//        }

//        [HttpPut]
//        [Authorize]
//        [Route("api/domain/{*remaining}")]
//        public async Task<IActionResult> HttpPut([FromBody] object request, string remaining)
//        {
//            try
//            {
//                return await domainManagementFunctions.GenericRequest(request, User, Request);
//            }
//            catch (Exception ex)
//            {
//                logger.LogError($"DomainManagementController.HttpPut: an error occurred updating domain - {ex}");
//                return StatusCode(StatusCodes.Status500InternalServerError);
//            }
//        }
//    }
//}