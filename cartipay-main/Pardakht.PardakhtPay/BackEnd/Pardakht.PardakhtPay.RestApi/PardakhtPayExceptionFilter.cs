using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Pardakht.PardakhtPay.Shared.Extensions;

namespace Pardakht.PardakhtPay.RestApi
{
    /// <summary>
    /// Represents a filter to handle exceptions
    /// </summary>
    public class PardakhtPayExceptionFilter : ExceptionFilterAttribute
    {
        private ILogger Logger { get; set; }

        /// <summary>
        /// Initialize a new instance of this filter
        /// </summary>
        /// <param name="logger"></param>
        public PardakhtPayExceptionFilter(ILogger<PardakhtPayExceptionFilter> logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Handles exception
        /// </summary>
        /// <param name="context">Current <see cref="ExceptionContext"/> instance</param>
        public override void OnException(ExceptionContext context)
        {
            Logger.LogError(context.Exception, context.Exception.GetExceptionDetailsWithStackTrace());

            base.OnException(context);
        }
    }
}
