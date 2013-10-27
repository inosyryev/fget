using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Fget.Tests
{
    [TestFixture]
    public class FgetTests
    {
        private static List<FlickrAlbum> GetAlbums()
        {
            var albums = new FlickrMember("thomashawk").GetFlickrAlbums();
            Assert.That(albums, Is.Not.Null);
            Assert.That(albums.Count, Is.GreaterThan(0));
            Assert.That(albums.Select(x => x.Id).Distinct().Count(), Is.EqualTo(albums.Count));
            return albums;
        }

        private static List<FlickrImage> GetImages(FlickrAlbum album)
        {
            var images = album.GetImages();
            Assert.That(images, Is.Not.Null);
            Assert.That(images.Count, Is.GreaterThan(0));
            Assert.That(images.Select(x => x.Id).Distinct().Count(), Is.EqualTo(images.Count));
            return images;
        }

        [Test]
        public void TestFlickrMemberGetFlickrAlbums()
        {
            var albums = GetAlbums();

            foreach (var album in albums)
            {
                Console.WriteLine(":" + album.Name + ":" + album.Id + ":");
            }
        }

        [Test]
        public void TestFlickrAlbumGetImages()
        {
            var album = GetAlbums()[0];
            Console.WriteLine(":" + album.Name + ":" + album.Id + ":");

            var images = GetImages(album);
            foreach (var image in images)
            {
                Console.WriteLine(image.Id);
            }
        }

        [Test]
        public void TestFlickrAlbumSaveTo()
        {
            var album = GetAlbums()[0];
            Console.WriteLine(":" + album.Name + ":" + album.Id + ":");

            var directory = Path.Combine("photos", Guid.NewGuid().ToString("N"));
            album.SaveTo(directory);

            var subDirectories = Directory.GetDirectories(directory);
            Assert.That(subDirectories, Is.Not.Null);
            Assert.That(subDirectories.Length, Is.EqualTo(1));
            Assert.That(Path.GetFileName(subDirectories[0]), Is.EqualTo(album.Name));

            var images = GetImages(album);

            var files = Directory.GetFiles(subDirectories[0]);
            Assert.That(files, Is.Not.Null);
            Assert.That(files, Is.Not.Contains(null));
            Assert.That(files.Select(Path.GetFileNameWithoutExtension).ToList(),
                Is.EquivalentTo(images.Select(x => x.Id).ToList()));
        }
    }
}
