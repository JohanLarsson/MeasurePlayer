namespace MeasurePlayer
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Vm vm;

        public MainWindow()
        {
            this.InitializeComponent();
            this.vm = new Vm();
            this.DataContext = this.vm;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = this.Bookmarks.SelectedItems.Cast<Bookmark>().ToList();
            this.vm.SelectedBookmarks = selected;
            if (this.Bookmarks.SelectedItems.Count == 1)
            {
                this.VideoView.Seek(((Bookmark)this.Bookmarks.SelectedItems[0]).Time);
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                this.vm.Path = files[0];
            }
        }

        private void OnToggleFullScreenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.WindowStyle == WindowStyle.SingleBorderWindow)
            {
                this.BookmarksExpander.Visibility = Visibility.Collapsed;
                this.VideoUrl.Visibility = Visibility.Collapsed;
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.BookmarksExpander.Visibility = Visibility.Visible;
                this.VideoUrl.Visibility = Visibility.Visible;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
            }
        }

        private void OnSaveBookmarkExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var time = this.VideoView.CurrentTime;
            if (time != null)
            {
                this.vm.AddBookmark(new Bookmark { Time = time.Value });
            }
        }

        private void OnHelpExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var window = new Window { Content = new HelpView(), SizeToContent = SizeToContent.WidthAndHeight };
            window.ShowDialog();
        }
    }
}
