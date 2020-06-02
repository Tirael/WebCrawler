using System;
using System.Net.Http;

namespace WebCrawler.Common.Extensions
{
    public static class HttpRequestExtensions
    {
        private const string TimeoutPropertyKey = "RequestTimeout";

        public static void SetTimeout(this HttpRequestMessage request, TimeSpan? timeout)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            request.Properties[TimeoutPropertyKey] = timeout;
        }

        public static TimeSpan? GetTimeout(this HttpRequestMessage request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            return request.Properties.TryGetValue(TimeoutPropertyKey, out var value) && value is TimeSpan timeout
                ? (TimeSpan?) timeout
                : null;
        }
    }
}