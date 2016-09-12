using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace MeasurePlayer
{
    using JetBrains.Annotations;

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
        [XmlIgnore]
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

        // Pretend property for serialization
        [XmlElement("Time"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public long Ticks
        {
            get { return _time.Ticks; }
            set { _time = new TimeSpan(value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}