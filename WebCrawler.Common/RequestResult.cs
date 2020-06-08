using System;

namespace WebCrawler.Common
{
    public class RequestResult
    {
        public Uri Uri { get; }
        public string MediaType { get; }
        public long? ContentLength { get; }

        public RequestResult(Uri uri, string mediaType, long? contentLength)
        {
            Uri = uri;
            MediaType = mediaType;
            ContentLength = contentLength;
        }
    }
}