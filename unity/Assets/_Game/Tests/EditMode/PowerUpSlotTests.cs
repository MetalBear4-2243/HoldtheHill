using HoldTheHill.Features.PowerUps;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class PowerUpSlotTests
    {
        [Test]
        public void StartsEmpty_AndStoresOne()
        {
            var slot = new PowerUpSlot();
            int changes = 0;
            slot.Changed += () => changes++;

            Assert.IsTrue(slot.IsEmpty);
            Assert.IsTrue(slot.TryStore(PowerUpKind.RoyalGift));
            Assert.AreEqual(PowerUpKind.RoyalGift, slot.Held);
            Assert.IsFalse(slot.IsEmpty);
            Assert.AreEqual(1, changes);
        }

        [Test]
        public void AFullSlot_RefusesASecondDrop_AndKeepsTheFirst()
        {
            var slot = new PowerUpSlot();
            slot.TryStore(PowerUpKind.SwarmFrenzy);
            int changes = 0;
            slot.Changed += () => changes++;

            Assert.IsFalse(slot.TryStore(PowerUpKind.RoyalGift));
            Assert.AreEqual(PowerUpKind.SwarmFrenzy, slot.Held);
            Assert.AreEqual(0, changes, "a refused drop is not a change");
        }

        [Test]
        public void Take_EmptiesTheSlot_AndHandsBackWhatItHeld()
        {
            var slot = new PowerUpSlot();
            slot.TryStore(PowerUpKind.SwarmFrenzy);
            int changes = 0;
            slot.Changed += () => changes++;

            Assert.AreEqual(PowerUpKind.SwarmFrenzy, slot.Take());
            Assert.IsTrue(slot.IsEmpty);
            Assert.AreEqual(1, changes);

            Assert.IsNull(slot.Take(), "taking from an empty slot gives nothing");
            Assert.AreEqual(1, changes, "and is not a change either");
        }
    }
}
