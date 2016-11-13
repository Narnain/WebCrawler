using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SimpleWebCrawler.Test
{
    public class CrawlQueueTests
    {
        [Fact]
        public void AddItemToQueue()
        {
            var q = new CrawlQueue();
            q.Add(new Uri("http://www.tyre-shopper.co.uk"));

            Assert.Equal(1, q.Length);
        }

        [Fact]
        public void AddTwoItemsToQueue()
        {
            var q = new CrawlQueue();
            q.Add(new Uri("http://www.tyre-shopper.co.uk"));
            q.Add(new Uri("http://www.tyre-shopper.co.uk/about"));

            Assert.Equal(2, q.Length);
        }

        [Fact]
        public void AddTwoItemsToQueue_trailingslashbug()
        {
            var q = new CrawlQueue();
            q.Add(new Uri("http://www.tyre-shopper.co.uk/about"));
            q.Add(new Uri("http://www.tyre-shopper.co.uk/about/"));

            Assert.Equal(1, q.Length);
        }

        [Fact]
        public void AddSameItemsToQueue()
        {
            var q = new CrawlQueue();
            q.Add(new Uri("http://www.tyre-shopper.co.uk"));
            q.Add(new Uri("http://www.tyre-shopper.co.uk"));

            Assert.Equal(1, q.Length);
        }

        [Fact]
        public void DequeueEmpty()
        {
            var q = new CrawlQueue();
            var data = q.Next();
            Assert.Equal(null, data);
            Assert.Equal(true, q.Done);
        }

        [Fact]
        public void AddItemDequeueAddAgain()
        {
            var q = new CrawlQueue();
            q.Add(new Uri("http://www.tyre-shopper.co.uk"));
            var data = q.Next();
            q.Add(new Uri("http://www.tyre-shopper.co.uk"));

            Assert.Equal(0, q.Length);
            Assert.Equal("http://www.tyre-shopper.co.uk/", data);
        }
    }
}
