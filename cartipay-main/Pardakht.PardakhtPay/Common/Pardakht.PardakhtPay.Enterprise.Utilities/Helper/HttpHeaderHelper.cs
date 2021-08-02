using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Helper
{
    public static class HttpHeaderHelper
    {
        public static KeyValuePair<string, StringValues> GetHeaderKeyValue(HttpRequest httpRequest, string key)
        {
            return httpRequest.Headers.FirstOrDefault(p => p.Key.Equals(key,StringComparison.OrdinalIgnoreCase));
        }

        public static bool ContainsKey(IHeaderDictionary httpHeaders, string key)
        {
            return httpHeaders.Keys.Any(q => q.Equals(key, StringComparison.OrdinalIgnoreCase));
        }
    }
}
