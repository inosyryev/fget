using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using FlickrNet;

namespace Fget
{
    public class Program
    {
        public static void Main()
        {
            Download(Properties.Settings.Default.Directory, Properties.Settings.Default.Albums.Cast<string>());
        }

        public static void Download(string directory, IEnumerable<string> albumStrings)
        {
            var flickr = new Flickr("ac1be508d767a070769f0fb8c77588b5");

            var q =
                from s in albumStrings
                let parts = s.Split('|').Select(x => x.Trim()).ToArray()
                where parts.Length == 2
                select new {Member = parts[0], Album = parts[1]};
            var memberGroups = q.ToLookup(x => x.Member, x => x.Album);
            foreach (var memberGroup in memberGroups)
            {
                var user = flickr.UrlsLookupUser("http://www.flickr.com/photos/" + memberGroup.Key);
                Console.WriteLine("================ Member: {0}", user.UserName);

                var albums = flickr.PhotosetsGetList(user.UserId);
                foreach (var albumName in memberGroup)
                {
                    var album =
                        albums.FirstOrDefault(
                            x => x.Title.Equals(albumName, StringComparison.InvariantCultureIgnoreCase));
                    if (album == null)
                    {
                        Console.WriteLine("======== Album \"{0}\" was not found", albumName);
                        continue;
                    }

                    var albumDirectory = Path.Combine(directory, album.Title);
                    Directory.CreateDirectory(albumDirectory);
                    var existing =
                        Directory.GetFiles(albumDirectory).Select(Path.GetFileNameWithoutExtension).ToList();

                    var photos = flickr.PhotosetsGetPhotos(album.PhotosetId, PhotoSearchExtras.OriginalUrl);
                    var missing = photos.Where(x => !existing.Contains(x.PhotoId)).ToList();
                    Console.WriteLine("======== Album: {0} (Total: {1}, Missing: {2})", album.Title, photos.Count,
                        missing.Count);

                    foreach (var photo in missing)
                    {
                        try
                        {
                            Console.WriteLine("Downloading " + photo.PhotoId + "...");
                            new WebClient().DownloadFile(photo.OriginalUrl,
                                Path.Combine(albumDirectory, photo.PhotoId + ".jpg"));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
        }
    }
}
