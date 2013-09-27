using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MeasurePlayer.Annotations;

namespace MeasurePlayer
{
    public class Vm : INotifyPropertyChanged
    {
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

    public class Bookmark: INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (Equals(value, _name)) return;
                _name = value;
                OnPropertyChanged();
            }
        }
        private TimeSpan _time;
        public TimeSpan Time
        {
            get { return _time; }
            set
            {
                if (Equals(value, _time)) return;
                _time = value;
                OnPropertyChanged();
            }
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
