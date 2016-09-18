namespace MeasurePlayer
{
    using System;
    using System.ComponentModel;
    using System.IO;
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

        public MainViewModel()
        {
            this.AddBookmarkCommand = new RelayCommand(_ => this.AddBookmarkAtCurrentTime(), _ => this.source != null);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand AddBookmarkCommand { get; }

        public BookmarksViewModel BookMarks { get; } = new BookmarksViewModel();

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
                this.BookMarks.AddBookmark(new Bookmark { Time = time.Value });
            }
        }
    }
}
