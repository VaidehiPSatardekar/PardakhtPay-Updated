using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    [Route("api/riskykeyword")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RiskyKeywordController : ControllerBase
    {
        IRiskyKeywordService _Service = null;
        ILogger _Logger = null;

        public RiskyKeywordController(IRiskyKeywordService service,
            ILogger<RiskyKeywordController> logger)
        {
            _Service = service;
            _Logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            try
            {
                var response = await _Service.GetAll();

                if (response.Success)
                {
                    var items = response.Payload.OrderBy(t => t).ToList();
                    return Ok(items);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Put([FromBody]string[] model)
        {
            try
            {
                var response = await _Service.Update(model);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}