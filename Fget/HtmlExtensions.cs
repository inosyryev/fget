using System.IO;
using System.Net;
using HtmlAgilityPack;

namespace Fget
{
    public static class HtmlExtensions
    {
        public static HtmlDocument GetHtmlDocument(this string url)
        {
            var data = new WebClient().DownloadData(url);
            var stream = new MemoryStream(data);
            var doc = new HtmlDocument();
            doc.Load(stream);
            return doc;
        }
    }
}
