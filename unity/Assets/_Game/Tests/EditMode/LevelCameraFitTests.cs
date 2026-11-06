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
        public void WithTheListBand_TheWholeMapSitsLeftOfIt_AtEveryAspect()
        {
            foreach (float aspect in new[] { 16f / 9f, 16f / 10f, 4f / 3f, 21f / 9f })
            {
                float size = LevelCameraFit.OrthographicSize(aspect, halfWidth: 10f, halfHeight: 5f, bottomBand: 0.09f, topBand: 0.15f, rightBand: 0.3f);
                float x = LevelCameraFit.CameraX(size, aspect, rightBand: 0.3f);
                float viewWidth = 2f * size * aspect;

                // Share of the screen width, from the left, of the map's two ends.
                float spawnEnd = 0.5f + (-10f - x) / viewWidth;
                float queenEnd = 0.5f + (10f - x) / viewWidth;
                Assert.GreaterOrEqual(spawnEnd, -1e-4f, $"the spawn is on screen at aspect {aspect}");
                Assert.LessOrEqual(queenEnd, 0.7f + 1e-4f, $"the Queen clears the list at aspect {aspect}");
            }
        }

        [Test]
        public void WithNoRightBand_TheCameraStaysCentred()
        {
            Assert.AreEqual(0f, LevelCameraFit.CameraX(6f, 16f / 9f, rightBand: 0f), 1e-6f);
        }

        [Test]
        public void ABiggerBottomBand_LowersTheCamera_SoTheMapSitsHigher()
        {
            Assert.Less(LevelCameraFit.CameraHeight(6f, bottomBand: 0.09f, topBand: 0.07f), 0f);
            Assert.AreEqual(0f, LevelCameraFit.CameraHeight(6f, bottomBand: 0.08f, topBand: 0.08f), 1e-6f);
        }
    }
}
