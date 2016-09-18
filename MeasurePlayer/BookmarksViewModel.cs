namespace MeasurePlayer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using JetBrains.Annotations;

    public class BookmarksViewModel : INotifyPropertyChanged
    {
        private Bookmark selectedBookMark;
        private TimeSpan? diff;
        private string bookmarksFile;
        private IEnumerable<Bookmark> lastSavedState;

        public BookmarksViewModel()
        {
            this.SelectedBookmarks.CollectionChanged += (_, __) =>
                {
                    this.Diff = this.SelectedBookmarks.Count < 2
                                    ? (TimeSpan?)null
                                    : this.SelectedBookmarks.Max(x => x.Time) - this.SelectedBookmarks.Min(x => x.Time);
                };

            this.SaveBookmarksCmd = new RelayCommand(o => this.SaveBookmarks(), _=>this.IsBookmarksDirty());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SaveBookmarksCmd { get; }

        public ObservableCollection<Bookmark> Bookmarks { get; } = new ObservableCollection<Bookmark>();

        public ObservableCollection<Bookmark> SelectedBookmarks { get; } = new ObservableCollection<Bookmark>();

        public TimeSpan? Diff
        {
            get
            {
                return this.diff;
            }

            private set
            {
                if (value.Equals(this.diff))
                {
                    return;
                }

                this.diff = value;
                this.OnPropertyChanged();
            }
        }

        public Bookmark SelectedBookMark
        {
            get
            {
                return this.selectedBookMark;
            }

            set
            {
                if (Equals(value, this.selectedBookMark))
                {
                    return;
                }

                this.selectedBookMark = value;
                this.OnPropertyChanged();
            }
        }

        public void Update(string bookmarksFileName)
        {
            if (this.IsBookmarksDirty())
            {
                BookmarksFile.AskBeforeSaveBookmarks(this.bookmarksFile, this.Bookmarks);
            }

            this.bookmarksFile = bookmarksFileName;
            this.Bookmarks.Clear();
            if (bookmarksFileName == null)
            {
                return;
            }

            this.lastSavedState = BookmarksFile.Load(bookmarksFileName);
            foreach (var bookmark in this.lastSavedState)
            {
                this.Bookmarks.Add(bookmark);
            }
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

        private bool IsBookmarksDirty()
        {
            if (this.lastSavedState == null)
            {
                return false;
            }

            return Enumerable.SequenceEqual(this.lastSavedState, this.Bookmarks, Bookmark.NameTimeComparer);
        }

        private void SaveBookmarks()
        {
            BookmarksFile.Save(this.bookmarksFile, this.Bookmarks);
            this.lastSavedState = this.Bookmarks.ToArray();
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
