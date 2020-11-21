namespace MeasurePlayer
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Input;

    public class MainViewModel : INotifyPropertyChanged
    {
        private bool isFullScreen;
        private TimeSpan? position;
        private VideoInfo? info;
        private TimeSpan? diff;
        private Bookmark? selectedBookmark;
        private string? mediaFileName;

        public MainViewModel()
        {
            this.SelectedBookmarks.CollectionChanged += (_, __) =>
            {
                this.Diff = this.SelectedBookmarks.Count < 2
                                ? (TimeSpan?)null
                                : this.SelectedBookmarks.Max(x => x.Time) - this.SelectedBookmarks.Min(x => x.Time);
            };

            this.AddBookmarkCommand = new RelayCommand(_ => this.AddBookmarkAtCurrentTime(), _ => this.mediaFileName != null);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ICommand AddBookmarkCommand { get; }

        public BookmarksViewModel BookMarks { get; } = new BookmarksViewModel();

        public ObservableCollection<Bookmark> SelectedBookmarks { get; } = new ObservableCollection<Bookmark>();

        public Bookmark? SelectedBookmark
        {
            get => this.selectedBookmark;

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
            get => this.diff;

            private set
            {
                if (value == this.diff)
                {
                    return;
                }

                this.diff = value;
                this.OnPropertyChanged();
            }
        }

        public string? MediaFileName
        {
            get => this.mediaFileName;

            set
            {
                if (value == this.mediaFileName)
                {
                    return;
                }

                this.mediaFileName = value;
                this.Info = VideoInfo.CreateOrDefault(this.mediaFileName);
                this.BookMarks.Update(BookmarksFile.GetBookmarksFileName(this.mediaFileName));
                this.OnPropertyChanged();
            }
        }

        public VideoInfo? Info
        {
            get => this.info;

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
            get => this.isFullScreen;
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
            get => this.position;

            set
            {
                if (value == this.position)
                {
                    return;
                }

                this.position = value;
                this.OnPropertyChanged();
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
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
