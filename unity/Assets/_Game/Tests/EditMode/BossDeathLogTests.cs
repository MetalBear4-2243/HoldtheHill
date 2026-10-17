using System.Collections.Generic;
using HoldTheHill.Features.Enemies;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class BossDeathLogTests
    {
        private const ulong Queen = 1;

        private static BossDeathLog NewLog(int capacity = 4, float memory = 10f)
        {
            return new BossDeathLog(Queen, capacity, memory);
        }

        [Test]
        public void AnOrdinaryDeath_Counts()
        {
            BossDeathLog log = NewLog();

            Assert.IsTrue(log.Record(2, 1f, 1f, 3, 0f));
            Assert.AreEqual(1, log.Count);
        }

        [Test]
        public void TheQueensOwnDeath_DoesNotCount()
        {
            BossDeathLog log = NewLog();

            Assert.IsFalse(log.Record(Queen, 0f, 0f, 0, 0f));
            Assert.AreEqual(0, log.Count);
        }

        [Test]
        public void AHuskSheRaised_DoesNotCount_SoItNeverComesBackTwice()
        {
            BossDeathLog log = NewLog();
            log.MarkHusk(7);

            Assert.IsFalse(log.Record(7, 0f, 0f, 0, 0f));
            Assert.AreEqual(0, log.Count);
        }

        [Test]
        public void OnlyTheNewestDeaths_AreKept_UpToTheCapacity()
        {
            BossDeathLog log = NewLog(capacity: 2);
            log.Record(2, 0f, 0f, 0, 0f);
            log.Record(3, 1f, 0f, 0, 1f);
            log.Record(4, 2f, 0f, 0, 2f);

            var taken = new List<DeathRecord>();
            Assert.AreEqual(2, log.Count);
            Assert.AreEqual(2, log.TakeNear(0f, 0f, 100f, 10, 2f, taken));
            Assert.AreEqual(2f, taken[0].X, "Newest first.");
            Assert.AreEqual(1f, taken[1].X);
        }

        [Test]
        public void TakeNear_RaisesOnlyDeathsWithinReach_NewestFirst_AndCapsTheCount()
        {
            BossDeathLog log = NewLog(capacity: 10);
            log.Record(2, 0f, 0f, 1, 0f);      // near
            log.Record(3, 50f, 0f, 2, 1f);     // far
            log.Record(4, 1f, 1f, 3, 2f);      // near
            log.Record(5, 0f, 2f, 4, 3f);      // near, newest

            var taken = new List<DeathRecord>();
            int count = log.TakeNear(0f, 0f, 3f, maxCount: 2, now: 3f, into: taken);

            Assert.AreEqual(2, count);
            Assert.AreEqual(4, taken[0].WaypointIndex, "The newest death within reach comes first.");
            Assert.AreEqual(3, taken[1].WaypointIndex);
            Assert.AreEqual(2, log.Count, "The far one and the one over the cap stay remembered.");
        }

        [Test]
        public void TakenDeaths_AreForgotten()
        {
            BossDeathLog log = NewLog();
            log.Record(2, 0f, 0f, 0, 0f);
            var taken = new List<DeathRecord>();

            Assert.AreEqual(1, log.TakeNear(0f, 0f, 1f, 5, 0f, taken));
            Assert.AreEqual(0, log.TakeNear(0f, 0f, 1f, 5, 0f, taken), "Nothing is raised twice.");
        }

        [Test]
        public void DeathsOlderThanTheMemory_AreDropped_NotRaised()
        {
            BossDeathLog log = NewLog(memory: 5f);
            log.Record(2, 0f, 0f, 0, 0f);
            log.Record(3, 0f, 0f, 0, 4f);

            var taken = new List<DeathRecord>();
            Assert.AreEqual(1, log.TakeNear(0f, 0f, 1f, 5, now: 6f, into: taken));
            Assert.AreEqual(4f, taken[0].Time);
            Assert.AreEqual(0, log.Count, "The stale one was dropped on the way.");
        }

        [Test]
        public void ADeathExactlyAtTheMemoryEdge_StillCounts()
        {
            BossDeathLog log = NewLog(memory: 5f);
            log.Record(2, 0f, 0f, 0, 0f);

            var taken = new List<DeathRecord>();
            Assert.AreEqual(1, log.TakeNear(0f, 0f, 1f, 5, now: 5f, into: taken));
        }
    }
}
