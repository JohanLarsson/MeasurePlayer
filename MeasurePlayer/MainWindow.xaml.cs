﻿namespace MeasurePlayer
{
    using System;
    using System.Windows;
    using System.Windows.Input;

    using Gu.Wpf.Media;

    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel vm = new MainViewModel();

        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = this.vm;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files == null)
                {
                    return;
                }

                if (files.Length > 1)
                {
                    MessageBox.Show(
                        this,
                        "Only onde file at the time can be dropped.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }

                try
                {
                    this.vm.Source = new Uri(files[0], UriKind.Absolute);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(
                        this,
                        exception.Message,
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void OnToggleFullScreenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.WindowStyle == WindowStyle.SingleBorderWindow)
            {
                this.vm.IsFullScreen = true;
                this.SizeToContent = SizeToContent.Manual;
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.OnEndFullScreenExecuted(sender,e);
            }

            e.Handled = true;
        }

        private void OnEndFullScreenCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.WindowState == WindowState.Maximized && this.WindowStyle == WindowStyle.None;
        }

        private void OnEndFullScreenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.vm.IsFullScreen = false;
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.WindowState = WindowState.Normal;
            e.Handled = true;
        }

        private void OnMediaFailed(object sender, ExceptionRoutedEventArgs exceptionRoutedEventArgs)
        {
            MessageBox.Show(exceptionRoutedEventArgs.ErrorException.Message, "Media failed", MessageBoxButton.OK);
        }

        private void OnHelpExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var window = new Window { Content = new HelpView(), SizeToContent = SizeToContent.WidthAndHeight };
            window.ShowDialog();
        }

        private void OnOpenExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = $"Media files|{this.MediaElement.VideoFormats}|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                this.vm.Source = new Uri(openFileDialog.FileName, UriKind.Absolute);
            }
        }
    }
}
