namespace MeasurePlayer
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class TimeSpanToMsConverter :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((TimeSpan) value).TotalMilliseconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fromMilliseconds = TimeSpan.FromMilliseconds((double) value);
            return fromMilliseconds;
        }
    }
}
