using System.Net.Http;
using Moq;

namespace WebCrawler.Tests
{
    public class FakeHttpClientFactory : IHttpClientFactory
    {
        private readonly Mock<HttpClientHandler> _mockHandler;

        public FakeHttpClientFactory(Mock<HttpClientHandler> mockHandler) => _mockHandler = mockHandler;

        public HttpClient CreateClient(string name) => new HttpClient(_mockHandler.Object);
    }
}