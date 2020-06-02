using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace WebCrawler.Tests.Extensions
{
    internal static class MockHttpClientHandlerExtensions
    {
        public static void SetupSendAsync(this Mock<HttpClientHandler> mockHandler, Uri requestUri,
            string response, string mediaType)
        {
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(message => message.RequestUri == requestUri),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                    {Content = new StringContent(response, Encoding.UTF8, mediaType)}));
        }
    }
}