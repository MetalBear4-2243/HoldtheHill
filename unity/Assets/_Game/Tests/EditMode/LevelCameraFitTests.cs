using HoldTheHill.Features.Levels;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class LevelCameraFitTests
    {
        [Test]
        public void At16By9_TheHeightWithItsHudBands_SetsTheSize()
        {
            float size = LevelCameraFit.OrthographicSize(16f / 9f, halfWidth: 10f, halfHeight: 5f, bottomBand: 0.09f, topBand: 0.07f);

            Assert.AreEqual(5f / 0.84f, size, 1e-4f);
            Assert.GreaterOrEqual(size * 16f / 9f, 10f, "the spawn and the Queen stay on screen");
        }

        [Test]
        public void At4By3_TheWidthSetsTheSize()
        {
            float size = LevelCameraFit.OrthographicSize(4f / 3f, halfWidth: 10f, halfHeight: 5f, bottomBand: 0.09f, topBand: 0.07f);

            Assert.AreEqual(7.5f, size, 1e-4f);
        }

        [Test]
        public void ABiggerBottomBand_LowersTheCamera_SoTheMapSitsHigher()
        {
            Assert.Less(LevelCameraFit.CameraHeight(6f, bottomBand: 0.09f, topBand: 0.07f), 0f);
            Assert.AreEqual(0f, LevelCameraFit.CameraHeight(6f, bottomBand: 0.08f, topBand: 0.08f), 1e-6f);
        }
    }
}
