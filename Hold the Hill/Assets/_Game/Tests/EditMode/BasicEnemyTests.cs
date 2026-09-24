using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
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
    }
}
