using System.Collections.Generic;
using HoldTheHill.Features.Economy;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class HoneydewTests
    {
        [Test]
        public void StartsAtTheStartingAmount_NeverBelowZero()
        {
            Assert.AreEqual(300, new Honeydew(300).Amount);
            Assert.AreEqual(0, new Honeydew(-50).Amount);
        }

        [Test]
        public void Earn_Adds_AndIgnoresZeroOrLess()
        {
            var honeydew = new Honeydew(100);

            honeydew.Earn(25);
            honeydew.Earn(0);
            honeydew.Earn(-10);

            Assert.AreEqual(125, honeydew.Amount);
        }

        [Test]
        public void Spending_FailsAndChangesNothing_WhenShort()
        {
            var honeydew = new Honeydew(100);

            Assert.IsTrue(honeydew.CanAfford(100));
            Assert.IsTrue(honeydew.TrySpend(100));
            Assert.IsFalse(honeydew.CanAfford(1));
            Assert.IsFalse(honeydew.TrySpend(1));
            Assert.AreEqual(0, honeydew.Amount);
            Assert.IsTrue(honeydew.TrySpend(0), "zero costs nothing and always succeeds");
        }

        [Test]
        public void Changed_IsRaisedWithTheNewAmount_OnlyWhenItChanges()
        {
            var honeydew = new Honeydew(10);
            var seen = new List<int>();
            honeydew.Changed += amount => seen.Add(amount);

            honeydew.Earn(5);
            honeydew.TrySpend(999);
            honeydew.Earn(0);
            honeydew.Set(15);
            honeydew.Set(-3);

            Assert.AreEqual(new[] { 15, 0 }, seen, "a failed spend, a no-op and setting the same value report nothing");
        }
    }
}
