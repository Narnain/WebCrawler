using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebCrawler
{
    public class PageCrawlResult
    {
        public string url { get; set; }
        public string HttpStatus { get; set; }

        public HtmlDocument Document { get; set; }
        public string ContentType { get; set; }
    }
}
