namespace MeasurePlayer
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    public sealed class BookmarksViewModel : INotifyPropertyChanged
    {
        private string? bookmarksFile;

        public BookmarksViewModel()
        {
            this.Bookmarks.CollectionChanged += (_, __) => this.SaveBookmarks();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<Bookmark> Bookmarks { get; } = new ObservableCollection<Bookmark>();

        public void Update(string bookmarksFileName)
        {
            this.bookmarksFile = null;
            this.Bookmarks.Clear();
            if (bookmarksFileName == null)
            {
                return;
            }

            var bookMarks = BookmarksFile.Load(bookmarksFileName);
            foreach (var bookmark in bookMarks)
            {
                this.Bookmarks.Add(bookmark);
            }

            this.bookmarksFile = bookmarksFileName;
        }

        public void AddBookmark(Bookmark bookmark)
        {
            for (int i = 0; i < this.Bookmarks.Count; i++)
            {
                if (this.Bookmarks[i].Time > bookmark.Time)
                {
                    if (i > 0)
                    {
                        this.Bookmarks.Insert(i - 1, bookmark);
                    }
                    else
                    {
                        this.Bookmarks.Insert(0, bookmark);
                    }

                    return;
                }
            }

            this.Bookmarks.Add(bookmark);
        }

        private void SaveBookmarks()
        {
            if (this.bookmarksFile != null)
            {
                BookmarksFile.Save(this.bookmarksFile, this.Bookmarks);
            }
        }
    }
}
