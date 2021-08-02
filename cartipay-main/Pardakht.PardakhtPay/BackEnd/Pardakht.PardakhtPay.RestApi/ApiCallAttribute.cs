using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.RestApi
{
    /// <summary>
    /// Represents an attribute to define the operation is an API call
    /// </summary>
    public class ApiCallAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Sets current request is and api call
        /// </summary>
        /// <param name="context">Execution context</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var currentUser = context.HttpContext.RequestServices.GetRequiredService<CurrentUser>();

            currentUser.ApiCall = true;

            base.OnActionExecuting(context);
        }
    }
}
