using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.Helpers
{
    public class UriHelper
    {
        public static string GetCustomHeader(HttpContext httpContext, string headerName)
        {
            var header = httpContext.Request.Headers.Where(w => w.Key.ToLower() == headerName.ToLower()).FirstOrDefault();

            if (header.Key != null)
            {
                return header.Value;
            }

            return string.Empty;
        }
    }
}
