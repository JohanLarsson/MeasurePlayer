namespace MeasurePlayer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    public class BookmarksFile
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(BookmarksFile));

        private BookmarksFile()
        {
            this.Bookmarks= new List<Bookmark>();
        }

        public List<Bookmark> Bookmarks { get; set; }

        public static void Save(string fileName, IEnumerable<Bookmark> bookmarks)
        {
            var bookmarksFile = new BookmarksFile {Bookmarks = bookmarks.ToList()};
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                Serializer.Serialize(stream, bookmarksFile);
            }
        }

        public static IReadOnlyList<Bookmark> Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return new Bookmark[0];
            }
            try
            {
                using (var stream = new FileStream(fileName, FileMode.Open))
                {
                    var file = (BookmarksFile)Serializer.Deserialize(stream);
                    return file.Bookmarks;
                }
            }
            catch (FileNotFoundException)
            {
                return new Bookmark[0];
            }
        }
    }
}