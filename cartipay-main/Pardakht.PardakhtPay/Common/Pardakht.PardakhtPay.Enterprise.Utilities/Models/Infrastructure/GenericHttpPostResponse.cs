using System.Net;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Models.Infrastructure
{
    public class GenericHttpPostResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string InnerMessage { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public T Payload { get; set; }
    }
}
