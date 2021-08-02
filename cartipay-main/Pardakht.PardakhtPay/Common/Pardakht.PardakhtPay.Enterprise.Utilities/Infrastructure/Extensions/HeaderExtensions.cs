//using System.Net.Http;
//using System.Net.Http.Headers;

//{
//    public static class HeaderExtensions
//    {
//        public static HttpClient GenerateHeaders(this HttpClient httpClient, HttpHeader httpHeader)
//        {
//            if (httpHeader == null)
//                return httpClient;

//            if (httpHeader.Headers != null)
//            {
//                foreach (var d in httpHeader.Headers)
//                {
//                    httpClient.DefaultRequestHeaders.Add(d.Key, d.Value);
//                }
//            }

//            if (httpHeader.AuthHeaders != null)
//            {
//                foreach (var a in httpHeader.AuthHeaders)
//                {
//                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(a.Key, a.Value);
//                }
//            }
//            return httpClient;
//        }
//    }
//}
