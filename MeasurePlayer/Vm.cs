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
        public Vm(MediaElement mediaElement)
        {
            this.path = "Browse for a file";
            this.mediaElement = mediaElement;
            this.mediaElement.MediaFailed += this.MediaElementOnMediaFailed;
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
        }

        private void MediaElementOnMediaFailed(object sender, ExceptionRoutedEventArgs exceptionRoutedEventArgs)
        {
            MessageBox.Show("Media failed");
        }

        private void SetDirty(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            this.IsBookmarksDirty = true;
        }

        private MediaTimeline timeline = new MediaTimeline();
        private readonly MediaElement mediaElement;
        public double FrameRate { get { return this.Info == null ? 0 : (double)this.Info.Properties.System.Video.FrameRate.Value / 1000; } }

        public MediaClock Clock { get { return this.mediaElement.Clock; } }

        private ClockController Controller { get { return this.Clock.Controller; } }

        private ICommand playCmd;
        public ICommand PlayCmd
        {
            get { return this.playCmd ?? (this.playCmd = new RelayCommand(o => this.Play(), o => !this.IsPlaying)); }
        }

        public void Play()
        {
            if (!this.IsPlaying)
            {
                this.Controller.Resume();
                this.OnPropertyChanged("IsPlaying");
            }
        }

        private ICommand pauseCmd;
        public ICommand PauseCmd
        {
            get { return this.pauseCmd ?? (this.pauseCmd = new RelayCommand(o => this.Pause(), o => this.IsPlaying)); }
        }

        private ICommand togglePlayPauseCmd;
        public ICommand TogglePlayPauseCmd
        {
            get { return this.togglePlayPauseCmd ?? (this.togglePlayPauseCmd = new RelayCommand(o => this.TogglePlayPause(), o => this.Clock != null)); }
        }

        public void Pause()
        {
            if (this.IsPlaying)
            {
                this.Controller.Pause();
                this.OnPropertyChanged("IsPlaying");
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

        private ICommand stop;
        public ICommand Stop
        {
            get { return this.stop ?? (this.stop = new RelayCommand(o => this.Controller.Stop(), o => this.Clock != null && !this.mediaElement.Clock.IsPaused)); }
        }

        private ICommand saveBookmarksCmd;
        public ICommand SaveBookmarksCmd
        {
            get { return this.saveBookmarksCmd ?? (this.saveBookmarksCmd = new RelayCommand(o => this.SaveBookmarks(), o => this.IsBookmarksDirty)); }
        }

        private void SaveBookmarks()
        {
            BookmarksFile.Save(this.BookmarkFileName, this.Bookmarks);
            this.IsBookmarksDirty = false;
        }

        private string BookmarkFileName
        {
            get
            {
                var directory = System.IO.Path.GetDirectoryName(this.Path);
                var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(this.Path);
                return System.IO.Path.Combine(directory, fileNameWithoutExtension + ".bookmarks.xml");
            }
        }

        public bool IsBookmarksDirty { get; set; }

        public bool IsPlaying
        {
            get
            {
                return this.Clock != null && !this.mediaElement.Clock.IsPaused;
            }
            set
            {
                this.TogglePlayPause();
            }
        }

        public TimeSpan CurrentTime { get { return (this.Clock != null) ? this.Clock.CurrentTime.Value : TimeSpan.Zero; } }

        public int CurrentFrame
        {
            get { return (int)Math.Round(this.FrameRate * this.CurrentTime.TotalSeconds, 0); }
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

        private TimeSpan totalTime;
        public TimeSpan TotalTime
        {
            get { return this.totalTime; }
            set
            {
                if (value.Equals(this.totalTime))
                {
                    return;
                }

                this.totalTime = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("TotalFrames");
            }
        }

        public int TotalFrames { get { return (int)Math.Round(this.FrameRate * this.TotalTime.TotalSeconds, 0); } }

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
            this.Seek(TimeSpan.FromSeconds(frames / this.FrameRate + this.Clock.CurrentTime.Value.TotalSeconds));
        }

        private string path;
        public string Path
        {
            get { return this.path; }
            set
            {
                if (value == this.path)
                {
                    return;
                }

                if (this.AskBeforeSaveBookmarks() == MessageBoxResult.Cancel)
                {
                    this.OnPropertyChanged();
                    return;
                }

                this.path = value;
                this.Bookmarks.Clear();
                var bookmarks = BookmarksFile.Load(this.BookmarkFileName);
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
                        this.TotalTime = (this.Clock != null) ? this.Clock.NaturalDuration.TimeSpan : TimeSpan.Zero;
                    };
                    this.Clock.CurrentTimeInvalidated += (sender, args) =>
                    {
                        this.OnPropertyChanged("CurrentTime");
                        this.OnPropertyChanged("CurrentFrame");
                    };
                    this.Controller.Begin();
                    this.Controller.Pause();
                }
                else
                {
                    this.TotalTime = TimeSpan.Zero;
                }
                this.Info = ShellFile.FromFilePath(this.path);

                this.OnPropertyChanged();
                this.OnPropertyChanged("Clock");
            }
        }

        private MessageBoxResult AskBeforeSaveBookmarks()
        {
            if (this.IsBookmarksDirty)
            {
                var result = MessageBox.Show("Do you want to save bookmarks?", "Save bookmarks", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Cancel)
                {

                    return result;
                }
                if (result == MessageBoxResult.Yes)
                {
                    BookmarksFile.Save(this.BookmarkFileName, this.Bookmarks);
                }
                return result;
            }
            return MessageBoxResult.No;
        }

        private ShellFile info;
        public ShellFile Info
        {
            get { return this.info; }
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

        private TimeSpan position;
        public TimeSpan Position
        {
            get { return this.position; }
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

        private Duration duration;
        public Duration Duration
        {
            get { return this.duration; }
            set
            {
                if (value.Equals(this.duration))
                {
                    return;
                }

                this.duration = value;
                this.OnPropertyChanged();
            }
        }

        public double Length { get; set; }

        private ObservableCollection<Bookmark> bookmarks;
        //private ObservableCollection<Bookmark> _innerBookMarks = new ObservableCollection<Bookmark>();
        public ObservableCollection<Bookmark> Bookmarks
        {
            get { return this.bookmarks ?? (this.bookmarks = new ObservableCollection<Bookmark>()); }
        }

        private List<Bookmark> selectedBookmarks;
        public List<Bookmark> SelectedBookmarks
        {
            get { return this.selectedBookmarks; }
            set
            {
                if (Equals(value, this.selectedBookmarks))
                {
                    return;
                }

                this.selectedBookmarks = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged("Diff");
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public void AddBookmark()
        {
            var bookmark = this.Bookmarks.FirstOrDefault(x => x.Time > this.CurrentTime);
            if (bookmark == null) this.Bookmarks.Add(new Bookmark() { Time = this.CurrentTime });
            else
            {
                var indexOf = this.Bookmarks.IndexOf(bookmark);
                this.Bookmarks.Insert(indexOf, new Bookmark() { Time = this.CurrentTime });
            }

        }
    }
}
