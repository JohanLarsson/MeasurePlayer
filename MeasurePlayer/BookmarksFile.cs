﻿namespace MeasurePlayer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Xml.Serialization;

    public class BookmarksFile
    {
        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(BookmarksFile));

        private BookmarksFile()
        {
            this.Bookmarks = new List<Bookmark>();
        }

        // ReSharper disable once MemberCanBePrivate.Global Only used for serialization
        public List<Bookmark> Bookmarks { get; set; }

        public static void AskBeforeSaveBookmarks(string videoFullFileName, IEnumerable<Bookmark> bookmarks)
        {
            if (string.IsNullOrEmpty(videoFullFileName))
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
                Save(BookmarksFile.GetBookmarksFileName(videoFullFileName), bookmarks);
            }
        }

        public static string GetBookmarksFileName(string fileName)
        {
            return System.IO.Path.ChangeExtension(fileName, null) + ".bookmarks.xml";
        }

        public static void Save(string fileName, IEnumerable<Bookmark> bookmarks)
        {
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