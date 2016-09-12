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
        public MainWindow()
        {
            //this.DefaultStyleKey = typeof(ModernWindow);
            this.InitializeComponent();
            this.vm = new Vm(this.MediaElement);
            this.DataContext = this.vm;
        }

        private Vm vm;
        private void VideoClick(object sender, MouseButtonEventArgs e)
        {
            //if (e.ClickCount == 1)
            //{
            //    if (_vm.IsPlaying)
            //        _vm.Pause();
            //}
            if (e.ClickCount != 2)
            {
                return;
            }

            this.vm.AddBookmark();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var selected = this.Bookmarks.SelectedItems.Cast<Bookmark>().ToList();
            this.vm.SelectedBookmarks = selected;
            if (this.Bookmarks.SelectedItems.Count != 1)
            {
                return;
            }

            this.vm.Seek(((Bookmark)this.Bookmarks.SelectedItems[0]).Time);
        }

        private void VideoWheel(object sender, MouseWheelEventArgs e)
        {
            this.vm.Step(Multiplier() * Math.Sign(e.Delta));
        }

        private void VideoKey(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Space)
            {
                this.vm.TogglePlayPause();
            }
            var multiplier = Multiplier();
            if (e.Key == Key.Left)
            {
                this.vm.CurrentFrame -= multiplier;
            }
            else if (e.Key == Key.Right)
            {
                this.vm.CurrentFrame += multiplier;
            }
        }

        private static int Multiplier()
        {
            var multiplier = 1;
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                multiplier = 10;
            }

            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                multiplier = 100;
            }

            if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                multiplier = 1000;
            }

            return multiplier;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }

        private void ToggleFullScreen()
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

        private void MainWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
            {
                this.ToggleFullScreen();
            }
            if (e.Key == Key.F11)
            {
                this.ToggleFullScreen();
            }
        }

    }
}
