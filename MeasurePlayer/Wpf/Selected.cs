namespace MeasurePlayer
{
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    public static class Selected
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.RegisterAttached(
            "Items",
            typeof(IList),
            typeof(Selected),
            new PropertyMetadata(default(IList)));

        static Selected()
        {
            EventManager.RegisterClassHandler(typeof(Selector), Selector.SelectionChangedEvent, new SelectionChangedEventHandler(OnSelectionChanged));
        }

        public static void SetItems(this Selector element, IList value)
        {
            element.SetValue(ItemsProperty, value);
        }

        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(Selector))]
        public static IList GetItems(this Selector element)
        {
            return (IList)element.GetValue(ItemsProperty);
        }

        private static void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selector = (Selector)sender;
            var items = selector.GetItems();
            if (items == null)
            {
                return;
            }

            foreach (var item in e.AddedItems)
            {
                items.Add(item);
            }

            foreach (var item in e.RemovedItems)
            {
                items.Remove(item);
            }
        }
    }
}
