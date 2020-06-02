using System;
using System.Net.Http;
using System.Threading;

namespace WebCrawler.Common
{
    public class HttpClientFactoryWithTimeoutHandle : IHttpClientFactory
    {
        private readonly long _maxResponseContentBufferSize = 1024 * 1024 * 100; // 100Mb
        private readonly TimeSpan _clientTimeout = TimeSpan.FromMilliseconds(5000); // 5 sec

        public HttpClient CreateClient(string name)
        {
            return new HttpClient(new TimeoutHandler
                {DefaultTimeout = _clientTimeout, InnerHandler = new HttpClientHandler()})
            {
                Timeout = Timeout.InfiniteTimeSpan,
                MaxResponseContentBufferSize = _maxResponseContentBufferSize
            };
        }
    }
}