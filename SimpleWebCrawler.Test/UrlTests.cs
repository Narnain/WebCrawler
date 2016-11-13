using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SimpleWebCrawler.Test
{
    public class UrlTests
    {
        [Theory]
        [InlineData("http://www.tyre-shopper.co.uk", "", "http://www.tyre-shopper.co.uk/")]
        [InlineData("http://www.tyre-shopper.co.uk", "/", "http://www.tyre-shopper.co.uk/")]
        [InlineData("http://www.tyre-shopper.co.uk/home", "about", "http://www.tyre-shopper.co.uk/about")]
        [InlineData("http://www.tyre-shopper.co.uk/home/test", "about", "http://www.tyre-shopper.co.uk/home/about")]
        [InlineData("http://www.tyre-shopper.co.uk/home/test", "https://about.io", "https://about.io/")]
        public void FixLink(string pageUri, string linkUri, string result)
        {
            var c = new Crawler();
            var r = c.FixLink(pageUri, linkUri);

            Assert.Equal(result, r.AbsoluteUri);
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("<a href='http://www.tyre-shopper.co.uk'></a>", 1)]
        [InlineData("<a href='http://www.tyre-shopper.co.uk' rel='nofollow'></a>", 0)]
        [InlineData("<link rel='stylesheet' href='https://www.tyre-shopper.co.uk/?css=stylesheets/owl.themek.v.1435661505'>", 1)]
        [InlineData("<script type='text/javascript' async='' src='//www.gstatic.com/trustedstores/js/gtmp_compiled_WW65wIcSFx0.js'></script>", 1)]
        public void ExtactLinks(string html, int links)
        {
            var c = new Crawler();
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var pcr = new PageCrawlResult()
            {
                url = "http://www.tyre-shopper.co.uk",
                ContentType = "text/html",
                HttpStatus = "OK",
                Document = doc
            };

            var r = c.ExtractLinks(pcr);
            Assert.Equal(links, r.Count);
        }
    }
}
