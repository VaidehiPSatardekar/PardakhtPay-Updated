using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.GenericManagementApi
{
    public interface ISystemFunctions<T> where T : ApiSettings, new()
    {
        Task<HttpResponseMessage> MakeRequest(object request, string url, HttpMethod method, Dictionary<string, string> dictionary = null);
        string GenerateJwtToken();
        string GenerateJwtToken(string platformGuid, string jwtKey);


    }
}
