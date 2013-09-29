using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MeasurePlayer
{
    public class BookmarksFile
    {
        public BookmarksFile()
        {
            Bookmarks= new List<Bookmark>();
        }
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(BookmarksFile));
        public List<Bookmark> Bookmarks { get; set; }

        public static async Task SaveAsync(string fileName, IEnumerable<Bookmark> bookmarks)
        {
            BookmarksFile bookmarksFile = new BookmarksFile {Bookmarks = bookmarks.ToList()};
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, bookmarksFile);
                memoryStream.Position = 0;
                using (FileStream stream = new FileStream(fileName, FileMode.Create))
                {
                    await memoryStream.CopyToAsync(stream);
                }
            }
        }

        public static BookmarksFile Load(string fileName)
        {
            try
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    var file = (BookmarksFile)Serializer.Deserialize(stream);
                    return file;
                }
            }
            catch (FileNotFoundException)
            {
                return new BookmarksFile();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task<BookmarksFile> LoadAsync(string fileName)
        {
            try
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    MemoryStream memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    var file = (BookmarksFile) Serializer.Deserialize(memoryStream);
                    return file;
                }
            }
            catch (FileNotFoundException)
            {
                return new BookmarksFile();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}