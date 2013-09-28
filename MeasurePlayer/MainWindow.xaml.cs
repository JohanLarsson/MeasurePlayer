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
            if(e.ClickCount!=2)
                return;
            _vm.Bookmarks.Add(new Bookmark() { Time = MediaElement.Position });
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(Bookmarks.SelectedItems.Count!=1)
                return;
            _vm.Seek(((Bookmark)Bookmarks.SelectedItems[0]).Time);
        }

        private void VideoWheel(object sender, MouseWheelEventArgs e)
        {
            _vm.Step(Math.Sign(e.Delta));
        }

    }
}
