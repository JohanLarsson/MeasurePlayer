namespace MeasurePlayer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using JetBrains.Annotations;
    using Microsoft.WindowsAPICodePack.Shell;

    public class Vm : INotifyPropertyChanged
    {
        private readonly MediaTimeline timeline = new MediaTimeline();
        private readonly MediaElement mediaElement;
        private string path;

        public Vm(MediaElement mediaElement)
        {
            this.path = "Browse for a file";
            this.mediaElement = mediaElement;
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

            this.PlayCommand = new RelayCommand(o => this.Play(), o => !this.IsPlaying);
            this.PauseCommand = new RelayCommand(o => this.Pause(), o => this.IsPlaying);
            this.TogglePlayPauseCommand = new RelayCommand(o => this.TogglePlayPause(), o => this.Clock != null);
            this.StopCommand = new RelayCommand(o => this.Stop(), o => this.Clock != null && !this.mediaElement.Clock.IsPaused);
            this.SaveBookmarksCmd = new RelayCommand(o => this.SaveBookmarks(), o => this.IsBookmarksDirty);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand PlayCommand { get; }

        public ICommand PauseCommand { get; }

        public ICommand TogglePlayPauseCommand { get; }

        public ICommand StopCommand { get; }

        public ICommand SaveBookmarksCmd { get; }

        public double FrameRate
        {
            get
            {
                var frameRate = this.Info?.Properties.System.Video.FrameRate;
                if (frameRate == null)
                {
                    return 0;
                }

                return (double)(frameRate.Value / 1000.0);
            }
        }

        public MediaClock Clock => this.mediaElement.Clock;

        private ClockController Controller => this.Clock.Controller;

        private void SetDirty(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            this.IsBookmarksDirty = true;
        }

        public bool IsPlaying
        {
            get { return this.mediaElement?.Clock.IsPaused != true; }
            set { this.TogglePlayPause(); }
        }

        private void SaveBookmarks()
        {
            BookmarksFile.Save(BookmarksFile.GetBookmarksFileName(this.path), this.Bookmarks);
            this.IsBookmarksDirty = false;
        }

        public bool IsBookmarksDirty { get; set; }

        public TimeSpan CurrentTime => this.Clock?.CurrentTime ?? TimeSpan.Zero;

        public int CurrentFrame
        {
            get
            {
                return (int)Math.Round(this.FrameRate * this.CurrentTime.TotalSeconds, 0);
            }

            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                if (value > this.TotalFrames)
                {
                    value = this.TotalFrames;
                }

                this.Seek(TimeSpan.FromSeconds(value / this.FrameRate));
                this.OnPropertyChanged();
            }
        }

        public TimeSpan TotalTime => this.Clock?.NaturalDuration.TimeSpan ?? TimeSpan.Zero;

        public int TotalFrames => (int)Math.Round(this.FrameRate * this.TotalTime.TotalSeconds, 0);

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
                    this.AskBeforeSaveBookmarks();
                }

                this.path = value;
                this.Bookmarks.Clear();
                var bookmarks = BookmarksFile.Load(BookmarksFile.GetBookmarksFileName(this.path));
                foreach (var bookmark in bookmarks)
                {
                    this.Bookmarks.Add(bookmark);
                }

                this.IsBookmarksDirty = false;
                this.timeline.Source = new Uri(this.Path);
                this.mediaElement.Clock = this.timeline.Source != null
                    ? this.timeline.CreateClock()
                    : null;
                if (this.Clock != null)
                {
                    this.Clock.CurrentStateInvalidated += (sender, args) =>
                        {
                            this.OnPropertyChanged(nameof(this.TotalTime));
                            this.OnPropertyChanged(nameof(this.TotalFrames));
                        };
                    this.Clock.CurrentTimeInvalidated += (sender, args) =>
                    {
                        this.OnPropertyChanged(nameof(this.CurrentTime));
                        this.OnPropertyChanged(nameof(this.CurrentFrame));
                    };
                    this.Controller.Begin();
                    this.Controller.Pause();
                }
                else
                {
                    this.OnPropertyChanged(nameof(this.TotalTime));
                    this.OnPropertyChanged(nameof(this.TotalFrames));
                }

                this.Info = ShellFile.FromFilePath(this.path);
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(this.Clock));
            }
        }

        private void AskBeforeSaveBookmarks()
        {
            if (this.IsBookmarksDirty)
            {
                var result = MessageBox.Show("Do you want to save bookmarks?", "Save bookmarks", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Cancel)
                {
                    return;
                }

                if (result == MessageBoxResult.Yes)
                {
                    BookmarksFile.Save(BookmarksFile.GetBookmarksFileName(this.path), this.Bookmarks);
                }
            }
        }

        private ShellFile info;
        public ShellFile Info
        {
            get
            {
                return this.info;
            }

            set
            {
                if (Equals(value, this.info))
                {
                    return;
                }

                this.info = value;
                this.OnPropertyChanged();
            }
        }

        private ObservableCollection<Bookmark> bookmarks;
        //private ObservableCollection<Bookmark> _innerBookMarks = new ObservableCollection<Bookmark>();
        public ObservableCollection<Bookmark> Bookmarks => this.bookmarks ?? (this.bookmarks = new ObservableCollection<Bookmark>());

        private List<Bookmark> selectedBookmarks;
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

        public void Play()
        {
            if (!this.IsPlaying)
            {
                this.Controller.Resume();
                this.OnPropertyChanged(nameof(this.IsPlaying));
            }
        }

        public void Pause()
        {
            if (this.IsPlaying)
            {
                this.Controller.Pause();
                this.OnPropertyChanged(nameof(this.IsPlaying));
            }
        }

        public void Stop()
        {
            if (this.IsPlaying)
            {
                this.Controller.Stop();
                this.OnPropertyChanged(nameof(this.IsPlaying));
            }
        }

        public void TogglePlayPause()
        {
            if (this.IsPlaying)
            {
                this.Pause();
            }
            else
            {
                this.Play();
            }
        }

        public void Seek(TimeSpan timeSpan)
        {
            if (this.Clock == null || timeSpan < TimeSpan.Zero || timeSpan > this.Clock.NaturalDuration.TimeSpan)
            {
                return;
            }

            this.Controller.Seek(timeSpan, TimeSeekOrigin.BeginTime);
        }

        public void Step(int frames)
        {
            var currentTime = this.Clock?.CurrentTime;
            if (currentTime == null)
            {
                return;
            }

            var jump = TimeSpan.FromSeconds(frames / this.FrameRate);
            this.Seek(jump + currentTime.Value);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddBookmark()
        {
            var bookmark = this.Bookmarks.FirstOrDefault(x => x.Time > this.CurrentTime);
            if (bookmark == null)
            {
                var item = new Bookmark { Time = this.CurrentTime };
                this.Bookmarks.Add(item);
            }
            else
            {
                var indexOf = this.Bookmarks.IndexOf(bookmark);
                this.Bookmarks.Insert(indexOf, new Bookmark { Time = this.CurrentTime });
            }
        }
    }
}
