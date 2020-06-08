using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using WebCrawler.Common;
using WebCrawler.Common.Interfaces;
using WebCrawler.Common.Strategies;
using WebCrawler.Tests.Extensions;

namespace WebCrawler.Tests
{
    [TestFixture]
    public class AppTests
    {
        private Mock<HttpClientHandler> _mockHandler;
        private CancellationTokenSource _cts;
        private FakeHttpClientFactory _httpClientFactory;
        private Mock<IResultSaver> _resultSaverMock;
        private List<string> _urls;

        [SetUp]
        public void Setup()
        {
            _cts = new CancellationTokenSource();

            _resultSaverMock = new Mock<IResultSaver>();

            _urls = new List<string>
            {
                "https://google.com",
                "https://amazon.com",
                "https://www.linux.org"
            };

            var expectedMediaType = "text/plain";

            _mockHandler = new Mock<HttpClientHandler>();
            _httpClientFactory = new FakeHttpClientFactory(_mockHandler);

            foreach (var url in _urls)
            {
                var uri = new Uri(url);
                var expectedResponse = $"Test response: {url}";
                var expectedLength = expectedResponse.Length;

                _mockHandler.SetupSendAsync(uri, expectedResponse, "text/plain");

                var targetRequestResult = new RequestResult(uri, expectedMediaType, expectedLength);

                _resultSaverMock
                    .Setup(saver =>
                        saver.SaveAsync(
                            It.Is<RequestResult>(result => RequestResultIsEquals(result, targetRequestResult)),
                            It.IsAny<CancellationToken>()))
                    .Returns(() => Task.FromResult(expectedResponse))
                    .Verifiable();
            }
        }

        [Test]
        public async Task When_ParallelForEachWebCrawlerStrategyExecute_Then_SaveRequestResultIsCalled()
        {
            // Arrange
            var appOptions = new AppOptions(_urls, 2, "test.txt", "foreach");
            var strategy =
                new ParallelForEachWebCrawlerStrategy(_httpClientFactory, appOptions, _resultSaverMock.Object);

            // Act
            await strategy.ExecuteAsync(_cts.Token);

            // Assert
            _resultSaverMock.VerifyAll();
        }

        [Test]
        public async Task When_ParallelLinqWebCrawlerStrategyExecute_Then_SaveRequestResultIsCalled()
        {
            // Arrange
            var appOptions = new AppOptions(_urls, 2, "test.txt", "plinq");
            var strategy = new ParallelLinqWebCrawlerStrategy(_httpClientFactory, appOptions, _resultSaverMock.Object);

            // Act
            await strategy.ExecuteAsync(_cts.Token);

            // Assert
            _resultSaverMock.VerifyAll();
        }

        [Test]
        public async Task When_PartitionerWebCrawlerStrategyExecute_Then_SaveRequestResultIsCalled()
        {
            // Arrange
            var appOptions = new AppOptions(_urls, 2, "test.txt", "part");
            var strategy = new PartitionerWebCrawlerStrategy(_httpClientFactory, appOptions, _resultSaverMock.Object);

            // Act
            await strategy.ExecuteAsync(_cts.Token);

            // Assert
            _resultSaverMock.VerifyAll();
        }

        [Test]
        public async Task When_TasksDataflowWebCrawlerStrategyExecute_Then_SaveRequestResultIsCalled()
        {
            // Arrange
            var appOptions = new AppOptions(_urls, 2, "test.txt", "tdf");
            var strategy = new TasksDataflowWebCrawlerStrategy(_httpClientFactory, appOptions, _resultSaverMock.Object);

            // Act
            await strategy.ExecuteAsync(_cts.Token);

            // Assert
            _resultSaverMock.VerifyAll();
        }

        [Test]
        public async Task When_AsyncEnumerableStrategyExecute_Then_SaveRequestResultIsCalled()
        {
            // Arrange
            var appOptions = new AppOptions(_urls, 2, "test.txt", "asyncenum");
            var strategy = new AsyncEnumerableStrategy(_httpClientFactory, appOptions, _resultSaverMock.Object);

            // Act
            await strategy.ExecuteAsync(_cts.Token).ConfigureAwait(false);

            // Assert
            _resultSaverMock.VerifyAll();
        }

        private bool RequestResultIsEquals(RequestResult actual, RequestResult expected) =>
            actual.MediaType == expected.MediaType && actual.Uri == expected.Uri &&
            actual.ContentLength == expected.ContentLength;
    }
}