using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.Helpers
{
    public class HttpContextHelper
    {
        public static string GetSubDomain(HttpContext httpContext)
        {
            var subDomain = string.Empty;

            var origin = httpContext.Request.Headers.Where(w => w.Key.ToLower() == "origin").FirstOrDefault();

            if (origin.Key != null)
            {
                var uri = new Uri(origin.Value);
                var index = uri.Host.IndexOf('.');
                if (index != -1)
                {
                    subDomain = uri.Host.Substring(0, index);
                }
            }

            return subDomain;
        }

        public static string GetDomain(HttpContext httpContext)
        {
            var domain = string.Empty;

            var origin = httpContext.Request.Headers.Where(w => w.Key.ToLower() == "origin").FirstOrDefault();

            if (origin.Key != null)
            {
                var uri = new Uri(origin.Value);
                domain = uri.Host;
                //domain = uri.Host.Replace("www.", string.Empty, StringComparison.InvariantCultureIgnoreCase);
            }

            return domain;
        }

        public static string GetDomainWithoutSubDomain(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return domain;
            }

            var domainTexts = domain
                .Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            if(domainTexts.Length <= 2)
            {
                return domain;
            }

            return string.Join(".", domainTexts
                .Skip(domainTexts.Length - 2));
        }

        public static bool IsSameOriginRequest(HttpContext httpContext)
        {
            var hostDomain = string.Empty;
            var originDomain = GetDomain(httpContext);
            var hostHeader = httpContext.Request.Headers.Where(w => w.Key.ToLower() == "host").FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(originDomain) && hostHeader.Key != null)
            {
                // remove port if reqd
                var index = hostHeader.Value.ToString().IndexOf(":");
                if (index != -1)
                {
                    hostDomain = hostHeader.Value.ToString().Substring(0, index);
                }

                return (originDomain == hostDomain);
            }
            return true; //default to true
        }

        public static string GetOriginBaseUri(HttpContext httpContext)
        {
            var originUri = string.Empty;
            var origin = httpContext.Request.Headers.Where(w => w.Key.ToLower() == "origin").FirstOrDefault();

            if (origin.Key != null)
            {
                return origin.Value;
            }

            return originUri;
        }

        public static string GetCustomHeader(HttpContext httpContext, string headerName)
        {
            var header = httpContext.Request.Headers.Where(w => w.Key.ToLower() == headerName.ToLower()).FirstOrDefault();

            if (header.Key != null)
            {
                return header.Value;
            }

            return string.Empty;
        }

        public static string CopyPortToNewUrl(string originUrl, string destinationUrl)
        {
            // extract the port info (if any) and append to destination (if it doesn't already have one)
            var exclusions = new int[] { 80, 443 };
            // does the destination already have a port?
            var destUri = new Uri(destinationUrl);
            if (destUri.Port > 0 && !exclusions.Contains(destUri.Port))
            {
                return destinationUrl;
            }

            var originUri = new Uri(originUrl);
            if (originUri.Port > 0 && !exclusions.Contains(originUri.Port))
            {
                destinationUrl += $":{originUri.Port.ToString()}";
            }

            return destinationUrl;
        }
    }
}
