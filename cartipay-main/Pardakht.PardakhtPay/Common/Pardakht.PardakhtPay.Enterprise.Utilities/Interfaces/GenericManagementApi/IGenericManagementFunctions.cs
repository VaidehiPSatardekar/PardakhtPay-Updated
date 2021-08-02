using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.GenericManagementApi
{
    public interface IGenericManagementFunctions<T> : IGenericManagementFunctions where T : ApiSettings, new()
    {
    }

    public interface IGenericManagementFunctions
    {
        Task<IActionResult> GenericRequest(object request, ClaimsPrincipal user, HttpRequest httpRequest, Dictionary<string, string> dictionary = null);
        Task<HttpResponseMessage> MakeRequest(object request, ClaimsPrincipal user, string url, HttpMethod method, Dictionary<string, string> dictionary = null);
    }
}
