using HoldTheHill.Features.Towers;
using NUnit.Framework;
using UnityEngine;

namespace HoldTheHill.Tests
{
    /// <summary>The rules behind the tower roles: the flamethrower's cone, the Crazy Ant's hits in a row and poison stacks, the sap puddle's fade.</summary>
    public class TowerRolesTests
    {
        [Test]
        public void Cone_TakesWhatIsInFront_AndLeavesWhatIsBesideOrBehind()
        {
            Vector3 tower = Vector3.zero;
            Vector3 aim = Vector3.right;

            Assert.IsTrue(Tower.InCone(tower, aim, new Vector3(1.5f, 0f, 0f), 70f), "straight ahead");
            Assert.IsTrue(Tower.InCone(tower, aim, new Vector3(1f, 0.6f, 0f), 70f), "31 degrees off, inside a 70 degree cone");
            Assert.IsFalse(Tower.InCone(tower, aim, new Vector3(1f, 0.8f, 0f), 70f), "39 degrees off, outside it");
            Assert.IsFalse(Tower.InCone(tower, aim, new Vector3(0f, 1f, 0f), 70f), "beside");
            Assert.IsFalse(Tower.InCone(tower, aim, new Vector3(-1f, 0f, 0f), 70f), "behind");
        }

        [Test]
        public void Cone_OfNoneOrAllRound_TakesEverything_AsTheAntsSwing()
        {
            Assert.IsTrue(Tower.InCone(Vector3.zero, Vector3.right, new Vector3(-1f, 0f, 0f), 0f));
            Assert.IsTrue(Tower.InCone(Vector3.zero, Vector3.right, new Vector3(-1f, 0f, 0f), 360f));
        }

        [Test]
        public void Cone_IgnoresDepth_AndTakesAnEnemyOnTheTower()
        {
            Assert.IsTrue(Tower.InCone(Vector3.zero, new Vector3(1f, 0f, 5f), new Vector3(1f, 0f, -3f), 30f), "z plays no part");
            Assert.IsTrue(Tower.InCone(Vector3.zero, Vector3.right, Vector3.zero, 30f), "standing on the tower");
        }

        [Test]
        public void TwoHitsInARow_AddAStack_AndTheCountStartsAgain()
        {
            int hits = PoisonOnHits.NextHits(lastSwing: 0, hits: 0, swing: 5, hitsPerStack: 2, out bool stack);
            Assert.AreEqual(1, hits);
            Assert.IsFalse(stack, "the first hit only counts");

            hits = PoisonOnHits.NextHits(lastSwing: 5, hits: hits, swing: 6, hitsPerStack: 2, out stack);
            Assert.IsTrue(stack, "the second hit in a row adds a stack");
            Assert.AreEqual(0, hits);

            hits = PoisonOnHits.NextHits(lastSwing: 6, hits: hits, swing: 7, hitsPerStack: 2, out stack);
            Assert.IsFalse(stack, "the third starts a new pair");
            Assert.AreEqual(1, hits);
        }

        [Test]
        public void AMissedSwing_BreaksTheRun()
        {
            int hits = PoisonOnHits.NextHits(lastSwing: 3, hits: 1, swing: 5, hitsPerStack: 2, out bool stack);
            Assert.IsFalse(stack, "swing 4 missed, so swing 5 is a first hit again");
            Assert.AreEqual(1, hits);
        }

        [Test]
        public void Stacks_GoUpByOne_ToTheMost()
        {
            Assert.AreEqual(1, PoisonStacks.NextStacks(0, 5));
            Assert.AreEqual(5, PoisonStacks.NextStacks(4, 5));
            Assert.AreEqual(5, PoisonStacks.NextStacks(5, 5));
            Assert.AreEqual(1, PoisonStacks.NextStacks(0, 0), "a cap below one still allows one stack");
        }

        [Test]
        public void Puddle_IsWhole_ThenFadesOverItsLastMoments()
        {
            Assert.AreEqual(1f, SapPuddle.FadeAlpha(0f, 4f, 0.6f), 1e-5f);
            Assert.AreEqual(1f, SapPuddle.FadeAlpha(3.4f, 4f, 0.6f), 1e-5f);
            Assert.AreEqual(0.5f, SapPuddle.FadeAlpha(3.7f, 4f, 0.6f), 1e-5f);
            Assert.AreEqual(0f, SapPuddle.FadeAlpha(4f, 4f, 0.6f), 1e-5f);
            Assert.AreEqual(1f, SapPuddle.FadeAlpha(3.9f, 4f, 0f), 1e-5f, "no fade: whole until it goes");
        }
    }
}
