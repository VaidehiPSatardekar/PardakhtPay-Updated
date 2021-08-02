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
    /// <summary>
    /// Manages user segment groups and segments
    /// </summary>
    [Route("api/usersegmentgroup")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class UserSegmentGroupController : PardakhtPayBaseController
    {
        IUserSegmentGroupService _Service = null;

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="service"></param>
        public UserSegmentGroupController(ILogger<UserSegmentGroupController> logger,
            IUserSegmentGroupService service):base(logger)
        {
            _Service = service;
        }

        /// <summary>
        /// Returns all user segment groups
        /// </summary>
        /// <description>
        /// Returns all user segment groups
        /// </description>
        /// <returns>List of <see cref="UserSegmentGroupDTO"/></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserSegmentGroupDTO>))]
        public async Task<IActionResult> Get()
        {
            var result = await _Service.GetAllItemsAsync();

            return ReturnWebResponse(result);
        }

        /// <summary>
        /// Returns a user segment group with the given id
        /// </summary>
        /// <description>
        /// Returns a user segment group with the given id
        /// </description>
        /// <param name="id">id of the user segment group</param>
        /// <returns>User segment group with given id. <see cref="UserSegmentGroupDTO"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserSegmentGroupDTO))]
        [Authorize(Roles = "CP-ADD-USER-SEGMENT-GROUP")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _Service.GetItemById(id);

            return ReturnWebResponse(result);
        }

        /// <summary>
        /// Creates a new user segment group
        /// </summary>
        /// <description>
        /// Creates a new user segment group
        /// </description>
        /// <param name="item">User segment group parameters. <see cref="UserSegmentGroupDTO"/></param>
        /// <returns>Creates user segment groups</returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPost]
        [Authorize(Roles = "CP-ADD-USER-SEGMENT-GROUP")]
        public async Task<IActionResult> Post([FromBody]UserSegmentGroupDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _Service.InsertAsync(item);

            return ReturnWebResponse(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "CP-ADD-USER-SEGMENT-GROUP")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _Service.DeleteAsync(id);

            if (result.Success)
            {
                return Ok();
            }

            return BadRequest(result.Message);
        }

        /// <summary>
        /// Updates a user segment group
        /// </summary>
        /// <description>
        /// Updates a user segment group
        /// </description>
        /// <param name="id">Id of the user segment group</param>
        /// <param name="item">User segment group dto. <see cref="UserSegmentGroupDTO"/></param>
        /// <returns>Updated user segment group. <see cref="UserSegmentGroupDTO"/></returns>
        /// <response code="400">If parameters are invalid</response>
        /// <response code="500">if there is an internal server error</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserSegmentGroupDTO))]
        [Authorize(Roles = "CP-ADD-USER-SEGMENT-GROUP")]
        public async Task<IActionResult> Put(int id, [FromBody]UserSegmentGroupDTO item)
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
    }
}