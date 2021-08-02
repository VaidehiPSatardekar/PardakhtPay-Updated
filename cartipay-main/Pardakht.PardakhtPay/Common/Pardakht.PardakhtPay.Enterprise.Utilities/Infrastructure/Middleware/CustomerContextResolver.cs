using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Customer;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.Middleware
{
    public class CustomerContextResolver
    {
        private readonly RequestDelegate next;

        public CustomerContextResolver(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var customerContext = context.RequestServices.GetRequiredService<CustomerContext>();

            if (context.User != null && context.User.Claims != null)
            {
                customerContext.CustomerGuid = context.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
                customerContext.Username = context.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value;

                var customerData = context.User.Claims.FirstOrDefault(p => p.Type == "CustomerData")?.Value;
                if (!string.IsNullOrEmpty(customerData))
                {
                    customerContext.CustomerData = JsonConvert.DeserializeObject<CustomerData>(customerData);
                }
            }

            await next(context);
        }
    }
}
