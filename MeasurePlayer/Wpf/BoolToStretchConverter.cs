namespace MeasurePlayer
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class BoolToStretchConverter : IValueConverter
    {
        public static readonly BoolToStretchConverter Default = new BoolToStretchConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value
                       ? Stretch.Uniform
                       : Stretch.None;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{this.GetType().Name} does not support use in bindings with Mode = TwoWay.");
        }
    }
}