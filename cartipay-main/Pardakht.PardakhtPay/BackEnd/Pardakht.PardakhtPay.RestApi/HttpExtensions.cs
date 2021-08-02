using Microsoft.AspNetCore.Http;

namespace Pardakht.PardakhtPay.RestApi
{
    public static class HttpExtensions
    {
        const string XForwardHeader = "X-Forwarded-For";
        const string CfHeader = "CF-Connecting-IP";

        /// <summary>
        /// Gets ip address of the request with checking to CF-Connecting-IP, X-Forwarded-For and RemoteIpAddress
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetIpAddress(this HttpContext context)
        {
            if(context == null || context.Request == null)
            {
                return string.Empty;
            }

            if (context.Request.Headers.ContainsKey(CfHeader))
            {
                return context.Request.Headers[CfHeader];
            }

            if (context.Request.Headers.ContainsKey(XForwardHeader))
            {
                return context.Request.Headers[XForwardHeader];
            }

            if(context.Connection == null)
            {
                return string.Empty;
            }

            return context.Connection.RemoteIpAddress.ToString();
        }
    }
}
