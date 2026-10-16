using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using HoldTheHill.Features.Combat;
using HoldTheHill.Features.Enemies;

namespace HoldTheHill.Tests
{
    public class BasicEnemyTests
    {
        private GameObject _enemyObject;
        private BasicEnemy _enemy;

        [SetUp]
        public void SetUp()
        {
            _enemyObject = new GameObject("TestEnemy");
            _enemy = _enemyObject.AddComponent<BasicEnemy>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_enemyObject != null)
            {
                Object.DestroyImmediate(_enemyObject);
            }
        }

        [Test]
        public void Health_IsDouble_CanTakeDoubleDamage()
        {
            // Verify types and double precision
            Assert.That(_enemy.CurrentHealth, Is.TypeOf<double>());
            double initialHealth = _enemy.CurrentHealth;

            double damageAmount = 25.5;
            _enemy.TakeDamage(damageAmount);

            Assert.That(_enemy.CurrentHealth, Is.EqualTo(initialHealth - damageAmount).Within(0.0001));
        }

        [Test]
        public void Damage_IsInt_CanGetAndSet()
        {
            Assert.That(_enemy.Damage, Is.TypeOf<int>());

            _enemy.Damage = 10;
            Assert.That(_enemy.Damage, Is.EqualTo(10));
        }

        [Test]
        public void PathCoordinates_CanBeAssignedAndAccessed()
        {
            var coords = new List<Vector3>
            {
                new Vector3(0, 0, 0),
                new Vector3(2, 0, 0),
                new Vector3(2, 4, 0)
            };

            _enemy.SetPath(coords, teleportToStart: true);

            Assert.That(_enemy.PathCoordinates.Count, Is.EqualTo(3));
            Assert.That(_enemy.PathCoordinates[0], Is.EqualTo(coords[0]));
            Assert.That(_enemy.PathCoordinates[1], Is.EqualTo(coords[1]));
            Assert.That(_enemy.PathCoordinates[2], Is.EqualTo(coords[2]));
            Assert.That(_enemy.transform.position, Is.EqualTo(coords[0]));
        }

        [Test]
        public void PathCoordinates_Vector2Overload_AssignsCorrectly()
        {
            var coords2D = new List<Vector2>
            {
                new Vector2(1.5f, 2.5f),
                new Vector2(3.0f, 4.0f)
            };

            _enemy.SetPath(coords2D, teleportToStart: false);

            Assert.That(_enemy.PathCoordinates.Count, Is.EqualTo(2));
            Assert.That(_enemy.PathCoordinates[0], Is.EqualTo(new Vector3(1.5f, 2.5f, 0f)));
            Assert.That(_enemy.PathCoordinates[1], Is.EqualTo(new Vector3(3.0f, 4.0f, 0f)));
        }

        [Test]
        public void TakeDamage_ExceedingHealth_TriggersDiedEventAndSetsIsDead()
        {
            bool diedTriggered = false;
            _enemy.OnDied += (enemy) => diedTriggered = true;

            _enemy.TakeDamage(_enemy.CurrentHealth + 50.0);

            Assert.That(_enemy.CurrentHealth, Is.EqualTo(0.0));
            Assert.That(_enemy.IsDead, Is.True);
            Assert.That(diedTriggered, Is.True);
        }

        [Test]
        public void Implements_IDamageable_And_IHealthReadable()
        {
            Assert.That(_enemy is IDamageable, Is.True);
            Assert.That(_enemy is IHealthReadable, Is.True);

            IDamageable damageable = _enemy as IDamageable;
            Assert.That(damageable.Transform, Is.EqualTo(_enemy.transform));
            Assert.That(damageable.IsDead, Is.False);

            IHealthReadable healthReadable = _enemy as IHealthReadable;
            Assert.That(healthReadable.CurrentHealth, Is.EqualTo((float)_enemy.CurrentHealth));
            Assert.That(healthReadable.MaxHealth, Is.EqualTo((float)_enemy.MaxHealth));
        }

        [Test]
        public void TakeDamage_DamageInfo_ReducesHealth_AndFiresOnDamaged()
        {
            bool eventFired = false;
            DamageInfo receivedInfo = default;
            float receivedApplied = 0f;

            _enemy.OnDamaged += (enemy, info, applied) =>
            {
                eventFired = true;
                receivedInfo = info;
                receivedApplied = applied;
            };

            DamageInfo hit = new DamageInfo(20f, null, Vector2.zero, DamageType.Physical);
            double expectedHealth = _enemy.CurrentHealth - 20.0;

            _enemy.TakeDamage(hit);

            Assert.That(_enemy.CurrentHealth, Is.EqualTo(expectedHealth).Within(0.0001));
            Assert.That(eventFired, Is.True);
            Assert.That(receivedInfo.Amount, Is.EqualTo(20f));
            Assert.That(receivedApplied, Is.EqualTo(20f).Within(0.0001f));
        }

        [Test]
        public void ApplyStatusEffect_Slow_ModifiesSpeedMultiplier()
        {
            Assert.That(_enemy.SpeedMultiplier, Is.EqualTo(1f));

            StatusEffectData slow = new StatusEffectData
            {
                EffectName = "FrostSlow",
                Duration = 2f,
                SlowMultiplier = 0.6f
            };

            _enemy.ApplyStatusEffect(slow);

            Assert.That(_enemy.SpeedMultiplier, Is.EqualTo(0.6f).Within(0.0001f));

            // Advancing past duration clears the slow
            _enemy.UpdateStatusEffects(2.5f);
            Assert.That(_enemy.SpeedMultiplier, Is.EqualTo(1f));
        }

        [Test]
        public void ApplyStatusEffect_StrongestSlowWins()
        {
            _enemy.ApplyStatusEffect(new StatusEffectData
            {
                EffectName = "LightSlow",
                Duration = 2f,
                SlowMultiplier = 0.8f
            });

            _enemy.ApplyStatusEffect(new StatusEffectData
            {
                EffectName = "HeavySlow",
                Duration = 2f,
                SlowMultiplier = 0.4f
            });

            Assert.That(_enemy.SpeedMultiplier, Is.EqualTo(0.4f).Within(0.0001f));
        }

        [Test]
        public void Knockback_PushesEnemyBackwardAlongPath()
        {
            var coords = new List<Vector3>
            {
                new Vector3(0, 0, 0),
                new Vector3(10, 0, 0)
            };

            _enemy.SetPath(coords, startingWaypointIndex: 1, teleportToStart: false);
            _enemy.transform.position = new Vector3(5, 0, 0);

            _enemy.Knockback(2f);

            Assert.That(_enemy.transform.position.x, Is.EqualTo(3f).Within(0.001f));
        }
    }
}
