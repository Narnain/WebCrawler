using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebCrawler
{
    public class CrawlResult
    {
        public string Url { get; set; }
        public string PageTitle { get; set; }
        public string HttpStatus { get; set; } 
    }
}
