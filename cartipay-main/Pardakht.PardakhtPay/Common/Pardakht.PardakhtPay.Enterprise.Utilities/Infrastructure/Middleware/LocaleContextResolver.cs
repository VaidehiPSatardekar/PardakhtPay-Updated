using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.Helpers;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.Language;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Language;
using Serilog;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.Middleware
{
    public class LocaleContextResolver
    {
        RequestDelegate next;

        public LocaleContextResolver(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var locale = UriHelper.GetCustomHeader(context, "locale");
            if (!string.IsNullOrEmpty(locale))
            {
                var localeInfo = context.RequestServices.GetRequiredService<LocaleInfo>();
                var resolver = context.RequestServices.GetRequiredService<ILocaleResolver>();

                var requestLocaleInfo = resolver.Resolve(locale);

                if (requestLocaleInfo != null)
                {
                    Log.Information($"Using locale {requestLocaleInfo.Name} for this request");
                    localeInfo.Clone(requestLocaleInfo);
                }
                else
                {
                    Log.Information($"Unable to get locale info for {locale}");
                }
            }
            else
            {
              //  Log.Information("No specific locale being used for this request");
            }

            await next.Invoke(context);
        }
    }
}
