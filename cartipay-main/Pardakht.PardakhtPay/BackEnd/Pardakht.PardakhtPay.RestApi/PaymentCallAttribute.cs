//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Pardakht.PardakhtPay.Shared.Models.WebService;

//namespace Pardakht.PardakhtPay.RestApi
//{
//    public class PaymentCallAttribute : ActionFilterAttribute
//    {
//        public override void OnActionExecuting(ActionExecutingContext context)
//        {
//            var currentUser = context.HttpContext.RequestServices.GetRequiredService<CurrentUser>();

//            if (!currentUser.IsPaymentUrl)
//            {
//                context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
//                context.Result = new NotFoundResult();
//            }
//            else
//            {
//                base.OnActionExecuting(context);
//            }
//        }
//    }
//}
