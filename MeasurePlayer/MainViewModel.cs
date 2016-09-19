namespace MeasurePlayer
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    using JetBrains.Annotations;

    using Microsoft.WindowsAPICodePack.Shell;

    public class MainViewModel : INotifyPropertyChanged
    {
        private Uri source;
        private bool isFullScreen;
        private TimeSpan? position;
        private VideoInfo info;
        private TimeSpan? diff;

        private Bookmark selectedBookmark;

        public MainViewModel()
        {
            this.SelectedBookmarks.CollectionChanged += (_, __) =>
            {
                this.Diff = this.SelectedBookmarks.Count < 2
                                ? (TimeSpan?)null
                                : this.SelectedBookmarks.Max(x => x.Time) - this.SelectedBookmarks.Min(x => x.Time);
            };
            this.AddBookmarkCommand = new RelayCommand(_ => this.AddBookmarkAtCurrentTime(), _ => this.source != null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AddBookmarkCommand { get; }

        public BookmarksViewModel BookMarks { get; } = new BookmarksViewModel();

        public ObservableCollection<Bookmark> SelectedBookmarks { get; } = new ObservableCollection<Bookmark>();

        public Bookmark SelectedBookmark
        {
            get
            {
                return this.selectedBookmark;
            }

            set
            {
                if (Equals(value, this.selectedBookmark))
                {
                    return;
                }

                this.selectedBookmark = value;
                if (this.selectedBookmark != null)
                {
                    this.Position = this.selectedBookmark.Time;
                }

                this.OnPropertyChanged();
            }
        }

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

        public Uri Source
        {
            get
            {
                return this.source;
            }

            set
            {
                if (Equals(value, this.source))
                {
                    return;
                }

                this.source = value;
                if (this.source == null)
                {
                    this.Info = null;
                    this.BookMarks.Update(null);
                }
                else
                {
                    this.Info = new VideoInfo(ShellFile.FromFilePath(this.source.LocalPath));
                    this.BookMarks.Update(BookmarksFile.GetBookmarksFileName(this.source.LocalPath));
                }

                this.OnPropertyChanged();
            }
        }

        public VideoInfo Info
        {
            get
            {
                return this.info;
            }

            private set
            {
                if (Equals(value, this.info))
                {
                    return;
                }

                this.info = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsFullScreen
        {
            get
            {
                return this.isFullScreen;
            }

            set
            {
                if (value == this.isFullScreen)
                {
                    return;
                }

                this.isFullScreen = value;
                this.OnPropertyChanged();
            }
        }

        public TimeSpan? Position
        {
            get
            {
                return this.position;
            }

            set
            {
                if (value.Equals(this.position))
                {
                    return;
                }

                this.position = value;
                this.OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddBookmarkAtCurrentTime()
        {
            var time = this.Position;
            if (time != null)
            {
                this.BookMarks.AddBookmark(new Bookmark { Name = $"#{this.BookMarks.Bookmarks.Count + 1}", Time = time.Value });
            }
        }
    }
}
