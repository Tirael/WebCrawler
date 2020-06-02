using System.Collections.Generic;

namespace WebCrawler.Common
{
    public class AppOptions
    {
        public List<string> Urls { get; }
        public int MaxDegreeOfParallelism { get; }
        public string OutputFile { get; }
        
        public AppOptions(List<string> urls, int maxDegreeOfParallelism, string outputFile)
        {
            Urls = urls;
            MaxDegreeOfParallelism = maxDegreeOfParallelism;
            OutputFile = outputFile;
        }
    }
}