namespace MeasurePlayer
{
    using System;

    using Microsoft.WindowsAPICodePack.Shell;

    public class VideoInfo
    {
        private VideoInfo(string filename)
        {
            using (var shellFile = ShellFile.FromFilePath(filename))
            {
                this.FrameRate = MeasurePlayer.FrameRate.Create(shellFile.Properties.System.Video.FrameRate.Value);
                this.Duration = CreateDuration(shellFile.Properties.System.Media.Duration.Value);
            }
        }

        public static TimeSpan DefaultDuration { get; } = TimeSpan.FromMilliseconds(1 / 25.0);

        public FrameRate? FrameRate { get; }

        public TimeSpan FrameDuration => this.FrameRate?.FrameDuration ?? DefaultDuration;

        public TimeSpan? Duration { get; }

        public static VideoInfo? CreateOrDefault(string? mediaFileName)
        {
            if (string.IsNullOrEmpty(mediaFileName))
            {
                return null;
            }

            return new VideoInfo(mediaFileName);
        }

        public uint? GetFrameAt(TimeSpan time)
        {
            return (uint?)(this.FrameRate * time);
        }

        public TimeSpan? GetTimeAtFrame(uint frame)
        {
            return frame / this.FrameRate;
        }

        private static TimeSpan? CreateDuration(ulong? durationValue)
        {
            return durationValue == null
                       ? (TimeSpan?)null
                       : TimeSpan.FromTicks((long)durationValue.Value);
        }
    }
}
