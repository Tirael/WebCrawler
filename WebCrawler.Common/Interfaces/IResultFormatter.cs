namespace WebCrawler.Common.Interfaces
{
    public interface IResultFormatter
    {
        string GetFormattedResult(RequestResult requestResult);
    }
}