using HoldTheHill.Features.Enemies;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class RollyPollyCurlTests
    {
        private static RollyPollyCurl NewCurl()
        {
            return new RollyPollyCurl(maxCurls: 3, rollDistance: 3f, curledDamageShare: 0.4, curledSpeedMultiplier: 2f);
        }

        [Test]
        public void TheFirstHit_LandsInFull_AndCurlsIt()
        {
            RollyPollyCurl curl = NewCurl();

            Assert.IsFalse(curl.IsCurled);
            Assert.AreEqual(10.0, curl.Hit(10.0));
            Assert.IsTrue(curl.IsCurled);
            Assert.AreEqual(2, curl.CurlsLeft);
            Assert.AreEqual(3f, curl.DistanceLeft);
            Assert.AreEqual(2f, curl.SpeedMultiplier);
        }

        [Test]
        public void HitsWhileCurled_LandAtTheReducedShare_AndDoNotUseACurl()
        {
            RollyPollyCurl curl = NewCurl();
            curl.Hit(10.0);

            Assert.AreEqual(4.0, curl.Hit(10.0), 0.0001);
            Assert.AreEqual(4.0, curl.Hit(10.0), 0.0001);
            Assert.AreEqual(2, curl.CurlsLeft);
        }

        [Test]
        public void UncurlsAfterRollingTheDistance_AndSaysSoOnce()
        {
            RollyPollyCurl curl = NewCurl();
            curl.Hit(10.0);

            Assert.IsFalse(curl.Advance(1f));
            Assert.IsFalse(curl.Advance(1.5f));
            Assert.IsTrue(curl.Advance(0.5f), "The call that finishes the roll reports it.");
            Assert.IsFalse(curl.IsCurled);
            Assert.AreEqual(0f, curl.DistanceLeft);
            Assert.AreEqual(1f, curl.SpeedMultiplier);
            Assert.IsFalse(curl.Advance(5f), "Walking while uncurled changes nothing.");
        }

        [Test]
        public void CurlsAtMostThreeTimes_ThenEveryHitLandsInFull()
        {
            RollyPollyCurl curl = NewCurl();

            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(10.0, curl.Hit(10.0), $"Curl {i + 1} is triggered by a full hit.");
                Assert.IsTrue(curl.IsCurled);
                curl.Advance(3f);
            }

            Assert.AreEqual(0, curl.CurlsLeft);
            Assert.AreEqual(10.0, curl.Hit(10.0));
            Assert.IsFalse(curl.IsCurled, "No curls left: it stays an ordinary walker.");
            Assert.AreEqual(10.0, curl.Hit(10.0));
        }

        [Test]
        public void ZeroOrNegativeDamage_NeitherLandsNorCurls()
        {
            RollyPollyCurl curl = NewCurl();

            Assert.AreEqual(0.0, curl.Hit(0.0));
            Assert.AreEqual(0.0, curl.Hit(-3.0));
            Assert.IsFalse(curl.IsCurled);
            Assert.AreEqual(3, curl.CurlsLeft);
        }

        [Test]
        public void NumbersAreClamped_ToSensibleRanges()
        {
            var curl = new RollyPollyCurl(maxCurls: -1, rollDistance: -2f, curledDamageShare: 1.5, curledSpeedMultiplier: -1f);

            Assert.AreEqual(0, curl.MaxCurls);
            Assert.AreEqual(0f, curl.RollDistance);
            Assert.AreEqual(1.0, curl.CurledDamageShare);
            Assert.AreEqual(0f, curl.CurledSpeedMultiplier);
            Assert.AreEqual(10.0, curl.Hit(10.0));
            Assert.IsFalse(curl.IsCurled, "With no curls at all it never curls.");
        }
    }
}
