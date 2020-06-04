using WebCrawler.Common.Interfaces;

namespace WebCrawler.Common
{
    public class SimpleResultFormatter : IResultFormatter
    {
        public string GetFormattedResult(RequestResult requestResult) => 
            $"{requestResult?.Uri}: {requestResult?.MediaType}, {requestResult?.ContentLength ?? 0}\n";
    }
}