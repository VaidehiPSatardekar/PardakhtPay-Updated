using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.GenericManagementApi;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.User;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Services.GenericManagementApi
{
    public class SystemFunctions<T> : ISystemFunctions<T> where T : ApiSettings, new()
    {
        private Dictionary<string, string> httpHeader;
        private readonly T settings;
        private HttpClient _httpClient;
        private IHttpClientFactory _httpClientFactory;
        public SystemFunctions(IOptions<T> settings, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            this.settings = settings.Value;
            if (this.settings.Url.EndsWith("/"))
            {
                this.settings.Url = this.settings.Url.Substring(0, this.settings.Url.Length - 1);
            }
        }


        public SystemFunctions(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        private string GenerateToken()
        {
            var userData = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                PlatformGuid = settings.PlatformGuid,
            });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(settings.JwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.UserData, userData)
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task<HttpResponseMessage> MakeRequest(object request, string url, HttpMethod httpMethod, Dictionary<string, string> dictionary = null)
        {
           
            var token = GenerateToken();
            httpHeader = new Dictionary<string, string>
            {
                    { "Authorization", $"Bearer {token}" }
            };

            if (dictionary != null)
            {
                foreach (KeyValuePair<string, string> entry in dictionary)
                {
                    if (!httpHeader.ContainsKey(entry.Key))
                    {
                        httpHeader.Add(entry.Key, entry.Value);
                    }
                }
            }

            if (_httpClient == null)
                _httpClient = _httpClientFactory.CreateClient(typeof(T).Name);

            HttpResponseMessage response = null;
            foreach (var header in httpHeader)
            {
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }

            if (url.StartsWith("/"))
            {
                url = url.Substring(1);
            }
            switch (httpMethod.Method.ToLower())
            {
                case "get":
                    {
                        response = await _httpClient.GetAsync($"{settings.Url}/{url}");
                        break;
                    }
                case "delete":
                    {
                        response = await _httpClient.DeleteAsync($"{settings.Url}/{url}");
                        break;
                    }
                case "post":
                    {
                        if (request != null && request is IFormFile file)
                        {
                            var fileStream = file.OpenReadStream();
                            var streamContent = new StreamContent(fileStream);
                            streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                            {
                                Name = "\"file\"",
                                FileName = "\"" + file.FileName + "\""
                            };

                            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            response = await _httpClient.PostAsync($"{settings.Url}/{url}", streamContent);

                        }
                        else
                        {
                            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                            response = await _httpClient.PostAsync($"{settings.Url}/{url}", content);
                        }
                        break;
                    }
                case "put":
                    {
                        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                        response = await _httpClient.PutAsync($"{settings.Url}/{url}", content);
                        break;
                    }

            }

            return response;
        }

        public string GenerateJwtToken()
        {
            return GenerateToken();
        }

        public string GenerateJwtToken(string platformGuid, string jwtKey)
        {
            var userData = JsonConvert.SerializeObject(new
            {
                PlatformGuid = platformGuid,
                UserType = (int)UserType.ApiUser
            });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.UserData, userData)
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
