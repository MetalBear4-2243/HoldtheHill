using HoldTheHill.Features.Towers;
using NUnit.Framework;
using UnityEngine;

namespace HoldTheHill.Tests
{
    public class RangeRingTests
    {
        [Test]
        public void CirclePoints_AllLieOnTheRange_AroundTheTower()
        {
            var centre = new Vector3(2.5f, -1.5f, 0f);
            var points = new Vector3[RangeRing.Segments];
            RangeRing.CirclePoints(centre, 1.5f, points);

            foreach (Vector3 point in points)
            {
                Assert.AreEqual(1.5f, Vector2.Distance(centre, point), 1e-4f);
                Assert.AreEqual(centre.z, point.z);
            }
        }

        [Test]
        public void CirclePoints_GoAllTheWayRound()
        {
            var points = new Vector3[RangeRing.Segments];
            RangeRing.CirclePoints(Vector3.zero, 1f, points);

            Vector3 sum = Vector3.zero;
            foreach (Vector3 point in points)
            {
                sum += point;
            }

            // Evenly spaced round the whole circle, so they balance out at the centre.
            Assert.AreEqual(0f, sum.magnitude, 1e-3f);
        }
    }
}
