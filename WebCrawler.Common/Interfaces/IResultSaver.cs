using System.Threading;
using System.Threading.Tasks;

namespace WebCrawler.Common.Interfaces
{
    public interface IResultSaver
    {
        Task SaveAsync(RequestResult requestResult, CancellationToken token);
    }
}