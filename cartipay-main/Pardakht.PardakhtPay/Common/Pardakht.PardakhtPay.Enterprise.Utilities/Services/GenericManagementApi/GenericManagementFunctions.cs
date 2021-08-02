using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.ApiKey;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.GenericManagementApi;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Services.GenericManagementApi
{
    public class GenericManagementFunctions<T> : ControllerBase, IGenericManagementFunctions<T> where T : ApiSettings, new()
    {
        private readonly T _genericApiSettings;
        private IHttpClientFactory _httpClientFactory;
        private readonly GenericManagementTokenGenerator<T> _genericManagementTokenGenerator;
        private const string UserData = "UserData";
        private Dictionary<string, string> httpHeader;
        public GenericManagementFunctions(GenericManagementTokenGenerator<T> genericManagementTokenGenerator, IHttpClientFactory httpClientFactory, IOptions<T> genericManagementSettings)
        {
            if (string.IsNullOrEmpty(genericManagementTokenGenerator.Token))
                throw new Exception($"API KEY TOKEN ERROR {nameof(T)}");

            _httpClientFactory = httpClientFactory;
            _genericApiSettings = genericManagementSettings.Value;
            _genericManagementTokenGenerator = genericManagementTokenGenerator;

            httpHeader = new Dictionary<string, string>
            {
                    { "Authorization", $"Bearer {_genericManagementTokenGenerator.Token}" }
            };
        }

        public async Task<IActionResult> GenericRequest(object request, ClaimsPrincipal user, HttpRequest httpRequest, Dictionary<string, string> dictionary = null)
        {
            var headers = new string[] {UserData, "TimeZoneId", "locale", "Account-Context", "user-agent", "X-Forwarded-For", "user-identity" };

            foreach (var header in headers)
            {
                if (Helper.HttpHeaderHelper.ContainsKey(httpRequest.Headers, header))
                {
                    httpHeader.Add(header, httpRequest.Headers[header]);
                }
            }

            try
            {
                var url = httpRequest.Path.Value;
                if (httpRequest.QueryString.HasValue)
                {
                    url = url + $"{httpRequest.QueryString.Value}";
                }
                var response = await MakeRequest(request, user, url, new HttpMethod(httpRequest.Method), dictionary);
                var content = await response.Content.ReadAsStringAsync();
                var objectContent = JsonConvert.DeserializeObject<dynamic>(content);
                return StatusCode((int)response.StatusCode, objectContent);

            }
            catch (Exception ex)
            {
                throw new Exception($"GenericRequest Exception URL : {_genericApiSettings.Url}/{httpRequest.Path.Value} Exception : {ex.Message}");
            }
        }

        public async Task<HttpResponseMessage> MakeRequest(object request, ClaimsPrincipal user, string url, HttpMethod httpMethod, Dictionary<string, string> dictionary = null)
        {
            var client = _httpClientFactory.CreateClient(typeof(T).Name);
            if (dictionary != null)
            {
                foreach (KeyValuePair<string, string> entry in dictionary)
                {
                    var hasKey = httpHeader.Keys.Any(q => q.Equals(entry.Key, StringComparison.OrdinalIgnoreCase));
                    if (!hasKey)
                    {
                        try
                        {
                            httpHeader.Add(entry.Key, entry.Value);
                        }
                        catch
                        {

                        }
                    }
                }
            }

            if (user?.Claims != null)
            {
                var userData = user.Claims.FirstOrDefault(p => p.Type == ClaimTypes.UserData)?.Value;
                var hasUserData = httpHeader.Keys.Any(q => q.Equals(UserData, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(userData) && !hasUserData)
                {
                    httpHeader.Add(UserData, JsonConvert.SerializeObject(userData));
                }

                var userName = user.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value;
                if (!string.IsNullOrEmpty(userName))
                {
                    httpHeader.Add("UserName", userName);
                }

                var userInfo = user.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userInfo))
                {
                    httpHeader.Add("NameIdentifier", userInfo);
                }

                var roles = user.Claims.Where(p => p.Type == ClaimTypes.Role).Select(p => p.Value).ToArray();
                if (roles.Length > 0)
                {
                    httpHeader.Add("Roles", JsonConvert.SerializeObject(roles));
                }
            }


            _genericApiSettings.Url = _genericApiSettings.Url.TrimEnd('/');
            url = url.TrimStart('/');
            var apiUrl = $"{_genericApiSettings.Url}/{url}";
            HttpResponseMessage response = null;

            foreach (var header in httpHeader)
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }
            switch (httpMethod.Method.ToLower())
            {
                case "get":
                    {
                        response = await client.GetAsync(apiUrl);
                        break;
                    }
                case "delete":
                    {
                        response = await client.DeleteAsync(apiUrl);
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

                            string boundary = Guid.NewGuid().ToString();
                            var content = new MultipartFormDataContent(boundary);
                            content.Headers.Remove("Content-Type");
                            content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);
                            content.Add(streamContent);

                            response = await client.PostAsync(apiUrl, content);

                        }
                        else if (request != null && request is IFormCollection formCollection)
                        {
                            response = await PostFormCollectionAsync(apiUrl, httpHeader, formCollection);
                        }
                        else
                        {
                            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                            response = await client.PostAsync(apiUrl, content);
                        }
                        break;
                    }
                case "put":
                    {
                        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                        response = await client.PutAsync(apiUrl, content);
                        break;
                    }
                case "patch":
                    {
                        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                        response = await client.PatchAsync(apiUrl, content);
                        break;
                    }
            }

            return response;
        }

        private async Task<HttpResponseMessage> PostFormCollectionAsync(string url, Dictionary<string, string> httpHeader, IFormCollection formCollection)
        {
            var streamContents = new Dictionary<string, StreamContent>();
            var stringContents = new Dictionary<string, StringContent>();
            if (formCollection != null)
            {
                if (formCollection.Files != null)
                {
                    foreach (var file in formCollection.Files)
                    {
                        var fileStream = file.OpenReadStream();
                        var streamContent = new StreamContent(fileStream);
                        streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "\"" + file.Name + "\"",
                            FileName = "\"" + file.FileName + "\""
                        };

                        streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                        streamContents.Add(file.Name, streamContent);
                    }
                }

                foreach (var key in formCollection.Keys)
                {
                    if (formCollection.TryGetValue(key, out StringValues value))
                    {
                        var val = value.FirstOrDefault();
                        if (val != null && val != "null" && val != "undefined")
                        {
                            var stringContent = new StringContent(val);
                            stringContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                            {
                                Name = key
                            };
                            stringContents.Add(key, stringContent);
                        }
                    }
                }
            }

            var client = _httpClientFactory.CreateClient(typeof(T).Name);
            foreach (var header in httpHeader)
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }

            string boundary = Guid.NewGuid().ToString();
            var content = new MultipartFormDataContent(boundary);
            content.Headers.Remove("Content-Type");
            content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);

            if (streamContents != null)
            {
                foreach (var key in streamContents.Keys)
                {
                    content.Add(streamContents[key]);
                }
            }

            if (stringContents != null)
            {
                foreach (var key in stringContents.Keys)
                {
                    content.Add(stringContents[key]);
                }
            }

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.PostAsync(url, content);
        }
    }
}
