using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler.Common.Interfaces;

namespace WebCrawler.Common.Strategies
{
    public abstract class WebCrawlerStrategy
    {
        private readonly AppOptions _options;
        private readonly IResultSaver _resultSaver;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IProgress<string> _progress =
            new Progress<string>(url => Console.WriteLine($"Completed: {url}"));

        protected WebCrawlerStrategy(IHttpClientFactory httpClientFactory, AppOptions options, IResultSaver resultSaver)
        {
            _httpClientFactory = httpClientFactory;
            _options = options;
            _resultSaver = resultSaver;
        }

        protected IEnumerable<Uri> GetUris()
        {
            foreach (var url in _options.Urls)
                yield return new Uri(url);
        }

        protected int MaxDegreeOfParallelism => _options.MaxDegreeOfParallelism;

        protected void ReportProgress(Uri url) => _progress.Report(url?.AbsoluteUri);

        public abstract Task ExecuteAsync(CancellationToken cancellationToken);
        
        protected async Task<RequestResult> RequestResultAsync(Uri uri, CancellationToken cancellationToken)
        {
            using var client = _httpClientFactory.CreateClient();
            return await ExecuteRequestAsync(client, uri, cancellationToken)
                .ConfigureAwait(false);
        }

        protected async Task SaveResultAsync(RequestResult requestResult, CancellationToken cancellationToken) => 
            await _resultSaver.SaveAsync(requestResult, cancellationToken).ConfigureAwait(false);

        private static async Task<RequestResult> ExecuteRequestAsync(HttpClient client, Uri uri,
            CancellationToken token)
        {
            MediaTypeHeaderValue contentType;
            long? contentLength;

            try
            {
                using var response = await client
                    .GetAsync(uri, token)
                    .ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                contentType = response.Content.Headers.ContentType;
                contentLength = response.Content.Headers.ContentLength;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return new RequestResult(uri, contentType.MediaType, contentLength);
        }
    }
}