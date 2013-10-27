using System.Collections.Generic;
using System.Linq;

namespace Fget
{
    public class FlickrMember
    {
        public string Name { get; private set; }

        public FlickrMember(string name)
        {
            Name = name;
        }

        public List<FlickrAlbum> GetFlickrAlbums()
        {
            var doc = ("http://www.flickr.com/photos/" + Name + "/sets/").GetHtmlDocument();
            var q =
                from node in doc.DocumentNode.SelectNodes("//a[@class='Seta']")
                let album = new FlickrAlbum(
                    this,
                    node.GetAttributeValue("href", "default").Split('/').Last(x => x.Length > 0),
                    node.GetAttributeValue("title", "default"))
                group album by album.Id
                into g
                select g.First();
            return q.ToList();
        }
    }
}
