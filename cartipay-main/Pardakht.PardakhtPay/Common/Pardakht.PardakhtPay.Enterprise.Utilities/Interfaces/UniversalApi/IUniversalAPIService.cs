//using System.Collections.Generic;
//using System.Net.Http;
//using System.Threading.Tasks;

//{
//    public interface IUniversalApiService
//    {
//        Task<HttpResponseMessage> GetAsync(string url, HttpHeader httpHeader, string dependencyName,
//            string dependencyFunction, string accept = "application/json");

//        Task<HttpResponseMessage> PutAsync(string url, HttpHeader httpHeader, string dependencyName,
//            string dependencyFunction, string body, string accept = "application/json",
//            string contentType = "application/json");

//        Task<HttpResponseMessage> PostAsync(string url, HttpHeader httpHeader, string dependencyName,
//            string dependencyFunction, string body, string accept = "application/json",
//            string contentType = "application/json");

//        Task<HttpResponseMessage> PatchAsync(string url, HttpHeader httpHeader, string dependencyName,
//            string dependencyFunction, string body, string accept = "application/json",
//            string contentType = "application/json");

//        Task<HttpResponseMessage> DeleteAsync(string url, HttpHeader httpHeader, string dependencyName,
//            string dependencyFunction, string accept = "application/json");

//        Task<HttpResponseMessage> GetAsync(string url, HttpHeader httpHeader, string accept = "application/json");

//        Task<HttpResponseMessage> GetProxyAsync(string url, HttpHeader httpHeader, string ipAddress,
//            string accept = "application/json");

//        Task<HttpResponseMessage> GetProxyAsync(string url, HttpHeader httpHeader, string ipAddress, string username,
//            string password, string accept = "application/json");

//        Task<HttpResponseMessage> PutAsync(string url, HttpHeader httpHeader, string body,
//            string accept = "application/json", string contentType = "application/json");

//        Task<HttpResponseMessage> PutProxyAsync(string url, HttpHeader httpHeader, string body, string ipAddress,
//            string accept = "application/json", string contentType = "application/json");

//        Task<HttpResponseMessage> PutProxyAsync(string url, HttpHeader httpHeader, string body, string ipAddress,
//            string username, string password, string accept = "application/json",
//            string contentType = "application/json");

//        Task<HttpResponseMessage> PostAsync(string url, HttpHeader httpHeader, string body,
//            string accept = "application/json", string contentType = "application/json");

//        Task<HttpResponseMessage> PostProxyAsync(string url, HttpHeader httpHeader, string body, string ipAddress,
//            string username, string password, string accept = "application/json",
//            string contentType = "application/json");

//        Task<HttpResponseMessage> PostProxyAsync(string url, HttpHeader httpHeader, string body, string ipAddress,
//            string accept = "application/json", string contentType = "application/json");

//        Task<HttpResponseMessage> PosCollectionsAsync(string url, HttpHeader httpHeader,
//             IDictionary<string, StreamContent> streamContents, IDictionary<string, StringContent> stringContents, string accept = "application/json");

//        Task<HttpResponseMessage> PostFileAsync(string url, HttpHeader httpHeader, StreamContent streamContent,
//            string accept = "application/json");

//        Task<HttpResponseMessage> PostFileProxyAsync(string url, HttpHeader httpHeader, StreamContent streamContent,
//            string ipAddress, string username, string password, string accept = "application/json");

//        Task<HttpResponseMessage> PostFileProxyAsync(string url, HttpHeader httpHeader, StreamContent streamContent,
//            string ipAddress, string accept = "application/json");

//        Task<HttpResponseMessage> PatchAsync(string url, HttpHeader httpHeader, string body,
//            string accept = "application/json", string contentType = "application/json");

//        Task<HttpResponseMessage> PatchProxyAsync(string url, HttpHeader httpHeader, string body, string ipAddress,
//            string accept = "application/json", string contentType = "application/json");

//        Task<HttpResponseMessage> DeleteAsync(string url, HttpHeader httpHeader, string accept = "application/json");

//        Task<HttpResponseMessage> DeleteProxyAsync(string url, HttpHeader httpHeader, string ipAddress, string username,
//            string password, string accept = "application/json");

//        Task<HttpResponseMessage> DeleteProxyAsync(string url, HttpHeader httpHeader, string ipAddress,
//            string accept = "application/json");
//    }
//}
