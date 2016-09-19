namespace MeasurePlayer
{
    using System;

    public struct FrameRate
    {
        private FrameRate(uint frameRateValue)
        {
            this.FrameRateValue = frameRateValue;
        }

        public uint FrameRateValue { get; }

        public double FramesPerSecond => this.FrameRateValue / 1000.0;

        public TimeSpan FrameDuration => TimeSpan.FromSeconds(1 / this.FramesPerSecond);

        public static TimeSpan operator /(int left, FrameRate right)
        {
            return TimeSpan.FromSeconds(left / right.FramesPerSecond);
        }

        public static TimeSpan operator /(ulong left, FrameRate right)
        {
            return TimeSpan.FromSeconds(left / right.FramesPerSecond);
        }

        public static double operator *(FrameRate left, TimeSpan right)
        {
            return left.FramesPerSecond * right.TotalSeconds;
        }

        public static double operator *(TimeSpan left, FrameRate right)
        {
            return right * left;
        }

        public static FrameRate? Create(uint? frameRateValue)
        {
            if (frameRateValue == null)
            {
                return null;
            }

            return new FrameRate(frameRateValue.Value);
        }
    }
}
