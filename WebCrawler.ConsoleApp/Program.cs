using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler.Common;
using WebCrawler.Common.Interfaces;
using WebCrawler.Common.Strategies;

namespace WebCrawler.ConsoleApp
{
    // Написать web-crawler - консольное приложение, которое на вход получает:
    // - url входа,
    // - степень параллелизма(кол-во одновременно обрабатываемых url'ов)
    // Результат сохраняет в файл в виде:
    // url: content-type, response length
    // Учитывать возможность выделения кода в компонент, который будет встраиваться в другие приложения и тестируемость.
    class Program
    {
        static async Task Main(string[] args) => await BuildCommandLine()
            .UseDefaults()
            .Build()
            .InvokeAsync(args)
            .ConfigureAwait(false);

        private static CommandLineBuilder BuildCommandLine()
        {
            var root = new RootCommand(
                "$ dotnet run --urls 'url1' 'url2' --max-degree-of-parallelism 1 --output-file result.txt --strategy tdf")
            {
                new Option<List<string>>("--urls")
                {
                    Required = true
                },
                new Option<int>("--max-degree-of-parallelism")
                {
                    Required = true
                },
                new Option<string>("--output-file")
                {
                    Required = true
                },
                new Option<string>("--strategy")
                {
                    Required = false
                }
            };

            root.Handler = CommandHandler.Create<AppOptions>(Run);

            return new CommandLineBuilder(root);
        }

        private static async Task Run(AppOptions options)
        {
            Console.WriteLine($"Use '{options.SelectedStrategy}' strategy");

            using var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Console.WriteLine("Cancel event triggered");
                cts.Cancel();
                eventArgs.Cancel = true;
            };

            Console.WriteLine("Press Ctrl+C to exit...");

            IResultFormatter resultFormatter = new SimpleResultFormatter();
            IResultSaver resultSaver = new TextFileResultSaver(options.OutputFile, resultFormatter);

            using var httpClientFactory = new HttpClientFactoryWithTimeoutHandle();

            WebCrawlerStrategy crawlerStrategy = options.SelectedStrategy switch
            {
                AppOptions.Strategy.TasksDataflow => new TasksDataflowWebCrawlerStrategy(httpClientFactory, options,
                    resultSaver),
                AppOptions.Strategy.ParallelLinq => new ParallelLinqWebCrawlerStrategy(httpClientFactory, options,
                    resultSaver),
                AppOptions.Strategy.ParallelForEach => new ParallelForEachWebCrawlerStrategy(httpClientFactory, options,
                    resultSaver),
                AppOptions.Strategy.Partitioner => new PartitionerWebCrawlerStrategy(httpClientFactory, options,
                    resultSaver),
                AppOptions.Strategy.AsyncEnumerable => new AsyncEnumerableStrategy(httpClientFactory, options,
                    resultSaver),
                _ => throw new NotImplementedException($"Not implemented strategy: {options.SelectedStrategy}")
            };

            await new App(crawlerStrategy)
                .Run(cts.Token)
                .ConfigureAwait(false);

            Console.WriteLine("Finished...");
        }
    }
}