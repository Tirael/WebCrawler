using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebCrawler.Common;
using WebCrawler.Common.Interfaces;

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
            .InvokeAsync(args);

        private static CommandLineBuilder BuildCommandLine()
        {
            var root = new RootCommand(
                "$ dotnet run --urls 'url1' 'url2' --max-degree-of-parallelism 1 --output-file result.txt")
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
                }
            };

            root.Handler = CommandHandler.Create<AppOptions>(Run);

            return new CommandLineBuilder(root);
        }

        private static async Task Run(AppOptions options)
        {
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
            IHttpClientFactory httpClientFactory = new HttpClientFactoryWithTimeoutHandle();

            await new App(httpClientFactory, options, resultSaver).Run(cts.Token);

            Console.WriteLine("Finished...");
        }
    }
}