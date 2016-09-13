namespace MeasurePlayer
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    using Microsoft.WindowsAPICodePack.Shell;

    public partial class VideoView : UserControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source),
            typeof(string),
            typeof(VideoView),
            new PropertyMetadata(default(string), OnSourceChanged));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VideoView)d).Info = new VideoInfo(ShellFile.FromFilePath((string)e.NewValue));
        }

        public static readonly DependencyProperty CurrentFrameProperty = DependencyProperty.Register(
            nameof(CurrentFrame),
            typeof(uint?),
            typeof(VideoView),
            new PropertyMetadata(default(uint?), OnCurrentFrameChanged));

        private static void OnCurrentFrameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var videoView = (VideoView)d;
            if (e.NewValue == null || videoView.Info?.FrameRate == null)
            {
                return;
            }

            // ReSharper disable once PossibleInvalidOperationException already checked
            videoView.MediaElement.Position = videoView.Info.GetTimeAtFrame((uint)e.NewValue).Value;
        }

        private static readonly DependencyPropertyKey InfoPropertyKey = DependencyProperty.RegisterReadOnly(
            "Info",
            typeof(VideoInfo),
            typeof(VideoView),
            new PropertyMetadata(default(VideoInfo)));

        public static readonly DependencyProperty InfoProperty = InfoPropertyKey.DependencyProperty;

        private MediaState mediaState;

        public string Source
        {
            get { return (string)this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }

        public VideoView()
        {
            this.InitializeComponent();
        }

        public VideoInfo Info
        {
            get { return (VideoInfo)this.GetValue(InfoProperty); }
            protected set { this.SetValue(InfoPropertyKey, value); }
        }

        public uint? CurrentFrame
        {
            get { return (uint?)this.GetValue(CurrentFrameProperty); }
            set { this.SetValue(CurrentFrameProperty, value); }
        }

        public void Step(int frames)
        {
            if (this.MediaElement.Position == null || this.Info?.FrameRate == null)
            {
                return;
            }

            var jump = frames / this.Info.FrameRate;
            if (jump != null)
            {
                this.MediaElement.Skip(jump.Value);
            }
        }

        private static int Multiplier()
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                return 10;
            }

            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                return 100;
            }

            if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                return 1000;
            }

            return 1;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.Step(Multiplier() * Math.Sign(e.Delta));
            e.Handled = true;
        }

        //// ReSharper disable UnusedParameter.Local
        private void OnProgressSliderDragStarted(object sender, DragStartedEventArgs e)
        //// ReSharper restore UnusedParameter.Local
        {
            this.mediaState = this.MediaElement.State;
            this.MediaElement.Pause();
        }

        //// ReSharper disable UnusedParameter.Local
        private void OnProgressSliderDragCompleted(object sender, DragCompletedEventArgs e)
        //// ReSharper restore UnusedParameter.Local
        {
            if (this.mediaState == MediaState.Play)
            {
                this.MediaElement.Play();
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                this.Step(-Multiplier());
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                this.Step(Multiplier());
                e.Handled = true;
            }
        }

        private void OnMediaFailed(object sender, ExceptionRoutedEventArgs exceptionRoutedEventArgs)
        {
            MessageBox.Show(exceptionRoutedEventArgs.ErrorException.Message, "Media failed", MessageBoxButton.OK);
        }
    }
}
