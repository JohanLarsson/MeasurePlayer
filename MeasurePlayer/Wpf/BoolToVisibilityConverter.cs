namespace MeasurePlayer
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public static readonly BoolToVisibilityConverter Default = new BoolToVisibilityConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value
                       ? Visibility.Visible
                       : Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{typeof(BoolToVisibilityConverter).Name} does not support use in bindings with Mode = TwoWay.");
        }
    }
}
