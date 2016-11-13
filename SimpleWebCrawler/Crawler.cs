using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebCrawler
{
    public class CrawlOptions
    {
        public int MaxPages { get; set; } = 100000;
    }
    public class Crawler
    {

        public async Task<List<CrawlResult>> Crawl(string StartUrl)
        {
            var options = new CrawlOptions();
            return await Crawl(StartUrl, options);
        }

        public async Task<List<CrawlResult>> Crawl(string StartUrl, CrawlOptions options)
        {
            try
            {
                var result = new List<CrawlResult>();
                var startUri = new Uri(StartUrl);

                var queue = new CrawlQueue();
                queue.Add(startUri);

                while (!queue.Done)
                {
                    var page = queue.Next();
                    Console.WriteLine($"Fetching page {page}: Queue Remaining {queue.Length}");
                    var pcr = await FetchPage(page);
                    var cr = new CrawlResult();
                    cr.Url = pcr.url;
                    cr.HttpStatus = pcr.HttpStatus;
                    if (pcr.ContentType.ToLower().Contains("html"))
                    {
                        cr.PageTitle = ExtractTitle(pcr.Document);
                        result.Add(cr);
                        foreach (var lnk in ExtractLinks(pcr))
                        {
                            if (lnk?.IdnHost == startUri?.IdnHost)
                                queue.Add(lnk);
                        }
                    }

                    if (result.Count >= options.MaxPages)
                        break;
                }

                return result;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<PageCrawlResult> FetchPage(string url)
        {
            try
            {
                var client = new HttpClient();
                var result = await client.GetAsync(url);

                var cr = new PageCrawlResult();
                cr.url = url;
                cr.HttpStatus = result.StatusCode.ToString();
                cr.ContentType = result.Content?.Headers.ContentType.MediaType;
                if (cr.ContentType.ToLower().Contains("html"))
                {
                    HtmlDocument doc = new HtmlDocument();
                    var data = await result.Content.ReadAsStringAsync();
                    doc.LoadHtml(data);
                    cr.Document = doc;
                }
                return cr;
            }
            catch (Exception ex)
            {
               throw;
            }

        }

        public string ExtractTitle(HtmlDocument doc)
        {
            return doc.DocumentNode.SelectNodes("//title")?.First().InnerText;
        }


        public List<Uri> ExtractLinks(PageCrawlResult pcr)
        {
            // should also exclude nofollow
            var aLinks = pcr.Document.DocumentNode.SelectNodes("//a[@href]")?.Where(n=> n.Attributes["rel"]?.Value != "nofollow")?.Select(n=> FixLink(pcr.url, n.Attributes["href"].Value)).ToList();
            var imgLinks = pcr.Document.DocumentNode.SelectNodes("//img[@src]")?.Select(n => FixLink(pcr.url, n.Attributes["src"].Value)).ToList();
            var scriptLinks = pcr.Document.DocumentNode.SelectNodes("//script[@src]")?.Select(n => FixLink(pcr.url, n.Attributes["src"].Value)).ToList();
            var linkLinks = pcr.Document.DocumentNode.SelectNodes("//link[@href]")?.Select(n => FixLink(pcr.url, n.Attributes["href"].Value)).ToList();

            var allLinks = new List<Uri>();
            allLinks.AddRange(aLinks ?? new List<Uri>());
            allLinks.AddRange(imgLinks ?? new List<Uri>());
            allLinks.AddRange(scriptLinks ?? new List<Uri>());
            allLinks.AddRange(linkLinks ?? new List<Uri>());

            return allLinks.Distinct().ToList();
        }

        public Uri FixLink(string page, string link)
        {
            try
            {
                Uri uri;
                if (Uri.TryCreate(link, UriKind.Absolute, out uri))
                    return uri;
                if(Uri.TryCreate(new Uri(page), link, out uri))
                    return uri;

                return null;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

    }
}
