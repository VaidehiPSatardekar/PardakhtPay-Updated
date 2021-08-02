using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Microsoft.Extensions.Options;

namespace Pardakht.PardakhtPay.RestApi
{
    /// <summary>
    /// Represents a class which manages log operations for operations
    /// </summary>
    public class ActionLogAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Initialize a new instance of this class
        /// </summary>
        public ActionLogAttribute()
        {
        }

        /// <summary>
        /// Log operation
        /// </summary>
        /// <param name="context">Execution context</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var logConfiguration = context.HttpContext.RequestServices.GetRequiredService<IOptions<LogConfiguration>>();

            if (logConfiguration.Value.LogActions)
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ActionLogAttribute>>();

                string data = string.Empty;

                if (context.ActionArguments != null && context.ActionArguments.Keys.Count > 0)
                {
                    data = JsonConvert.SerializeObject(context.ActionArguments);

                    logger.LogInformation($"Action Executing. Action :{context.Controller.GetType().Name}.{context.ActionDescriptor.DisplayName} Data : {data}");
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
