using System.Net.Http;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Notification;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.NotificationSignalR
{
    public interface INotificationSignalR
    {
        Task<HubConnectionResponse> GetHubConnectionDetails(HubConnectionRequest request);
        Task<HttpResponseMessage> SendMessageToGroup(SendMessageToGroupSignalRRequest request);
        Task<HttpResponseMessage> SendMessageToUser(SendMessageToCustomerSignalRRequest request);
        Task<HttpResponseMessage> SendMessageToAllUsers(SendMessageToAllUserSignalRRequest request);
        Task<HttpResponseMessage> AddUserToGroup(HubEventSubscriptionRequest request);
        Task<HttpResponseMessage> RemoveUserFromGroup(HubEventSubscriptionRequest request);
    }
}
