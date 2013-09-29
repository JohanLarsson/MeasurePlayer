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
using FirstFloor.ModernUI.Windows.Controls;

namespace MeasurePlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ToggleFullScreen()
        {
            //if (this.WindowStyle == WindowStyle.SingleBorderWindow)
            //{
            //    BookmarksExpander.Visibility = Visibility.Collapsed;
            //    MainMenu.Visibility = Visibility.Collapsed;
            //    this.WindowStyle = WindowStyle.None;
            //    this.WindowState = WindowState.Maximized;
            //}
            //else
            //{
            //    BookmarksExpander.Visibility = Visibility.Visible;
            //    MainMenu.Visibility = Visibility.Visible;
            //    this.WindowStyle = WindowStyle.SingleBorderWindow;
            //    this.WindowState = WindowState.Normal;
            //}
        }

        private void MainWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.Control)
            {
                ToggleFullScreen();
            }
            if (e.Key == Key.F11)
            {
                ToggleFullScreen();
            }
        }

    }
}
