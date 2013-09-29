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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MeasurePlayer
{
    /// <summary>
    /// Interaction logic for PlayerPage.xaml
    /// </summary>
    public partial class PlayerPage : Page
    {
        private Vm _vm;
        public PlayerPage()
        {
            InitializeComponent();
            _vm = new Vm(MediaElement);
            this.DataContext = _vm;
        }



        private void VideoClick(object sender, MouseButtonEventArgs e)
        {
            //if (e.ClickCount == 1)
            //{
            //    if (_vm.IsPlaying)
            //        _vm.Pause();
            //}
            if (e.ClickCount != 2)
                return;
            _vm.AddBookmark();
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
