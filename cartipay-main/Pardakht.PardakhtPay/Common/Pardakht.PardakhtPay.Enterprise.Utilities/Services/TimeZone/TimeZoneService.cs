using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.Helpers;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.GenericManagementApi;
using Pardakht.PardakhtPay.Enterprise.Utilities.Interfaces.TimeZone;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.Settings;
using Pardakht.PardakhtPay.Enterprise.Utilities.Models.TimeZone;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Services.TimeZone
{
    public class TimeZoneService : ITimeZoneService
    { 
        private readonly IServiceProvider serviceProvider;

        public TimeZoneService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<DateTime> ConvertCalendar(DateTime date, string fromCode, string toCode)
        {
            var body = new TimeZoneRequest
            {
                fromAreaCode = fromCode,
                toAreaCode = toCode,
                DateTime = new DateTime(date.Ticks, DateTimeKind.Unspecified),
            };
            HttpResponseMessage httpResponseMessage;
            var managementFunctions = serviceProvider.GetRequiredService<IGenericManagementFunctions<TimeZoneCalendarManagementSettings>>();
            httpResponseMessage = await managementFunctions.MakeRequest(body, null, $"/api/timeconversion/get-date-time", System.Net.Http.HttpMethod.Post);

            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage == null || !httpResponseMessage.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }
            var tokenResponse = JsonConvert.DeserializeObject<TimeZoneRequest>(content);
            return tokenResponse.DateTime.Value;
        }

        public async Task<DateTime> ConvertCalendar(DateTime date, string fromCode, string toCode, string jwtToken)
        {
            var settings = serviceProvider.GetRequiredService<IOptions<TimeZoneCalendarManagementSettings>>();
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient(typeof(TimeZoneCalendarManagementSettings).Name);
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new TimeZoneRequest
            {
                fromAreaCode = fromCode,
                toAreaCode = toCode,
                DateTime = new DateTime(date.Ticks, DateTimeKind.Unspecified),
            });

            var url = $"{settings.Value.Url}/api/timeconversion/get-date-time";
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
            var httpContent = new StringContent(body, Encoding.UTF8, "application/json");
            var httpResponseMessage = await client.PostAsync(url, httpContent);
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage == null || !httpResponseMessage.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }
            var tokenResponse = JsonConvert.DeserializeObject<TimeZoneRequest>(content);
            return tokenResponse.DateTime.Value;

        }

        public async Task<List<string>> ConvertCalendarLocal(List<DateTime> dates, string fromCode, string toCode)
        {
            if (toCode == TimeZoneExtension.Utc)
            {
                if (string.IsNullOrEmpty(fromCode) || fromCode == TimeZoneExtension.Utc)
                {
                    return dates.ConvertAll(t => t.ToString("o", CultureInfo.InvariantCulture));
                }
            }

            if (fromCode == toCode)
            {
                return dates.ConvertAll(t => t.ToString("o", CultureInfo.InvariantCulture));
            }

            var body = new TimeZoneRequest
            {
                fromAreaCode = fromCode,
                toAreaCode = toCode,
                DateTime = DateTime.Today,
                DateTimeList = dates
            };
            HttpResponseMessage httpResponseMessage;
            var managementFunctions = serviceProvider.GetRequiredService<IGenericManagementFunctions<TimeZoneCalendarManagementSettings>>();
            httpResponseMessage = await managementFunctions.MakeRequest(body, null, $"/api/timeconversion/get-date-time-local", System.Net.Http.HttpMethod.Post);

            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage == null || !httpResponseMessage.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }

            var dateResponse = JsonConvert.DeserializeObject<TimeZoneLocalResponse>(content);
            return dateResponse.DateTimeList;
        }

        public async Task<List<string>> ConvertCalendarLocal(List<DateTime> dates, string fromCode, string toCode, string jwtToken)
        {
            var settings = serviceProvider.GetRequiredService<IOptions<TimeZoneCalendarManagementSettings>>();
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var client = httpClientFactory.CreateClient(typeof(TimeZoneCalendarManagementSettings).Name);

            if (toCode == TimeZoneExtension.Utc)
            {
                if (string.IsNullOrEmpty(fromCode) || fromCode == TimeZoneExtension.Utc)
                {
                    return dates.ConvertAll(t => t.ToString("o", CultureInfo.InvariantCulture));
                }
            }

            if (fromCode == toCode)
            {
                return dates.ConvertAll(t => t.ToString("o", CultureInfo.InvariantCulture));
            }

            var body = Newtonsoft.Json.JsonConvert.SerializeObject(new TimeZoneRequest
            {
                fromAreaCode = fromCode,
                toAreaCode = toCode,
                DateTime = DateTime.Today,
                DateTimeList = dates
            });

            var url = $"{settings.Value.Url}/api/timeconversion/get-date-time-local";
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {jwtToken}");
            var httpContent = new StringContent(body, Encoding.UTF8, "application/json");
            var httpResponseMessage = await client.PostAsync(url, httpContent);
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            if (httpResponseMessage == null || !httpResponseMessage.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }

            var dateResponse = JsonConvert.DeserializeObject<TimeZoneLocalResponse>(content);
            return dateResponse.DateTimeList;
        }
    }
}
