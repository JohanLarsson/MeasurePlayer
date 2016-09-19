﻿namespace MeasurePlayer
{
    using System;

    using Microsoft.WindowsAPICodePack.Shell;

    public class VideoInfo
    {
        public VideoInfo(ShellFile shellFile)
        {
            this.FrameRate = MeasurePlayer.FrameRate.Create(shellFile.Properties.System.Video.FrameRate.Value);
            this.Duration = CreateDuration(shellFile.Properties.System.Media.Duration.Value);
        }

        public FrameRate? FrameRate { get; }

        public TimeSpan FrameDuration => FrameRate?.FrameDuration ?? TimeSpan.FromMilliseconds(100);

        public TimeSpan? Duration { get; }

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