using System;
using System.IO;
using System.Net;

namespace Fget
{
    public class FlickrImage
    {
        public FlickrAlbum Album { get; set; }
        public string Id { get; set; }

        public FlickrImage(FlickrAlbum album, string id)
        {
            Album = album;
            Id = id;
        }

        public bool Exists(string directory)
        {
            try
            {
                var fileName = MakeFileName(directory);
                return File.Exists(fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public void SaveTo(string directory)
        {
            try
            {
                Directory.CreateDirectory(directory);

                var document = (
                    "http://www.flickr.com/photos/" + Album.Member.Name + "/" + Id + "/sizes/o/in/Album-" + Album.Id + "/")
                    .GetHtmlDocument();
                var img = document.DocumentNode.SelectSingleNode("//*[@id='allsizes-photo']/img");
                var src = img.GetAttributeValue("src", "default");

                Console.WriteLine("Downloading " + Id + "...");
                new WebClient().DownloadFile(src, MakeFileName(directory));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private string MakeFileName(string directory)
        {
            return Path.Combine(directory, Id + ".jpg");
        }
    }
}
