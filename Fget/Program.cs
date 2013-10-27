using System;
using System.Collections.Generic;
using System.Linq;

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
            var q =
                from s in albumStrings
                let parts = s.Split('|').Select(x => x.Trim()).ToArray()
                where parts.Length == 2
                select new {Member = parts[0], Album = parts[1]};
            var memberGroups = q.ToLookup(x => x.Member, x => x.Album);
            foreach (var memberGroup in memberGroups)
            {
                var member = new FlickrMember(memberGroup.Key);
                Console.WriteLine("================ Member: {0}", member.Name);

                var albums = member.GetFlickrAlbums();
                foreach (var albumName in memberGroup)
                {
                    var album =
                        albums.FirstOrDefault(x => x.Name.Equals(albumName, StringComparison.InvariantCultureIgnoreCase));
                    if (album == null)
                        Console.WriteLine("======== Album \"{0}\" was not found", albumName);
                    else
                        album.SaveTo(directory);
                }
            }
        }
    }
}
