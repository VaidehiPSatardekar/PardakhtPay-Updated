using System.ComponentModel.DataAnnotations;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Models.Notification
{
    public class HubConnectionRequest
    {
        [Required]
        public string UserId { get; set; }
    }

    public class HubEventSubscriptionRequest : HubSubscriptionRequest
    {
        [Required]
        public string EventId { get; set; }
    }

    public class HubSubscriptionRequest
    {
        [Required]
        public string UserId { get; set; }
    }

    public class HubConnectionResponse
    {
        public string Url { get; set; }
        public string Token { get; set; }
    }
    public class SendMessageToCustomerSignalRRequest
    {
        public string MethodName { get; set; }

        public string UserId { get; set; }
        public object[] Args { get; set; }
    }

    public class SendMessageToAllUserSignalRRequest
    {
        public string MethodName { get; set; }

        public string UserId { get; set; }
        public object[] Args { get; set; }
    }

    public class SendMessageToGroupSignalRRequest
    {
        public string MethodName { get; set; }

        public string Group { get; set; }
        public object[] Args { get; set; }
    }
}
