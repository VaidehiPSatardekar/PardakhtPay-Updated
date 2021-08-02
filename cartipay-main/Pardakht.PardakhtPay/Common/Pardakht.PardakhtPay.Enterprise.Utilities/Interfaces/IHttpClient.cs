using System.Net.Http;
using System.Threading.Tasks;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> PostAsync(string uri, StringContent content);
        Task<HttpResponseMessage> PostFileAsync(string uri, MultipartFormDataContent content);
        Task<HttpResponseMessage> GetAsync(string uri);
        Task<HttpResponseMessage> PutAsync(string uri, StringContent content);
        Task<HttpResponseMessage> PatchAsync(string uri, StringContent content);
        Task<HttpResponseMessage> DeleteAsync(string uri);

    }
}
