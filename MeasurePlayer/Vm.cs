namespace MeasurePlayer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using JetBrains.Annotations;
    using Microsoft.WindowsAPICodePack.Shell;

    public class Vm : INotifyPropertyChanged
    {
        private List<Bookmark> selectedBookmarks;
        private ObservableCollection<Bookmark> bookmarks;
        private string path;

        public Vm()
        {
            this.Bookmarks.CollectionChanged += (sender, e) =>
            {
                if (e.OldItems != null)
                {
                    foreach (INotifyPropertyChanged item in e.OldItems)
                    {
                        item.PropertyChanged -= this.SetDirty;
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (INotifyPropertyChanged item in e.NewItems)
                    {
                        item.PropertyChanged += this.SetDirty;
                    }
                }

                this.IsBookmarksDirty = true;
            };
            this.SaveBookmarksCmd = new RelayCommand(o => this.SaveBookmarks(), o => this.IsBookmarksDirty);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SaveBookmarksCmd { get; }

        private void SetDirty(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            this.IsBookmarksDirty = true;
        }

        private void SaveBookmarks()
        {
            BookmarksFile.Save(BookmarksFile.GetBookmarksFileName(this.path), this.Bookmarks);
            this.IsBookmarksDirty = false;
        }

        public bool IsBookmarksDirty { get; set; }

        public string Path
        {
            get
            {
                return this.path;
            }

            set
            {
                if (value == this.path)
                {
                    return;
                }

                if (this.IsBookmarksDirty)
                {
                    BookmarksFile.AskBeforeSaveBookmarks(this.path, this.bookmarks);
                }

                this.path = value;
                this.Bookmarks.Clear();
                var bookmarks = BookmarksFile.Load(BookmarksFile.GetBookmarksFileName(this.path));
                foreach (var bookmark in bookmarks)
                {
                    this.Bookmarks.Add(bookmark);
                }

                this.IsBookmarksDirty = false;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<Bookmark> Bookmarks => this.bookmarks ?? (this.bookmarks = new ObservableCollection<Bookmark>());

        public List<Bookmark> SelectedBookmarks
        {
            get
            {
                return this.selectedBookmarks;
            }

            set
            {
                if (Equals(value, this.selectedBookmarks))
                {
                    return;
                }

                this.selectedBookmarks = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.Diff));
            }
        }

        public TimeSpan Diff
        {
            get
            {
                if (this.SelectedBookmarks == null || this.SelectedBookmarks.Count < 2)
                {
                    return TimeSpan.Zero;
                }

                return this.SelectedBookmarks.Max(x => x.Time) - this.SelectedBookmarks.Min(x => x.Time);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddBookmark(Bookmark bookmark)
        {
            for (int i = 0; i < this.bookmarks.Count; i++)
            {
                if (this.bookmarks[i].Time > bookmark.Time)
                {
                    if (i > 0)
                    {
                        this.bookmarks.Insert(i - 1, bookmark);
                    }
                    else
                    {
                        this.bookmarks.Insert(0, bookmark);
                    }

                    return;
                }
            }

            this.bookmarks.Add(bookmark);
        }
    }
}
