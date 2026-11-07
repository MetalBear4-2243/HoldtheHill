using HoldTheHill.Features.Combat;
using HoldTheHill.Features.Enemies;
using NUnit.Framework;
using UnityEngine;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// The shield as an ability beside a plain <see cref="BasicEnemy"/>: it sees each hit first.
    /// The shield rule itself is covered by <see cref="EnemyShieldTests"/>.
    /// </summary>
    public class RhinoBeetleShieldTests
    {
        private GameObject _object;
        private BasicEnemy _enemy;
        private RhinoBeetleShield _shield;

        [SetUp]
        public void SetUp()
        {
            _object = new GameObject("TestRhinoBeetle");
            _enemy = _object.AddComponent<BasicEnemy>();
            _shield = _object.AddComponent<RhinoBeetleShield>();
            _enemy.SetMaxHealth(60.0, resetCurrentHealth: true);
        }

        [TearDown]
        public void TearDown()
        {
            if (_object != null)
            {
                Object.DestroyImmediate(_object);
            }
        }

        [Test]
        public void TheShield_StartsFull_AtThePrefabsCapacity()
        {
            Assert.That(_shield.Shield.Capacity, Is.EqualTo(40.0));
            Assert.That(_shield.Shield.Current, Is.EqualTo(40.0));
        }

        [Test]
        public void AHitSmallerThanTheShield_LeavesHealthAlone()
        {
            _enemy.TakeDamage(30.0);

            Assert.That(_enemy.CurrentHealth, Is.EqualTo(60.0));
            Assert.That(_shield.Shield.Current, Is.EqualTo(10.0));
            Assert.That(_enemy.IsDead, Is.False);
        }

        [Test]
        public void AHitSoakedWhole_RaisesNoDamageEvent()
        {
            int damaged = 0;
            _enemy.OnDamaged += (enemy, hit, applied) => damaged++;

            _enemy.TakeDamage(30.0);

            Assert.That(damaged, Is.EqualTo(0), "A hit that never reached health is no hit: no flinch, no tower credit.");
        }

        [Test]
        public void OnlyTheExcess_ReachesHealth_AndIsWhatTheEventReports()
        {
            float applied = -1f;
            _enemy.OnDamaged += (enemy, hit, amount) => applied = amount;

            _enemy.TakeDamage(50.0);

            Assert.That(_shield.Shield.Current, Is.EqualTo(0.0));
            Assert.That(_enemy.CurrentHealth, Is.EqualTo(50.0));
            Assert.That(applied, Is.EqualTo(10f));
        }

        [Test]
        public void AHitThroughDamageInfo_GoesThroughTheShieldToo()
        {
            _enemy.TakeDamage(new DamageInfo(45f, null, Vector3.zero));

            Assert.That(_shield.Shield.Current, Is.EqualTo(0.0));
            Assert.That(_enemy.CurrentHealth, Is.EqualTo(55.0));
        }

        [Test]
        public void WithTheShieldDown_EveryHitLands_UntilItDies()
        {
            _enemy.TakeDamage(40.0);
            Assert.That(_shield.Shield.IsUp, Is.False);

            _enemy.TakeDamage(60.0);

            Assert.That(_enemy.IsDead, Is.True);
        }
    }
}
