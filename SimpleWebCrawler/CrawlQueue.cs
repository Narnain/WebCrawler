using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleWebCrawler
{
    public enum QueueStatus
    {
        Success,
        Duplicate,
        Error
    }
    public class CrawlQueue
    {
        public CrawlQueue() { }

        public CrawlQueue(int threshold)
        {
            DequeueThreshold = threshold;
        }
        public int DequeueThreshold { get; set; } = 3333;
        DateTime LastDequeue { get; set; } = DateTime.UtcNow;
        Queue<string> ToBeCrawled { get; set; } = new Queue<string>();
        List<string> Crawled { get; set; } = new List<string>();

        public QueueStatus Add(Uri url)
        {
            if (url.AbsoluteUri.EndsWith("/", StringComparison.Ordinal))
                url = new Uri(url.AbsoluteUri.Remove(url.AbsoluteUri.Length - 1, 1));
            //Should take into account robots file
            if(Crawled.Any(page => String.Equals(page, url.AbsoluteUri, StringComparison.OrdinalIgnoreCase)) || ToBeCrawled.Any(page => String.Equals(page, url.AbsoluteUri, StringComparison.OrdinalIgnoreCase)))
            {
                return QueueStatus.Duplicate;
            }
            ToBeCrawled.Enqueue(url.AbsoluteUri.ToLower());
            return QueueStatus.Success;
        }

        public string Next()
        {
            if (!Done)
            {
                var elapsed = DateTime.UtcNow - LastDequeue;
                if (elapsed.Milliseconds < DequeueThreshold)
                    Thread.Sleep(DequeueThreshold - elapsed.Milliseconds);

                var url = ToBeCrawled.Dequeue();
                Crawled.Add(url);
                return url;
            }
            return null;
        }

        public int Length
        {
            get
            {
                return ToBeCrawled.Count;
            }
        }

        public bool Done
        {
            get {
                return ToBeCrawled.Count == 0;
            }
        }
    }
}
