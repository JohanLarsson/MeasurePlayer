namespace MeasurePlayer.Tests
{
    using NUnit.Framework;

    public class FrameRateTests
    {
        [TestCase(25000u, 1, 0.04)]
        [TestCase(25000u, 2, 0.08)]
        public void Roundtrip(uint framerateValue, int frames, double seconds)
        {
            var frameRate = FrameRate.Create(framerateValue);
            Assert.NotNull(frameRate);
            var time = frames / frameRate.Value;
            Assert.AreEqual(seconds, time.TotalSeconds);
            Assert.AreEqual(frames, time * frameRate.Value);
        }
    }
}
