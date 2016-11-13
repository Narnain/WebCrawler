using CsvHelper;
using Fclp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebCrawler
{
    class Program
    {
        //TODO: Instrumentation and logging - Application Insights
        //TODO: Review Handling for 301 & 302 redirects - not relevant to http://www.tyre-shopper.co.uk
        //TODO: Respect robots.txt - not relevant to http://www.tyre-shopper.co.uk

        public class SimpleWebCrawlerArgs
        {
            public string Url { get; set; }
            public string Filepath { get; set; }
        }
        static void Main(string[] args)
        {
            var p = new FluentCommandLineParser<SimpleWebCrawlerArgs>();
            
            p.Setup(arg => arg.Url)
             .As('u', "url")
             .SetDefault("http://www.tyre-shopper.co.uk");

            p.Setup(arg => arg.Filepath)
             .As('f', "file")
             .SetDefault("crawlresult.csv");
            
            var argResult = p.Parse(args);

            if (argResult.HasErrors == false)
            {
                var options = p.Object;
                
                var c = new Crawler();
                var result = c.Crawl(options.Url).Result;

                using (var csv = new CsvWriter(new StreamWriter(options.Filepath)))
                {
                    csv.Configuration.QuoteAllFields = true;
                    csv.WriteRecords(result);
                }
            }
        }
        
    }
}
