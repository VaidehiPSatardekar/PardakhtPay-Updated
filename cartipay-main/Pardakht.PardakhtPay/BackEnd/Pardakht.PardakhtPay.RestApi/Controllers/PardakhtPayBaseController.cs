using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.RestApi.Controllers
{
    /// <summary>
    /// Base controller for api calls
    /// </summary>
    public abstract class PardakhtPayBaseController : Controller
    {
        /// <summary>
        /// Logger instance
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        /// <param name="logger"></param>
        public PardakhtPayBaseController(ILogger logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Returns response depends on the web response
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        protected IActionResult ReturnWebResponse<T>(WebResponse<T> response) where T:class
        {
            if (!response.Success)
            {
                if (response.Exception != null)
                {
                    Logger.LogError(response.Exception, response.Message);
                }
                else
                {
                    Logger.LogError(response.Message);
                }

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Json(response.Payload);
        }

        /// <summary>
        /// Returns response depends on the web response
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected IActionResult WebResponseAction(WebResponse response)
        {
            if (!response.Success)
            {
                if (response.Exception != null)
                {
                    Logger.LogError(response.Exception, response.Message);
                }
                else
                {
                    Logger.LogError(response.Message);
                }

                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
            }

            return Json("Operation completed successfully");
        }
    }
}