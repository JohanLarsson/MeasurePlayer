using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MeasurePlayer.Annotations;

namespace MeasurePlayer
{
    public class Bookmark : INotifyPropertyChanged
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