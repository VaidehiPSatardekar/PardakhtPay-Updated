//using Microsoft.Extensions.Options;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.ApiKey;
//using Pardakht.PardakhtPay.Shared.Interfaces;
//using Pardakht.PardakhtPay.Shared.Models;
//using Pardakht.PardakhtPay.Shared.Models.Configuration;
//using Pardakht.PardakhtPay.Shared.Models.WebService;

//namespace Pardakht.PardakhtPay.Shared.Services
//{
//    public class PardakhtPayTimeZoneService : ITimeZoneService
//    {
//        TimeZoneConfiguration _TimeZoneConfiguration = null;
//        //string _Token = string.Empty;
//        static object _LockObject = new object();
//        IHttpClientFactory _HttpClientFactory = null;
//        GenericManagementTokenGenerator<TimeZoneConfiguration> _TokenGenerator = null;

//        public PardakhtPayTimeZoneService(IOptions<TimeZoneConfiguration> timeZoneOptions,
//            IHttpClientFactory httpClientFactory,
//            GenericManagementTokenGenerator<TimeZoneConfiguration> tokenGenerator)
//        {
//            _TimeZoneConfiguration = timeZoneOptions.Value;
//            _HttpClientFactory = httpClientFactory;
//            _TokenGenerator = tokenGenerator;

//            //try
//            //{
//            //    var task = GetToken();

//            //    task.Wait();
//            //}
//            //catch
//            //{

//            //}
//        }

//        public DateTime Convert(DateTime date, TimeZoneInfo info)
//        {
//            return TimeZoneInfo.ConvertTimeFromUtc(date, info);
//        }

//        public async Task<List<DateTime>> ConvertCalendar(List<DateTime> dates, string fromCode, string toCode)
//        {
//            if (toCode == Helper.Utc)
//            {
//                if (string.IsNullOrEmpty(fromCode) || fromCode == Helper.Utc)
//                {
//                    return dates;
//                }
//            }

//            if (fromCode == toCode)
//            {
//                return dates;
//            }

//            //if (string.IsNullOrEmpty(_Token))
//            //{
//            //    await GetToken();
//            //}

//            var client = _HttpClientFactory.CreateClient();

//            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _TokenGenerator.Token);

//            string url = _TimeZoneConfiguration.Url + "/api/timeconversion/get-date-time";
//            var str = JsonConvert.SerializeObject(new TimeZoneRequest()
//            {
//                fromAreaCode = fromCode,
//                toAreaCode = toCode,
//                DateTime = DateTime.Today,
//                DateTimeList = dates
//            });

//            using (var response = await client.PostAsync(url, new StringContent(str, Encoding.UTF8, "application/json")))
//            {
//                if (response.IsSuccessStatusCode)
//                {
//                    var content = await response.Content.ReadAsStringAsync();

//                    var tokenResponse = JsonConvert.DeserializeObject<TimeZoneRequest>(content);

//                    return tokenResponse.DateTimeList;
//                }
//                else
//                {
//                    var content = await response.Content.ReadAsStringAsync();

//                    throw new Exception(content);
//                }
//            }
//        }

//        public async Task<DateTime> ConvertCalendar(DateTime date, string fromCode, string toCode)
//        {
//            if (toCode == Helper.Utc)
//            {
//                if (string.IsNullOrEmpty(fromCode) || fromCode == Helper.Utc)
//                {
//                    return date;
//                }
//            }

//            if (fromCode == toCode)
//            {
//                return date;
//            }

//            var client = _HttpClientFactory.CreateClient();
//            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _TokenGenerator.Token);

//            string url = _TimeZoneConfiguration.Url + "/api/timeconversion/get-date-time";
//            var str = JsonConvert.SerializeObject(new TimeZoneRequest()
//            {
//                fromAreaCode = fromCode,
//                toAreaCode = toCode,
//                DateTime = new DateTime(date.Ticks, DateTimeKind.Unspecified),
//                DateTimeList = null
//            });
//            using (var response = await client.PostAsync(url, new StringContent(str, Encoding.UTF8, "application/json")))
//            {

//                if (response.IsSuccessStatusCode)
//                {
//                    var content = await response.Content.ReadAsStringAsync();

//                    var tokenResponse = JsonConvert.DeserializeObject<TimeZoneRequest>(content);

//                    return tokenResponse.DateTime.Value;
//                }
//                else
//                {
//                    var content = await response.Content.ReadAsStringAsync();

//                    throw new Exception(content);
//                }
//            }
//        }

//        public async Task<List<string>> ConvertCalendarLocal(List<DateTime> dates, string fromCode, string toCode)
//        {
//            if (toCode == Helper.Utc)
//            {
//                if (string.IsNullOrEmpty(fromCode) || fromCode == Helper.Utc)
//                {
//                    return dates.ConvertAll(t => t.ToString("o", CultureInfo.InvariantCulture));
//                }
//            }

//            if (fromCode == toCode)
//            {
//                return dates.ConvertAll(t => t.ToString("o", CultureInfo.InvariantCulture));
//            }

//            var client = _HttpClientFactory.CreateClient();

//            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _TokenGenerator.Token);

//            string url = _TimeZoneConfiguration.Url + "/api/timeconversion/get-date-time-local";
//            var str = JsonConvert.SerializeObject(new TimeZoneRequest()
//            {
//                fromAreaCode = fromCode,
//                toAreaCode = toCode,
//                DateTime = DateTime.Today,
//                DateTimeList = dates
//            });

//            using (var response = await client.PostAsync(url, new StringContent(str, Encoding.UTF8, "application/json")))
//            {
//                if (response.IsSuccessStatusCode)
//                {
//                    var content = await response.Content.ReadAsStringAsync();

//                    var dateResponse = JsonConvert.DeserializeObject<TimeZoneLocalResponse>(content);

//                    return dateResponse.DateTimeList;
//                }
//                else
//                {
//                    var content = await response.Content.ReadAsStringAsync();

//                    throw new Exception(content);
//                }
//            }
//        }
//    }
//}
