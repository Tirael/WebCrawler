using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using WebCrawler.Common.Interfaces;

namespace WebCrawler.Common.Strategies
{
    public sealed class TasksDataflowWebCrawlerStrategy : WebCrawlerStrategy
    {
        private readonly ExecutionDataflowBlockOptions _dataFlowBlockOptions;
        private readonly DataflowLinkOptions _linkOptions;

        public TasksDataflowWebCrawlerStrategy(IHttpClientFactory httpClientFactory, AppOptions options,
            IResultSaver resultSaver) :
            base(httpClientFactory, options, resultSaver)
        {
            _dataFlowBlockOptions = new ExecutionDataflowBlockOptions
                {MaxDegreeOfParallelism = MaxDegreeOfParallelism};
            _linkOptions = new DataflowLinkOptions {PropagateCompletion = true};
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var requestExecutor = new TransformBlock<Uri, RequestResult>(
                async uri => await RequestResultAsync(uri, cancellationToken)
                    .ConfigureAwait(false),
                _dataFlowBlockOptions);

            var resultSaver = new ActionBlock<RequestResult>(async requestResult =>
            {
                await SaveResultAsync(requestResult, cancellationToken)
                    .ConfigureAwait(false);

                ReportProgress(requestResult.Uri);
            });

            requestExecutor.LinkTo(resultSaver, _linkOptions);

            foreach (var uri in GetUris())
            {
                cancellationToken.ThrowIfCancellationRequested();

                requestExecutor.Post(uri);
            }

            requestExecutor.Complete();

            await resultSaver.Completion.ConfigureAwait(false);
        }
    }
}