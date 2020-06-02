using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using WebCrawler.Common;
using WebCrawler.Common.Interfaces;
using WebCrawler.Tests.Extensions;

namespace WebCrawler.Tests
{
    [TestFixture]
    public class AppTests
    {
        private Mock<HttpClientHandler> _mockHandler;

        [SetUp]
        public void Setup()
        {
            _mockHandler = new Mock<HttpClientHandler>();
        }
        
        [Test]
        public async Task When_AppExecuteRun_Then_SaveRequestResultIsCalled()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            
            var requestUrl = "http://google.com";
            
            var expectedResponse = "Test response";
            var expectedMediaType = "text/plain";
            var expectedLength = expectedResponse.Length;
            
            _mockHandler.SetupSendAsync(new Uri(requestUrl), expectedResponse, "text/plain");
            
            var httpClientFactory = new FakeHttpClientFactory(_mockHandler);
            var appOptions = new AppOptions(new List<string> {requestUrl}, 1, "test.txt");
            var resultSaverMock = new Mock<IResultSaver>();

            var targetRequestResult = new RequestResult(requestUrl, expectedMediaType, expectedLength);

            resultSaverMock
                .Setup(saver => 
                    saver.SaveAsync(It.Is<RequestResult>(result => RequestResultIsEquals(result, targetRequestResult)), 
                        It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(expectedResponse))
                .Verifiable();

            App app = new App(httpClientFactory, appOptions, resultSaverMock.Object);
            
            // Act
            await app.Run(cts.Token);

            // Assert
            resultSaverMock.VerifyAll();
        }

        private bool RequestResultIsEquals(RequestResult actual, RequestResult expected) => 
            actual.MediaType == expected.MediaType && actual.Url == expected.Url && actual.ContentLength == expected.ContentLength;
    }
}