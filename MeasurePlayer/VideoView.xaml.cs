namespace MeasurePlayer
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    using Microsoft.WindowsAPICodePack.Shell;

    public partial class VideoView : UserControl
    {
        private readonly MediaTimeline timeline = new MediaTimeline();

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source),
            typeof(string),
            typeof(VideoView),
            new PropertyMetadata(default(string), OnSourceChanged));

        private static readonly DependencyPropertyKey TotalTimePropertyKey = DependencyProperty.RegisterReadOnly(
            "TotalTime",
            typeof(TimeSpan?),
            typeof(VideoView),
            new PropertyMetadata(default(TimeSpan?)));

        public static readonly DependencyProperty TotalTimeProperty = TotalTimePropertyKey.DependencyProperty;

        public static readonly DependencyProperty CurrentTimeProperty = DependencyProperty.Register(
            nameof(CurrentTime),
            typeof(TimeSpan?),
            typeof(VideoView),
            new PropertyMetadata(default(TimeSpan?), OnCurrentTimeChanged));

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
            videoView.Seek(videoView.Info.GetTimeAtFrame((uint)e.NewValue).Value);
        }

        private static readonly DependencyPropertyKey InfoPropertyKey = DependencyProperty.RegisterReadOnly(
            "Info",
            typeof(VideoInfo),
            typeof(VideoView),
            new PropertyMetadata(default(VideoInfo)));

        public static readonly DependencyProperty InfoProperty = InfoPropertyKey.DependencyProperty;

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

        public TimeSpan? CurrentTime
        {
            get { return (TimeSpan?)this.GetValue(CurrentTimeProperty); }
            set { this.SetValue(CurrentTimeProperty, value); }
        }

        public TimeSpan? TotalTime
        {
            get { return (TimeSpan?)this.GetValue(TotalTimeProperty); }
            protected set { this.SetValue(TotalTimePropertyKey, value); }
        }

        public uint? CurrentFrame
        {
            get { return (uint?)this.GetValue(CurrentFrameProperty); }
            set { this.SetValue(CurrentFrameProperty, value); }
        }

        private MediaClock Clock => this.MediaElement.Clock;

        private ClockController Controller => this.Clock.Controller;

        public bool IsPlaying
        {
            get { return this.Clock?.IsPaused != true; }
            set { this.TogglePlayPause(); }
        }

        public void Play()
        {
            if (!this.IsPlaying)
            {
                this.Controller.Resume();
            }
        }

        public void Pause()
        {
            if (this.IsPlaying)
            {
                this.Controller.Pause();
            }
        }

        public void Stop()
        {
            if (this.IsPlaying)
            {
                this.Controller.Stop();
            }
        }

        public void TogglePlayPause()
        {
            if (this.IsPlaying)
            {
                this.Pause();
            }
            else
            {
                this.Play();
            }
        }

        public void Seek(TimeSpan timeSpan)
        {
            if (this.Clock == null || timeSpan < TimeSpan.Zero || timeSpan > this.Clock.NaturalDuration.TimeSpan)
            {
                return;
            }

            this.Controller.Seek(timeSpan, TimeSeekOrigin.BeginTime);
        }

        public void Step(int frames)
        {
            var currentTime = this.Clock?.CurrentTime;
            if (currentTime == null || this.Info?.FrameRate == null)
            {
                return;
            }

            var jump = TimeSpan.FromMilliseconds(frames / this.Info.FrameRate.Value);
            this.Seek(jump + currentTime.Value);
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

        private void OnTogglePlayPauseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.TogglePlayPause();
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.Step(Multiplier() * Math.Sign(e.Delta));
            e.Handled = true;
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

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VideoView)d).OnSourceChanged((string)e.NewValue);
        }

        protected void OnSourceChanged(string path)
        {
            if (this.Clock != null)
            {
                this.Clock.CurrentStateInvalidated -= this.OnCurrentStateInvalidated;
                this.Clock.CurrentTimeInvalidated -= this.OnCurrentTimeInvalidated;
            }

            if (string.IsNullOrEmpty(path))
            {
                this.Info = null;
                this.TotalTime = null;
                return;
            }

            this.timeline.Source = new Uri(path);
            this.MediaElement.Clock = this.timeline.Source != null
                ? this.timeline.CreateClock()
                : null;
            if (this.Clock != null)
            {
                this.Clock.CurrentStateInvalidated += this.OnCurrentStateInvalidated;
                this.Clock.CurrentTimeInvalidated += this.OnCurrentTimeInvalidated;
                this.Controller.Begin();
                this.Controller.Pause();
                this.TotalTime = this.Clock?.NaturalDuration.TimeSpan;
            }

            if (this.TotalTime != null)
            {
                this.Info = new VideoInfo(ShellFile.FromFilePath(path), this.TotalTime.Value);
            }
        }

        private void OnCurrentTimeInvalidated(object sender, EventArgs args)
        {
            var time = this.Clock.CurrentTime;
            this.CurrentTime = time;
            this.CurrentFrame = time == null
                                    ? null
                                    : (uint?)this.Info?.GetFrameAt(time.Value);
        }

        private void OnCurrentStateInvalidated(object sender, EventArgs args)
        {
            //this.OnPropertyChanged(nameof(this.TotalTime));
            //this.OnPropertyChanged(nameof(this.TotalFrames));
        }

        private static void OnCurrentTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VideoView)d).OnCurrentTimeChanged((TimeSpan?)e.NewValue);
        }

        private void OnCurrentTimeChanged(TimeSpan? time)
        {
            if (time != null)
            {
                this.Seek(time.Value);
            }
        }

        private void OnMediaFailed(object sender, ExceptionRoutedEventArgs exceptionRoutedEventArgs)
        {
            MessageBox.Show(exceptionRoutedEventArgs.ErrorException.Message, "Media failed", MessageBoxButton.OK);
        }
    }
}
