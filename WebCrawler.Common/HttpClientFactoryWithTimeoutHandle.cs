using System;
using System.Net.Http;
using System.Threading;

namespace WebCrawler.Common
{
    public sealed class HttpClientFactoryWithTimeoutHandle : IHttpClientFactory, IDisposable
    {
        private readonly long _maxResponseContentBufferSize = 1024 * 1024 * 100; // 100Mb
        private readonly TimeSpan _clientTimeout = TimeSpan.FromMilliseconds(5000); // 5 sec
        private TimeoutHandler _timeoutHandler;

        public HttpClient CreateClient(string name)
        {
            _timeoutHandler = new TimeoutHandler
                {DefaultTimeout = _clientTimeout, InnerHandler = new HttpClientHandler()};

            return new HttpClient(_timeoutHandler)
            {
                Timeout = Timeout.InfiniteTimeSpan,
                MaxResponseContentBufferSize = _maxResponseContentBufferSize
            };
        }

        #region IDisposable

        private const int DisposedFlag = 1;

        #region IsDisposed

        private int _isDisposed;

        public bool IsDisposed
        {
            get
            {
                Thread.MemoryBarrier();
                return _isDisposed == DisposedFlag;
            }
        }

        #endregion

        public void Dispose()
        {
            var wasDisposed = Interlocked.Exchange(ref _isDisposed, DisposedFlag);

            if (wasDisposed == DisposedFlag)
                return;

            _timeoutHandler?.Dispose();

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}