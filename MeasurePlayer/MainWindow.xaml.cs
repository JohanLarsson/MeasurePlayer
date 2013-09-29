using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MeasurePlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Vm _vm;
        public MainWindow()
        {
            InitializeComponent();
            _vm = new Vm(MediaElement);
            this.DataContext = _vm;
        }


        private void VideoClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                if(_vm.IsPlaying)
                    _vm.Pause();
            }
            if (e.ClickCount != 2)
                return;
            _vm.Bookmarks.Add(new Bookmark() { Time = MediaElement.Position });
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Bookmarks.SelectedItems.Count != 1)
                return;
            _vm.Seek(((Bookmark)Bookmarks.SelectedItems[0]).Time);
        }

        private void VideoWheel(object sender, MouseWheelEventArgs e)
        {
            _vm.Step(Multiplier() * Math.Sign(e.Delta));
        }

        private void VideoKey(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
            {
                ToggleFullScreen();
            }
            if (e.Key == Key.F11)
            {
                ToggleFullScreen();
            }
            if (e.Key == Key.Space)
            {
                _vm.TogglePlayPause();
            }
            var multiplier = Multiplier();
            if (e.Key == Key.Left)
            {
                _vm.CurrentFrame -= multiplier;
            }
            else if (e.Key == Key.Right)
            {
                _vm.CurrentFrame += multiplier;
            }
        }



        private void ToggleFullScreen()
        {
            if (this.WindowStyle == WindowStyle.SingleBorderWindow)
            {
                BookmarksExpander.Visibility = Visibility.Collapsed;
                MainMenu.Visibility = Visibility.Collapsed;
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                BookmarksExpander.Visibility = Visibility.Visible;
                MainMenu.Visibility = Visibility.Visible;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
            }
        }

        private static int Multiplier()
        {
            int multiplier = 1;
            if (Keyboard.Modifiers == ModifierKeys.Control)
                multiplier = 10;
            if (Keyboard.Modifiers == ModifierKeys.Shift)
                multiplier = 100;
            if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                multiplier = 1000;
            return multiplier;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }

    }
}
