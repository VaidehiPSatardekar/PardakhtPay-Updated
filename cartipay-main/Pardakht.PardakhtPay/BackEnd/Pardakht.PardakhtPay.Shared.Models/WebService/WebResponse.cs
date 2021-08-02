using System;

namespace Pardakht.PardakhtPay.Shared.Models.WebService
{
    public class WebResponse<T> : WebResponse where T : class
    {
        public WebResponse()
        {

        }

        public WebResponse(bool success, string message = "", T payload = null)
        {
            Success = success;
            Message = message;
            Payload = payload;
        }

        public WebResponse(T payload)
        {
            Success = true;
            Message = string.Empty;
            Payload = payload;
        }

        public WebResponse(Exception ex)
        {
            Success = false;
            Payload = null;
            Message = ex.Message;
            Exception = ex;
        }
        
        public T Payload { get; set; }
    }

    public class WebResponse
    {

        public WebResponse()
        {
            Success = true;
        }

        public WebResponse(Exception ex)
        {
            Exception = ex;
            Message = ex.Message;
        }

        public bool Success { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }
    }
}
