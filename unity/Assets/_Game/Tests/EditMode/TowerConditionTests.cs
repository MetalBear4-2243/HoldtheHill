using HoldTheHill.Features.Towers;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class TowerConditionTests
    {
        [Test]
        public void AFreshTower_ActsAndItsHitsLand()
        {
            var condition = new TowerCondition();

            Assert.IsTrue(condition.CanAct);
            Assert.IsTrue(condition.HitsLand);
            Assert.IsFalse(condition.IsStunned);
            Assert.IsFalse(condition.IsMissing);
        }

        [Test]
        public void Burrowed_DoesNotAct_ButIsNotStunned()
        {
            var condition = new TowerCondition { Burrowed = true };

            Assert.IsFalse(condition.CanAct);
            Assert.IsTrue(condition.HitsLand, "burrowing is not a hazard; its hits would land if it fired");
            Assert.IsFalse(condition.IsStunned);

            condition.Burrowed = false;
            Assert.IsTrue(condition.CanAct);
        }

        [Test]
        public void Stunned_DoesNotAct_UntilTheTimeHasPassed()
        {
            var condition = new TowerCondition();
            condition.Stun(2f);

            Assert.IsFalse(condition.CanAct);
            Assert.IsFalse(condition.Tick(1.5f), "nothing ended yet");
            Assert.IsFalse(condition.CanAct);
            Assert.IsTrue(condition.Tick(0.6f), "the stun ended this tick");
            Assert.IsTrue(condition.CanAct);
            Assert.AreEqual(0f, condition.StunnedFor, "timers never go negative");
        }

        [Test]
        public void Missing_StillActs_ButItsHitsDoNothing()
        {
            var condition = new TowerCondition();
            condition.Miss(3f);

            Assert.IsTrue(condition.CanAct, "a missing tower keeps firing");
            Assert.IsFalse(condition.HitsLand);
            condition.Tick(3f);
            Assert.IsTrue(condition.HitsLand);
        }

        [Test]
        public void AShorterSpell_NeverCutsALongerOneShort()
        {
            var condition = new TowerCondition();
            condition.Stun(5f);
            condition.Stun(1f);
            condition.Miss(4f);
            condition.Miss(2f);

            Assert.AreEqual(5f, condition.StunnedFor);
            Assert.AreEqual(4f, condition.MissingFor);
            condition.Stun(0f);
            Assert.AreEqual(5f, condition.StunnedFor, "a zero stun changes nothing");
        }

        [Test]
        public void BurrowAll_BurrowsWhileAnyTowerIsUp_AndRaisesOnlyWhenAllAreDown()
        {
            Assert.IsTrue(TowerCondition.BurrowAllNext(burrowedCount: 0, towerCount: 4));
            Assert.IsTrue(TowerCondition.BurrowAllNext(burrowedCount: 3, towerCount: 4));
            Assert.IsFalse(TowerCondition.BurrowAllNext(burrowedCount: 4, towerCount: 4));
            Assert.IsFalse(TowerCondition.BurrowAllNext(burrowedCount: 0, towerCount: 0), "no towers: nothing to burrow");
        }
    }
}
