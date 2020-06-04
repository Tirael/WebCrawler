using System;
using System.Collections.Generic;

namespace WebCrawler.Common
{
    public class AppOptions
    {
        public enum Strategy
        {
            TasksDataflow,
            ParallelLinq,
            ParallelForEach,
            Partitioner,
            AsyncEnumerable,
        }

        public List<string> Urls { get; }
        public int MaxDegreeOfParallelism { get; }
        public string OutputFile { get; }
        public Strategy SelectedStrategy { get; }

        public AppOptions(List<string> urls, int maxDegreeOfParallelism, string outputFile, string strategy)
        {
            Urls = urls;
            MaxDegreeOfParallelism = maxDegreeOfParallelism;
            OutputFile = outputFile;
            SelectedStrategy = GetStrategy(strategy);
        }

        private static Strategy GetStrategy(string strategy)
        {
            return strategy switch
            {
                "tdf" => Strategy.TasksDataflow,
                "plinq" => Strategy.ParallelLinq,
                "foreach" => Strategy.ParallelForEach,
                "part" => Strategy.Partitioner,
                "asyncenum" => Strategy.AsyncEnumerable,
                _ => throw new NotImplementedException($"Unknown strategy: {strategy}")
            };
        }
    }
}