namespace MeasurePlayer
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Markup;

    [MarkupExtensionReturnType(typeof(NullToBoolConverter))]
    [ValueConversion(typeof(object), typeof(bool))]
    public class NullToBoolConverter : MarkupExtension, IValueConverter
    {
        public bool WhenNull { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? this.WhenNull : !this.WhenNull;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException($"{nameof(NullToBoolConverter)} can only be used in OneWay bindings");
        }
    }
}
