using System.Threading;
using System.Threading.Tasks;
using WebCrawler.Common.Interfaces;
using WebCrawler.Common.Strategies;

namespace WebCrawler.Common
{
    public class App : IApp
    {
        private readonly WebCrawlerStrategy _webCrawlerStrategy;

        public App(WebCrawlerStrategy webCrawlerStrategy) => _webCrawlerStrategy = webCrawlerStrategy;

        public async Task Run(CancellationToken cancellationToken) =>
            await _webCrawlerStrategy
                .ExecuteAsync(cancellationToken)
                .ConfigureAwait(false);
    }
}