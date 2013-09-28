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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Vm _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = new Vm();
            this.DataContext = _vm;
        }

        private void Play(object sender, RoutedEventArgs e)
        {
            MediaElement.Play();
        }

        private void Pause(object sender, RoutedEventArgs e)
        {
            MediaElement.Pause();
        }

        private void Capture(object sender, RoutedEventArgs e)
        {
            _vm.Bookmarks.Add(new Bookmark(){Time = MediaElement.Position});
        }

        private void Forward(object sender, RoutedEventArgs e)
        {
            MediaElement.Position += TimeSpan.FromSeconds(1.0/25);
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            MediaElement.Position -= TimeSpan.FromSeconds(1.0 / 25);
        }

        private void VideoClick(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount!=2)
                return;
            _vm.Bookmarks.Add(new Bookmark() { Time = MediaElement.Position });
        }

    }
}
