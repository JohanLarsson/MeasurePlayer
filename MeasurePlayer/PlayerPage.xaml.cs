namespace MeasurePlayer
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for PlayerPage.xaml
    /// </summary>
    public partial class PlayerPage : Page
    {
        private Vm vm;
        public PlayerPage()
        {
            this.InitializeComponent();
            this.vm = new Vm(this.MediaElement);
            this.DataContext = this.vm;
        }



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
    }
}
