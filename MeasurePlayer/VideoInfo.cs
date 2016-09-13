namespace MeasurePlayer
{
    using System;

    using Microsoft.WindowsAPICodePack.Shell;

    public class VideoInfo
    {
        public VideoInfo(ShellFile shellFile, TimeSpan length)
        {
            this.FrameRate = shellFile.Properties.System.Video.FrameRate.Value;
            this.TotaleFrames = (uint?)(this.FrameRate * length.TotalMilliseconds);
        }

        public uint? FrameRate { get; }

        public uint? TotaleFrames { get; }

        public int? GetFrameAt(TimeSpan time)
        {
            return (int?)(this.FrameRate * time.TotalMilliseconds);
        }

        public TimeSpan? GetTimeAtFrame(uint frame)
        {
            if (this.FrameRate == null)
            {
                return null;
            }

            return TimeSpan.FromMilliseconds(this.FrameRate.Value * frame);
        }
    }
}