using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.GenericManagementApi;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.NotificationSignalR;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Notification;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Services.Notification
{
    public class NotificationSignalRService : INotificationSignalR
    {
        private readonly IServiceProvider serviceProvider;

        public NotificationSignalRService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public async Task<HttpResponseMessage> AddUserToGroup(HubEventSubscriptionRequest request)
        {
            var managementFunctions = serviceProvider.GetRequiredService<IGenericManagementFunctions<NotificationManagementSettings>>();
            HttpResponseMessage httpResponseMessage = await managementFunctions.MakeRequest(request, null, $"/api/NotificationSignalR/addusertosignalrgroup", System.Net.Http.HttpMethod.Post);

            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            if (httpResponseMessage == null || !httpResponseMessage.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }

            return httpResponseMessage;
        }

        public async Task<HubConnectionResponse> GetHubConnectionDetails(HubConnectionRequest request)
        {

            var managementFunctions = serviceProvider.GetRequiredService<IGenericManagementFunctions<NotificationManagementSettings>>();
            HttpResponseMessage httpResponseMessage = await managementFunctions.MakeRequest(request, null, $"/api/NotificationSignalR/gethubconnectiondetails", System.Net.Http.HttpMethod.Post);

            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            if (httpResponseMessage == null || !httpResponseMessage.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }

            return  JsonConvert.DeserializeObject<HubConnectionResponse>(content);
        }

        public async Task<HttpResponseMessage> RemoveUserFromGroup(HubEventSubscriptionRequest request)
        {
            var managementFunctions = serviceProvider.GetRequiredService<IGenericManagementFunctions<NotificationManagementSettings>>();
            HttpResponseMessage httpResponseMessage = await managementFunctions.MakeRequest(request, null, $"/api/NotificationSignalR/removeusertosignalrgroup", System.Net.Http.HttpMethod.Post);

            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            if (httpResponseMessage == null || !httpResponseMessage.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }

            return httpResponseMessage;
        }

        public async Task<HttpResponseMessage> SendMessageToAllUsers(SendMessageToAllUserSignalRRequest request)
        {
            var managementFunctions = serviceProvider.GetRequiredService<IGenericManagementFunctions<NotificationManagementSettings>>();
            HttpResponseMessage httpResponseMessage = await managementFunctions.MakeRequest(request, null, $"/api/NotificationSignalR/sendmessagetoallusers", System.Net.Http.HttpMethod.Post);

            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            if (httpResponseMessage == null || !httpResponseMessage.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }

            return httpResponseMessage;
        }

        public async Task<HttpResponseMessage> SendMessageToGroup(SendMessageToGroupSignalRRequest request)
        {
            var managementFunctions = serviceProvider.GetRequiredService<IGenericManagementFunctions<NotificationManagementSettings>>();
            HttpResponseMessage httpResponseMessage = await managementFunctions.MakeRequest(request, null, $"/api/NotificationSignalR/sendmessagetogroup", System.Net.Http.HttpMethod.Post);

            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            if (httpResponseMessage == null || !httpResponseMessage.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }
            return httpResponseMessage;
        }

        public async Task<HttpResponseMessage> SendMessageToUser(SendMessageToCustomerSignalRRequest request)
        {
            var managementFunctions = serviceProvider.GetRequiredService<IGenericManagementFunctions<NotificationManagementSettings>>();
            HttpResponseMessage httpResponseMessage = await managementFunctions.MakeRequest(request, null, $"/api/NotificationSignalR/sendmessagetouser", System.Net.Http.HttpMethod.Post);

            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            if (httpResponseMessage == null || !httpResponseMessage.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }
            return httpResponseMessage;
        }
    }
}
