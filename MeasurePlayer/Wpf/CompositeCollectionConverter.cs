namespace MeasurePlayer
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Windows.Data;

    public class CompositeCollectionConverter : IMultiValueConverter
    {
        public static readonly CompositeCollectionConverter Default = new CompositeCollectionConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var compositeCollection = new CompositeCollection();
            foreach (var value in values)
            {
                var enumerable = value as IEnumerable;
                if (enumerable != null)
                {
                    compositeCollection.Add(new CollectionContainer { Collection = enumerable });
                }
                else
                {
                    compositeCollection.Add(value);
                }
            }

            return compositeCollection;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("CompositeCollectionConverter ony supports oneway bindings");
        }
    }
}