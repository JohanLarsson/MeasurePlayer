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
using Microsoft.Win32;

namespace MeasurePlayer
{
    /// <summary>
    /// Interaction logic for FileControl.xaml
    /// </summary>
    public partial class FileControl : UserControl
    {
        public FileControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(FileControl), new PropertyMetadata(default(string)));
        public string Filter
        {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(FileControl), new PropertyMetadata(default(string), (o, e) => ((FileControl)o).Label.Content = e.NewValue));
        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(string), typeof(FileControl), new PropertyMetadata(default(string), (o, e) => ((FileControl)o).PathTb.Text = (string)e.NewValue));

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set
            {
                SetValue(PathProperty, value);
            }
        }


        private void BrowseClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = Filter,
                Multiselect = false
            };
            var showDialog = fileDialog.ShowDialog();
            if (showDialog == true)
            {
                Path = fileDialog.FileName;
                SetCurrentValue(PathProperty, fileDialog.FileName);
            }

        }

        private void PathTb_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Path = PathTb.Text;
            //SetCurrentValue(PathProperty, PathTb.Text);
        }

    }
}
