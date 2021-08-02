using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.WebService;
using Pardakht.PardakhtPay.Shared.Models.WebService.Bot;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    /// <summary>
    /// Manages bank statement items
    /// </summary>
    [Route("api/statement")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class BankStatementItemController : PardakhtPayBaseController
    {
        IBankStatementItemService _Service = null;
        CurrentUser _CurrentUser = null;
        IWithdrawalService _WithdrawalService;
        IAutoTransferService _AutoTransferService;

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="service"></param>
        /// <param name="currentUser"></param>
        /// <param name="logger"></param>
        public BankStatementItemController(IBankStatementItemService service,
            CurrentUser currentUser,
            ILogger<BankStatementItemController> logger,
            IWithdrawalService withdrawalService,
            IAutoTransferService autoTransferService):base(logger)
        {
            _Service = service;
            _CurrentUser = currentUser;
            _WithdrawalService = withdrawalService;
            _AutoTransferService = autoTransferService;
        }

        /// <summary>
        /// Returns a bank statement item with the given id
        /// </summary>
        /// <description>
        /// Returns a bank statement item with the given id
        /// </description>
        /// <param name="id">Id of the bank statement</param>
        /// <returns>Bank Statement Item. <see cref="BankStatementItemDTO"/></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BankStatementItemDTO))]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _Service.GetItemById(id);

            return ReturnWebResponse(result);
        }

        /// <summary>
        /// It is called from bank bot when a statement report item is created. Creates a new record to database
        /// </summary>
        /// <description>
        /// It is called from bank bot when a statement report item is created. Creates a new record to database
        /// </description>
        /// <param name="item"></param>
        /// <returns>Created statement item. <see cref="BankStatementItemDTO"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BankStatementItemDTO))]
        [ApiCall]
        public async Task<IActionResult> Post([FromBody]BankStatementItemDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            //_CurrentUser.ApiCall = true;
            var result = await _Service.InsertAsync(item);

            if(item.TransferRequestId != 0)
            {
                await _WithdrawalService.CheckWithTransferRequestId(item.TransferRequestId, result.Payload);
                await _AutoTransferService.CheckWithTransferRequestId(item.TransferRequestId, item.RecordId);

            }

            if (result.Success)
            {
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// It is called from bank bot when a statement report items are created. Creates new records to database
        /// </summary>
        /// <description>
        /// It is called from bank bot when a statement report items are created. Creates new records to database
        /// </description>
        /// <param name="items"></param>
        /// <returns>Status code</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost("bulk")]
        [ApiCall]
        public async Task<IActionResult> Post([FromBody]List<BankStatementItemDTO> items)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.InsertAsync(items);

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                if(item.TransferRequestId != 0)
                {
                    await _WithdrawalService.CheckWithTransferRequestId(item.TransferRequestId, item);
                    await _AutoTransferService.CheckWithTransferRequestId(item.TransferRequestId, item.RecordId);
                }
            }

            if (result.Success)
            {
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Search bank statement items
        /// </summary>
        /// <param name="args">Search arguments. <see cref="BankStatementSearchArgs"/></param>
        /// <returns></returns>
        [HttpPost("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BankStatementItemSearchDTO>))]
        [Authorize(Roles = "CP-BANK-STATEMENTS")]
        public async Task<IActionResult> Search([FromBody]BankStatementSearchArgs args)
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
    }
}