using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Pardakht.PardakhtPay.Application.Interfaces;
using Pardakht.PardakhtPay.Domain.Interfaces;
using Pardakht.PardakhtPay.Shared.Models.Configuration;
using Pardakht.PardakhtPay.Shared.Models.WebService;

namespace Pardakht.PardakhtPay.Application.Services
{
    public class AccountService : IAccountService
    {
        CurrentUser _CurrentUser;
        IUserSegmentGroupManager _UserSegmentGroupManager = null;
        LoginManagementSettings _LoginManagementSettings = null;
        IHttpClientFactory _HttpClientFactory;

        public AccountService(CurrentUser currentUser,
            IUserSegmentGroupManager userSegmentGroupManager,
            IOptions<LoginManagementSettings> loginManagementSettingOptions,
            IHttpClientFactory httpClientFactory)
        {
            _CurrentUser = currentUser;
            _UserSegmentGroupManager = userSegmentGroupManager;
            _LoginManagementSettings = loginManagementSettingOptions.Value;
            _HttpClientFactory = httpClientFactory;
        }

        public async Task<WebResponse<List<UserDTO>>> GetUsers(string authorizationToken, string origin)
        {
            try
            {
                var client = _HttpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", authorizationToken);
                client.DefaultRequestHeaders.Add("Origin", origin);

                var url = _LoginManagementSettings.Url + "/api/user/get-list-by-tenantDomainPlatformMapGuid/";
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return new WebResponse<List<UserDTO>>()
                    {
                        Message = responseContent,
                        Payload = null,
                        Success = false
                    };
                }

                var responseText = await response.Content.ReadAsStringAsync();

                var items = JsonConvert.DeserializeObject<List<UserDTO>>(responseText);

                return new WebResponse<List<UserDTO>>(items);
            }
            catch (Exception ex)
            {
                return new WebResponse<List<UserDTO>>(ex);
            }
        }
    }
}
