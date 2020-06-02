using System.Threading;
using System.Threading.Tasks;

namespace WebCrawler.Common.Interfaces
{
    public interface IApp
    {
        Task Run(CancellationToken cancellationToken);
    }
}