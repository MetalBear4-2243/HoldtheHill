using HoldTheHill.Core.Animation;
using HoldTheHill.Features.Enemies;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class SpriteRowTests
    {
        [Test]
        public void FrameAt_PlaysTheRowOnce()
        {
            Assert.AreEqual(0, SpriteRow.FrameAt(0f, 0.08f, 2));
            Assert.AreEqual(1, SpriteRow.FrameAt(0.1f, 0.08f, 2));
            Assert.AreEqual(-1, SpriteRow.FrameAt(0.17f, 0.08f, 2), "Over after two frames.");
            Assert.AreEqual(-1, SpriteRow.FrameAt(0f, 0.08f, 0), "No frames, nothing to show.");
        }

        [Test]
        public void Length_IsEveryFrameOnce()
        {
            Assert.AreEqual(0.56f, SpriteRow.Length(0.08f, 7), 1e-6f);
            Assert.AreEqual(0f, SpriteRow.Length(0.08f, 0));
        }

        [Test]
        public void ADeath_StaysSolidThroughTheRow_ThenFades()
        {
            float row = SpriteRow.Length(0.08f, 7);
            Assert.AreEqual(1f, EnemyDeathEffect.Opacity(0f, row, 0.35f));
            Assert.AreEqual(1f, EnemyDeathEffect.Opacity(row, row, 0.35f));
            Assert.AreEqual(0.5f, EnemyDeathEffect.Opacity(row + 0.175f, row, 0.35f), 1e-4f);
            Assert.AreEqual(0f, EnemyDeathEffect.Opacity(row + 0.35f, row, 0.35f), 1e-4f);
            Assert.AreEqual(0f, EnemyDeathEffect.Opacity(row + 0.01f, row, 0f), "No fade: gone after the row.");
        }
    }
}
