using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler.Common.Interfaces;

namespace WebCrawler.Common
{
    public class TextFileResultSaver : IResultSaver
    {
        private readonly string _outputFileName;
        private readonly IResultFormatter _resultFormatter;

        public TextFileResultSaver(string outputFileName, IResultFormatter resultFormatter)
        {
            _outputFileName = outputFileName;
            _resultFormatter = resultFormatter;
        }

        public async Task SaveAsync(RequestResult requestResult, CancellationToken token) =>
            await File
                .AppendAllTextAsync(_outputFileName, _resultFormatter.GetFormattedResult(requestResult), token)
                .ConfigureAwait(false);
    }
}