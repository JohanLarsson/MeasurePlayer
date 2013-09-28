using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace MeasurePlayer
{
    public class Vm : INotifyPropertyChanged
    {
        public Vm(MediaElement mediaElement)
        {
            _mediaElement = mediaElement;
        }
        private MediaTimeline _timeline = new MediaTimeline();
        private readonly MediaElement _mediaElement;
        private double FrameRate { get { return 25; } }

        public MediaClock Clock { get { return _mediaElement.Clock; } }

        private ClockController Controller { get { return Clock.Controller; } }

        private ICommand _play;
        public ICommand Play
        {
            get { return _play ?? (_play = new RelayCommand(o => Controller.Resume(), o => Clock != null && _mediaElement.Clock.IsPaused)); }
        }

        private ICommand _pause;
        public ICommand Pause
        {
            get { return _pause ?? (_pause = new RelayCommand(o => Controller.Pause(), o => Clock != null && !_mediaElement.Clock.IsPaused)); }
        }

        private ICommand _stop;
        public ICommand Stop
        {
            get { return _stop ?? (_stop = new RelayCommand(o => Controller.Pause(), o => Clock != null && !_mediaElement.Clock.IsPaused)); }
        }

        public void Seek(TimeSpan timeSpan)
        {
            if (Clock == null ||timeSpan<TimeSpan.Zero || timeSpan>Clock.NaturalDuration.TimeSpan)
                return;
            Controller.Seek(timeSpan,TimeSeekOrigin.BeginTime);
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
                _path = value;
                _timeline.Source = new Uri(Path);
                _mediaElement.Clock = _timeline.Source != null
                    ? _timeline.CreateClock()
                    : null;
                if (Clock != null)
                {
                    Controller.Pause();
                }

                OnPropertyChanged();
                OnPropertyChanged("Clock");
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
