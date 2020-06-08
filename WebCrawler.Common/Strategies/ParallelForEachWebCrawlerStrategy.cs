using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler.Common.Interfaces;

namespace WebCrawler.Common.Strategies
{
    public sealed class ParallelForEachWebCrawlerStrategy : WebCrawlerStrategy
    {
        public ParallelForEachWebCrawlerStrategy(IHttpClientFactory httpClientFactory, AppOptions options,
            IResultSaver resultSaver) :
            base(httpClientFactory, options, resultSaver)
        {
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var pOptions = new ParallelOptions
                {MaxDegreeOfParallelism = MaxDegreeOfParallelism, CancellationToken = cancellationToken};

            await Task.Run(() =>
                {
                    Parallel.ForEach(GetUris(), pOptions, async (uri, state) =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var requestResult = await RequestResultAsync(uri, cancellationToken).ConfigureAwait(false);
                        
                        await SaveResultAsync(requestResult, cancellationToken).ConfigureAwait(false);

                        ReportProgress(uri);
                    });
                }, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}