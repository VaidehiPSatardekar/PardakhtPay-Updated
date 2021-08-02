using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Models;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    [Route("api/cardholdername")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CardHolderNameController : ControllerBase
    {
        ICardHolderNameService _Service = null;
        ILogger _Logger = null;

        public CardHolderNameController(ICardHolderNameService service,
            ILogger<CardHolderNameController> logger)
        {
            _Service = service;
            _Logger = logger;
        }

        [HttpPost]
        [ApiCall]
        [Authorize]
        public async Task<IActionResult> Post([FromBody]CardHolderNameResponse model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var response = await _Service.UpdateCardHolderName(model);

                if (response.Success)
                {
                    return Ok();
                }

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("getbycardnumber/{cardNumber}")]
        [ApiCall]
        [Authorize(Roles = Permissions.ListCardHolderNames)]
        public async Task<IActionResult> GetByCardNumber(string cardNumber)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var response = await _Service.GetCardHolderName(cardNumber);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }

                return BadRequest(response.Message);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("getaccountholdername")]
        [ApiCall]
        [Authorize]
        public async Task<IActionResult> GetAccountHolderName([FromBody]CardHolderNameDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var response = await _Service.GetAccountName(model);

                if (response.Success)
                {
                    return Ok(response.Payload);
                }

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}