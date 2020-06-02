namespace WebCrawler.Common
{
    public class RequestResult
    {
        public string Url { get; }
        public string MediaType { get; }
        public long? ContentLength { get; }

        public RequestResult(string url, string mediaType, long? contentLength)
        {
            Url = url;
            MediaType = mediaType;
            ContentLength = contentLength;
        }
    }
}