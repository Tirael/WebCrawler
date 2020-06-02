using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WebCrawler.Common.Interfaces;

namespace WebCrawler.Common
{
    public class App : IApp
    {
        private readonly AppOptions _options;
        private readonly IResultSaver _resultSaver;
        private readonly ExecutionDataflowBlockOptions _dataFlowBlockOptions;
        private readonly DataflowLinkOptions _linkOptions;
        private readonly IHttpClientFactory _httpClientFactory;

        public App(IHttpClientFactory httpClientFactory, AppOptions options, IResultSaver resultSaver)
        {
            _options = options;
            _resultSaver = resultSaver;
            _httpClientFactory = httpClientFactory;

            _dataFlowBlockOptions = new ExecutionDataflowBlockOptions
                {MaxDegreeOfParallelism = _options.MaxDegreeOfParallelism};
            _linkOptions = new DataflowLinkOptions {PropagateCompletion = true};
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            var requestExecutor = new TransformBlock<string, RequestResult>(
                async url =>
                {
                    using var client = _httpClientFactory.CreateClient();
                    
                    return await ExecuteRequestAsync(client, url, cancellationToken);
                }, _dataFlowBlockOptions);

            var resultSaver = new ActionBlock<RequestResult>(async requestResult =>
                await _resultSaver.SaveAsync(requestResult, cancellationToken));

            requestExecutor.LinkTo(resultSaver, _linkOptions);

            _options.Urls.ForEach(url => requestExecutor.Post(url));

            requestExecutor.Complete();

            await resultSaver.Completion;
        }

        private static async Task<RequestResult> ExecuteRequestAsync(HttpClient client, string url, CancellationToken token)
        {
            MediaTypeHeaderValue contentType;
            long? contentLength;

            try
            {
                using var response = await client.GetAsync(url, token);

                response.EnsureSuccessStatusCode();

                contentType = response.Content.Headers.ContentType;
                contentLength = response.Content.Headers.ContentLength;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return new RequestResult(url, contentType.MediaType, contentLength);
        }
    }
}