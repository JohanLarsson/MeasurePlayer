namespace MeasurePlayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;
    using JetBrains.Annotations;

    public class Bookmark : INotifyPropertyChanged
    {
        public static readonly IEqualityComparer<Bookmark> NameTimeComparer = new NameTimeEqualityComparer();

        private string name;
        private TimeSpan time;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get
            {
                return this.name;
            }

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

        [XmlIgnore]
        public TimeSpan Time
        {
            get
            {
                return this.time;
            }

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

        // For serialization
        [XmlElement("Time")]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public long Ticks
        {
            get
            {
                return this.time.Ticks;
            }

            set
            {
                this.time = new TimeSpan(value);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private sealed class NameTimeEqualityComparer : IEqualityComparer<Bookmark>
        {
            public bool Equals(Bookmark x, Bookmark y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null))
                {
                    return false;
                }

                if (ReferenceEquals(y, null))
                {
                    return false;
                }

                if (x.GetType() != y.GetType())
                {
                    return false;
                }

                return string.Equals(x.name, y.name) && x.time.Equals(y.time);
            }

            public int GetHashCode(Bookmark obj)
            {
                unchecked
                {
                    return ((obj.name?.GetHashCode() ?? 0) * 397) ^ obj.time.GetHashCode();
                }
            }
        }
    }
}