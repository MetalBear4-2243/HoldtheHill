using System;
using HoldTheHill.Features.Enemies;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class QueenMantisSummonRuleTests
    {
        private const float Tolerance = 0.0001f;

        [Test]
        public void TheFirstEscort_StandsStraightAboveHer()
        {
            QueenMantisSummonRule.RingOffset(0, 3, 0.7f, out float x, out float y);

            Assert.AreEqual(0f, x, Tolerance);
            Assert.AreEqual(0.7f, y, Tolerance);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        public void EveryEscort_IsOnTheRing(int count)
        {
            for (int i = 0; i < count; i++)
            {
                QueenMantisSummonRule.RingOffset(i, count, 0.7f, out float x, out float y);
                Assert.AreEqual(0.7f, (float)Math.Sqrt(x * x + y * y), Tolerance, $"Escort {i} of {count}.");
            }
        }

        [Test]
        public void Escorts_AreSpreadEvenly_NoTwoInOnePlace()
        {
            QueenMantisSummonRule.RingOffset(0, 2, 1f, out float x0, out float y0);
            QueenMantisSummonRule.RingOffset(1, 2, 1f, out float x1, out float y1);

            Assert.AreEqual(-x0, x1, Tolerance, "Two escorts stand opposite each other.");
            Assert.AreEqual(-y0, y1, Tolerance);
        }

        [Test]
        public void ACountBelowOne_StillGivesAPlace()
        {
            QueenMantisSummonRule.RingOffset(0, 0, 0.5f, out float x, out float y);

            Assert.AreEqual(0f, x, Tolerance);
            Assert.AreEqual(0.5f, y, Tolerance);
        }
    }
}
