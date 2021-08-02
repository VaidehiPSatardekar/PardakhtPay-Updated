using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    [Route("api/manualtransfer")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ManualTransferController : PardakhtPayBaseController
    {
        IManualTransferService _Service;

        public ManualTransferController(IManualTransferService service,
            ILogger<ManualTransferController> logger):base(logger)
        {
            _Service = service;
        }
        
        [HttpPost("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ManualTransferDTO>))]
        [Authorize(Roles = "CP-MANUAL-TRANSFERS")]
        public async Task<IActionResult> Search([FromBody]ManualTransferSearchArgs args)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _Service.Search(args);

                return ReturnWebResponse(result);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "CP-MANUAL-TRANSFERS")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _Service.GetItemById(id);

            return ReturnWebResponse(result);
        }

        [HttpPost]
        [Authorize(Roles = "CP-ADD-MANUAL-TRANSFER")]
        public async Task<IActionResult> Post([FromBody]ManualTransferDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.InsertAsync(item);

            return ReturnWebResponse(result);
        }

        [HttpPost("canceldetail/{detailId}")]
        [Authorize(Roles = "CP-ADD-MANUAL-TRANSFER")]
        public async Task<IActionResult> CancelDetail(int detailId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.CancelTransferDetail(detailId);

            return ReturnWebResponse(result);
        }

        [HttpPost("retrydetail/{detailId}")]
        [Authorize(Roles = "CP-ADD-MANUAL-TRANSFER")]
        public async Task<IActionResult> RetryDetail(int detailId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.RetryTransferDetail(detailId);

            return ReturnWebResponse(result);
        }

        [HttpPost("setascompleteddetail/{detailId}")]
        [Authorize(Roles = "CP-ADD-MANUAL-TRANSFER")]
        public async Task<IActionResult> SetAsCompletedDetail(int detailId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.SetAsCompletedTransferDetail(detailId);

            return ReturnWebResponse(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "CP-ADD-MANUAL-TRANSFER")]
        public async Task<IActionResult> Put(int id, [FromBody]ManualTransferDTO item)
         {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != item.Id)
            {
                return BadRequest("Entity and identity is different");
            }

            var result = await _Service.UpdateAsync(item);

            return ReturnWebResponse(result);
        }

        [HttpPut("cancel/{id}")]
        [Authorize(Roles = "CP-ADD-MANUAL-TRANSFER")]
        public async Task<IActionResult> Cancel(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _Service.CancelAsync(id);

            return ReturnWebResponse(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "CP-ADD-MANUAL-TRANSFER")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _Service.DeleteAsync(id);

            if (result.Success)
            {
                return Ok();
            }

            return BadRequest(result.Message);
        }

        [HttpPost("gettransferreceipt/{id}")]
        [Authorize(Roles = "CP-MANUAL-TRANSFERS")]
        public async Task<IActionResult> GetTransferReceipt(int id)
        {
            try
            {
                var result = await _Service.GetTransferReceipt(id);

                if (!result.Success)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

                string fileType = result.Payload.ContentType == "application/pdf" ? ".pdf" : ".png";
                Response.Headers.Add("File-Name", $"{id}_{result.Payload.TrackingNumber}_{DateTime.Now.ToString("ddMMyyyHHmmss")}{fileType}");

                return File(result.Payload.Data, result.Payload.ContentType, $"{id}_{Guid.NewGuid().ToString()}{fileType}");

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}