using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler.Common.Interfaces;

namespace WebCrawler.Common.Strategies
{
    public sealed class AsyncEnumerableStrategy : WebCrawlerStrategy
    {
        public AsyncEnumerableStrategy(IHttpClientFactory httpClientFactory, AppOptions options,
            IResultSaver resultSaver) :
            base(httpClientFactory, options, resultSaver)
        {
        }

        public override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            using var throttler = new SemaphoreSlim(MaxDegreeOfParallelism);

            var urls = GetUris()
                .ToAsyncEnumerable()
                .WithCancellation(cancellationToken)
                .ConfigureAwait(false);

            var tasks = new List<Task>();

            await foreach (var url in urls)
            {
                tasks.Add(Task.Run(async () =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var localUrl = url;

                    await throttler.WaitAsync(cancellationToken).ConfigureAwait(false);

                    try
                    {
                        var requestResult = await RequestResultAsync(localUrl, cancellationToken)
                            .ConfigureAwait(false);

                        await SaveResultAsync(requestResult, cancellationToken)
                            .ConfigureAwait(false);

                        ReportProgress(requestResult.Uri);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.InnerException?.Message ?? e.Message);

                        throw;
                    }
                    finally
                    {
                        throttler.Release();
                    }
                }, cancellationToken));
            }

            await Task
                .WhenAll(tasks.ToArray())
                .ConfigureAwait(false);
        }
    }
}