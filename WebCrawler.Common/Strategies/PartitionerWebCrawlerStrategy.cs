#nullable enable
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler.Common.Interfaces;

namespace WebCrawler.Common.Strategies
{
    public sealed class PartitionerWebCrawlerStrategy : WebCrawlerStrategy
    {
        public PartitionerWebCrawlerStrategy(IHttpClientFactory httpClientFactory, AppOptions options,
            IResultSaver resultSaver) :
            base(httpClientFactory, options, resultSaver)
        {
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(
                    Partitioner
                        .Create(GetUris())
                        .GetPartitions(MaxDegreeOfParallelism)
                        .Select(partition => Task.Run(async () =>
                        {
                            using var enumerator = partition;

                            while (partition.MoveNext())
                            {
                                cancellationToken.ThrowIfCancellationRequested();

                                var requestResult = await RequestResultAsync(partition.Current, cancellationToken)
                                    .ConfigureAwait(false);

                                await SaveResultAsync(requestResult, cancellationToken)
                                    .ConfigureAwait(false);

                                ReportProgress(partition.Current);
                            }
                        }, cancellationToken))
                )
                .ConfigureAwait(false);
        }
    }
}