using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler.Common.Interfaces;

namespace WebCrawler.Common.Strategies
{
    public sealed class ParallelLinqWebCrawlerStrategy : WebCrawlerStrategy
    {
        public ParallelLinqWebCrawlerStrategy(IHttpClientFactory httpClientFactory, AppOptions options,
            IResultSaver resultSaver) :
            base(httpClientFactory, options, resultSaver)
        {
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var requestResults = GetUris()
                .AsParallel()
                .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .WithDegreeOfParallelism(MaxDegreeOfParallelism)
                .Select(async uri => await RequestResultAsync(uri, cancellationToken)
                    .ConfigureAwait(false));

            await Task.Run(() => requestResults.ForAll(async task =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await SaveResultAsync(task.Result, cancellationToken)
                        .ConfigureAwait(false);

                    ReportProgress(task.Result.Uri);
                }), cancellationToken)
                .ConfigureAwait(false);
        }
    }
}