using System;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Options;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.ApiKey
{
    public class GenericManagementAuth<T> where T : ApiSettings, new()
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expire { get; set; }
        private readonly LoginManagementSettings _loginManagementSettings;
        private readonly T _genericManagementSettings;
        private readonly IHttpClientFactory _httpClientFactory;


        public GenericManagementAuth(IOptions<LoginManagementSettings> loginManagementSettings, IOptions<T> genericManagementSettings,
            IHttpClientFactory httpClientFactory)
        {
            _genericManagementSettings = genericManagementSettings.Value;
            _loginManagementSettings = loginManagementSettings.Value;
            _httpClientFactory = httpClientFactory;
            GenerateNewToken();
        }
        private class TokenResponse
        {
            public string AccessToken { get; set; }
            public string RefreshToken { get; set; }
            public DateTime Expires { get; set; }

        }
        public void GenerateNewToken()
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                apiKey = _genericManagementSettings.ApiKey,
                platformGuid = _genericManagementSettings.PlatformGuid
            });
            var client = _httpClientFactory.CreateClient(typeof(LoginManagementSettings).Name);
            var httpContent = new StringContent(body, Encoding.UTF8, "application/json");
            var serviceResponse = client.PostAsync($"{_loginManagementSettings.Url}{_loginManagementSettings.Action}", httpContent).ConfigureAwait(false).GetAwaiter().GetResult();
            if (serviceResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"API KEY TOKEN ERROR - {_loginManagementSettings.Url}{_loginManagementSettings.Action}");
            }

            var content = serviceResponse.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            var token = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(content);
            Expire = token.Expires;
            RefreshToken = token.RefreshToken;
            Token = token.AccessToken;
        }
    }
}
