namespace MeasurePlayer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Xml.Serialization;

#pragma warning disable INPC001 // Implement INotifyPropertyChanged.
    public class BookmarksFile
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(BookmarksFile));

        private BookmarksFile()
        {
            this.Bookmarks = new List<Bookmark>();
        }

        // ReSharper disable once MemberCanBePrivate.Global Only used for serialization
        public List<Bookmark> Bookmarks { get; set; }

        public static void AskBeforeSaveBookmarks(string bookmarksFile, IReadOnlyList<Bookmark> bookmarks)
        {
            if (bookmarksFile == null)
            {
                return;
            }

            var result = MessageBox.Show("Do you want to save bookmarks?", "Save bookmarks", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Cancel)
            {
                return;
            }

            if (result == MessageBoxResult.Yes)
            {
                Save(bookmarksFile, bookmarks);
            }
        }

        public static string GetBookmarksFileName(string fileName)
        {
            return Path.ChangeExtension(fileName, null) + ".bookmarks.xml";
        }

        public static void Save(string fileName, IReadOnlyList<Bookmark> bookmarks)
        {
            if (!bookmarks.Any())
            {
                File.Delete(fileName);
                return;
            }

            var bookmarksFile = new BookmarksFile { Bookmarks = bookmarks.OrderBy(x => x.Time).ToList() };
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
