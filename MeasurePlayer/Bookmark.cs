namespace MeasurePlayer
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;
    using JetBrains.Annotations;

    public class Bookmark : INotifyPropertyChanged
    {
        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                if (Equals(value, this.name))
                {
                    return;
                }

                this.name = value;
                this.OnPropertyChanged();
            }
        }

        private TimeSpan time;
        [XmlIgnore]
        public TimeSpan Time
        {
            get { return this.time; }
            set
            {
                if (Equals(value, this.time))
                {
                    return;
                }

                this.time = value;
                this.OnPropertyChanged();
            }
        }

        // Pretend property for serialization
        [XmlElement("Time"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public long Ticks
        {
            get { return this.time.Ticks; }
            set {
                this.time = new TimeSpan(value); }
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
    }
}