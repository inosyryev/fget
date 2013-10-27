using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Fget
{
    public class FlickrAlbum
    {
        public FlickrMember Member { get; private set; }
        public string Id { get; private set; }
        public string Name { get; private set; }

        public FlickrAlbum(FlickrMember member, string id, string name)
        {
            Member = member;
            Id = id;
            Name = name;
        }

        public List<FlickrImage> GetImages()
        {
            var doc2 = ("http://www.flickr.com/photos/" + Member.Name + "/sets/" + Id + "/").GetHtmlDocument();
            var q2 =
                from node in doc2.DocumentNode.SelectNodes("//a[@data-track='photo-click']")
                let image = new FlickrImage(
                    this,
                    node.GetAttributeValue("href", "default")
                        .Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries)[2])
                group image by image.Id
                into g
                select g.First();
            return q2.ToList();
        }

        public void SaveTo(string directory)
        {
            directory = Path.Combine(directory, Name);

            var images = GetImages();
            var missing = images.Where(x => !x.Exists(directory)).ToList();
            Console.WriteLine("======== Album: {0} (Total: {1}, Missing: {2})", Name, images.Count, missing.Count);
            if (missing.Count == 0)
                return;

            Random random = null;
            foreach (var image in images)
            {
                if (random == null)
                    random = new Random();
                else
                    Thread.Sleep(TimeSpan.FromSeconds(random.Next(20)));

                image.SaveTo(directory);
            }
        }
    }
}
