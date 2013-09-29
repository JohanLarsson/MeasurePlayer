using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MeasurePlayer.Annotations;
using Microsoft.WindowsAPICodePack.Shell;

namespace MeasurePlayer
{
    public class Vm : INotifyPropertyChanged
    {
        public Vm(MediaElement mediaElement)
        {
            _mediaElement = mediaElement;
            _mediaElement.MediaFailed+= MediaElementOnMediaFailed;
            Bookmarks.CollectionChanged += (sender, e) =>
            {
                if (e.OldItems != null)
                {
                    foreach (INotifyPropertyChanged item in e.OldItems)
                        item.PropertyChanged -= SetDirty;
                }
                if (e.NewItems != null)
                    foreach (INotifyPropertyChanged item in e.NewItems)
                        item.PropertyChanged += SetDirty;
                IsBookmarksDirty = true;
            };
        }

        private void MediaElementOnMediaFailed(object sender, ExceptionRoutedEventArgs exceptionRoutedEventArgs)
        {
            throw new NotImplementedException();
        }

        private void SetDirty(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            IsBookmarksDirty = true;
        }

        private MediaTimeline _timeline = new MediaTimeline();
        private readonly MediaElement _mediaElement;
        public double FrameRate { get { return Info==null ?0: (double) Info.Properties.System.Video.FrameRate.Value/1000; } }

        public MediaClock Clock { get { return _mediaElement.Clock; } }

        private ClockController Controller { get { return Clock.Controller; } }

        private ICommand _playCmd;
        public ICommand PlayCmd
        {
            get { return _playCmd ?? (_playCmd = new RelayCommand(o => Play(), o => !IsPlaying)); }
        }

        public void Play()
        {
            if (!IsPlaying)
            {
                Controller.Resume();
                OnPropertyChanged("IsPlaying");
            }
        }

        private ICommand _pauseCmd;
        public ICommand PauseCmd
        {
            get { return _pauseCmd ?? (_pauseCmd = new RelayCommand(o => Pause(), o => this.IsPlaying)); }
        }


        public void Pause()
        {
            if (IsPlaying)
            {
                Controller.Pause();
                OnPropertyChanged("IsPlaying");
            }

        }

        public void TogglePlayPause()
        {
            if (IsPlaying)
                Pause();
            else
                Play();
        }

        private ICommand _stop;
        public ICommand Stop
        {
            get { return _stop ?? (_stop = new RelayCommand(o => Controller.Stop(), o => Clock != null && !_mediaElement.Clock.IsPaused)); }
        }

        private ICommand _saveBookmarksCmd;
        public ICommand SaveBookmarksCmd
        {
            get { return _saveBookmarksCmd ?? (_saveBookmarksCmd = new RelayCommand(o => SaveBookmarks(), o => IsBookmarksDirty)); }
        }

        private void SaveBookmarks()
        {
            BookmarksFile.SaveAsync(BookmarkFileName, Bookmarks);
            IsBookmarksDirty = false;
        }

        private string BookmarkFileName
        {
            get
            {
                var directory = System.IO.Path.GetDirectoryName(Path);
                var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(Path);
                return System.IO.Path.Combine(directory, fileNameWithoutExtension + ".bookmarks.xml");
            }
        }

        public bool IsBookmarksDirty { get; set; }

        public bool IsPlaying{get { return Clock != null && !_mediaElement.Clock.IsPaused; }}

        public TimeSpan CurrentTime { get { return (Clock !=null)? Clock.CurrentTime.Value:TimeSpan.Zero; } }

        public int CurrentFrame
        {
            get { return (int)Math.Round(FrameRate * CurrentTime.TotalSeconds, 0); }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > TotalFrames)
                    value = TotalFrames;
                Seek(TimeSpan.FromSeconds(value/FrameRate));
                OnPropertyChanged();
            }
        }

        private TimeSpan _totalTime;
        public TimeSpan TotalTime
        {
            get { return _totalTime; }
            set
            {
                if (value.Equals(_totalTime)) return;
                _totalTime = value;
                OnPropertyChanged();
                OnPropertyChanged("TotalFrames");
            }
        }

        public int TotalFrames { get { return (int)Math.Round(FrameRate * TotalTime.TotalSeconds, 0); } }

        public void Seek(TimeSpan timeSpan)
        {
            if (Clock == null || timeSpan < TimeSpan.Zero || timeSpan > Clock.NaturalDuration.TimeSpan)
                return;
            Controller.Seek(timeSpan, TimeSeekOrigin.BeginTime);
        }

        public void Step(int frames)
        {
            Seek(TimeSpan.FromSeconds(frames / FrameRate + Clock.CurrentTime.Value.TotalSeconds));
        }

        private string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                if (value == _path) return;

                if (AskBeforeSaveBookmarks() == MessageBoxResult.Cancel)
                {
                    OnPropertyChanged();
                    return;
                } 


                _path = value;
                Bookmarks.Clear();
                var bookmarksFile = BookmarksFile.Load(BookmarkFileName);
                foreach (var bookmark in bookmarksFile.Bookmarks)
                {
                    Bookmarks.Add(bookmark);
                }
                IsBookmarksDirty = false;
                _timeline.Source = new Uri(Path);
                _mediaElement.Clock = _timeline.Source != null
                    ? _timeline.CreateClock()
                    : null;
                if (Clock != null)
                {
                    Clock.CurrentStateInvalidated += (sender, args) =>
                    {
                       TotalTime = (Clock != null) ? Clock.NaturalDuration.TimeSpan : TimeSpan.Zero;
                    };
                    Clock.CurrentTimeInvalidated += (sender, args) =>
                    {
                        OnPropertyChanged("CurrentTime");
                        OnPropertyChanged("CurrentFrame");
                    };
                    Controller.Begin();
                    Controller.Pause();
                }
                else
                {
                    TotalTime = TimeSpan.Zero;
                }
                Info = ShellFile.FromFilePath(_path);

                OnPropertyChanged();
                OnPropertyChanged("Clock");
            }
        }

        private MessageBoxResult AskBeforeSaveBookmarks()
        {
            if (IsBookmarksDirty)
            {
                var result = MessageBox.Show("Do you want to save bookmarks?", "Save bookmarks", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Cancel)
                {

                    return result;
                }
                if (result == MessageBoxResult.Yes)
                {
                    BookmarksFile.SaveAsync(BookmarkFileName, Bookmarks);
                }
                return result;
            }
            return MessageBoxResult.No;
        }

        private ShellFile _info;
        public ShellFile Info
        {
            get { return _info; }
            set
            {
                if (Equals(value, _info)) return;
                _info = value;
                OnPropertyChanged();
            }
        }

        private TimeSpan _position;
        public TimeSpan Position
        {
            get { return _position; }
            set
            {
                if (value.Equals(_position)) return;
                _position = value;
                OnPropertyChanged();
            }
        }

        private Duration _duration;
        public Duration Duration
        {
            get { return _duration; }
            set
            {
                if (value.Equals(_duration)) return;
                _duration = value;
                OnPropertyChanged();
            }
        }

        public double Length { get; set; }

        private ObservableCollection<Bookmark> _bookmarks;
        public ObservableCollection<Bookmark> Bookmarks
        {
            get { return _bookmarks ?? (_bookmarks = new ObservableCollection<Bookmark>()); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }



    }
}
