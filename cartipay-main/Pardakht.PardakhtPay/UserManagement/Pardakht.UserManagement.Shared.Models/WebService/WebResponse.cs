namespace Pardakht.UserManagement.Shared.Models.WebService
{
    public class WebResponse<T> where T : class
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Payload { get; set; }
    }

    public class WebResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

}
