namespace MeasurePlayer
{
    using System;

    using Microsoft.WindowsAPICodePack.Shell;

    public class VideoInfo
    {
        public VideoInfo(ShellFile shellFile)
        {
            this.FrameRate = MeasurePlayer.FrameRate.Create(shellFile.Properties.System.Video.FrameRate.Value);
            var durationValue = shellFile.Properties.System.Media.Duration;
            this.Duration = durationValue == null ? (TimeSpan?)null : TimeSpan.FromTicks((long)durationValue.Value);
        }

        public FrameRate? FrameRate { get; }

        public TimeSpan? Duration { get; }

        public uint? GetFrameAt(TimeSpan time)
        {
            return (uint?)(this.FrameRate * time);
        }

        public TimeSpan? GetTimeAtFrame(uint frame)
        {
            if (this.FrameRate == null)
            {
                return null;
            }

            return frame / this.FrameRate;
        }
    }
}