namespace MeasurePlayer
{
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for FileControl.xaml
    /// </summary>
    public partial class FileControl : UserControl
    {
        public static readonly DependencyProperty FilterProperty = DependencyProperty.Register(
            "Filter",
            typeof(string),
            typeof(FileControl),
            new PropertyMetadata(default(string)));

        public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(
            "LabelText",
            typeof(string),
            typeof(FileControl),
            new PropertyMetadata(default(string), (o, e) => ((FileControl)o).Label.Content = e.NewValue));

        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
            "Path",
            typeof(string),
            typeof(FileControl),
            new PropertyMetadata(default(string), (o, e) => ((FileControl)o).PathTb.Text = (string)e.NewValue));

        public FileControl()
        {
            this.InitializeComponent();
        }

        public string Filter
        {
            get { return (string)this.GetValue(FilterProperty); }
            set { this.SetValue(FilterProperty, value); }
        }

        public string LabelText
        {
            get { return (string)this.GetValue(LabelTextProperty); }
            set { this.SetValue(LabelTextProperty, value); }
        }

        public string Path
        {
            get { return (string)this.GetValue(PathProperty); }
            set { this.SetValue(PathProperty, value); }
        }

        private void BrowseClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = this.Filter,
                Multiselect = false
            };
            var showDialog = fileDialog.ShowDialog();
            if (showDialog == true)
            {
                this.Path = fileDialog.FileName;
                this.SetCurrentValue(PathProperty, fileDialog.FileName);
            }
        }

        private void PathTb_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            this.Path = this.PathTb.Text;
            //SetCurrentValue(PathProperty, PathTb.Text);
        }
    }
}
