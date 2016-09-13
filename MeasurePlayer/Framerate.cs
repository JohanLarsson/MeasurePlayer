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

        public static TimeSpan operator /(int left, FrameRate right)
        {
            return TimeSpan.FromSeconds(left / right.FramesPerSecond);
        }

        public static TimeSpan operator /(ulong left, FrameRate right)
        {
            return TimeSpan.FromSeconds(left / right.FramesPerSecond);
        }

        public static TimeSpan? operator /(int left, FrameRate? right)
        {
            // ReSharper disable once UseNullPropagation
            if (right == null)
            {
                return null;
            }

            return left / right.Value;
        }

        public static TimeSpan? operator /(ulong? left, FrameRate? right)
        {
            if (left == null || right == null)
            {
                return null;
            }

            return left.Value / right.Value;
        }

        public static double? operator *(FrameRate? left, TimeSpan right)
        {
            // ReSharper disable once UseNullPropagation
            if (left == null)
            {
                return null;
            }

            return left.Value.FramesPerSecond * right.TotalSeconds;
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
