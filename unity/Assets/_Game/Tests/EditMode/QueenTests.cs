using NUnit.Framework;
using UnityEngine;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.GameFlow;

namespace HoldTheHill.Tests
{
    public class QueenTests
    {
        private GameObject _queenObject;
        private Queen _queen;
        private GameObject _enemyObject;
        private BasicEnemy _enemy;

        [SetUp]
        public void SetUp()
        {
            _queenObject = new GameObject("TestQueen");
            _queen = _queenObject.AddComponent<Queen>();

            _enemyObject = new GameObject("TestEnemy");
            _enemy = _enemyObject.AddComponent<BasicEnemy>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_queenObject != null)
            {
                Object.DestroyImmediate(_queenObject);
            }

            if (_enemyObject != null)
            {
                Object.DestroyImmediate(_enemyObject);
            }
        }

        [Test]
        public void Health_IsDouble_AndInitializedToMaxHealth()
        {
            Assert.That(_queen.CurrentHealth, Is.TypeOf<double>());
            Assert.That(_queen.CurrentHealth, Is.EqualTo(100.0));
            Assert.That(_queen.MaxHealth, Is.EqualTo(100.0));
            Assert.That(_queen.IsGameOver, Is.False);
        }

        [Test]
        public void TakeDamage_ReducesCurrentHealth_AndFiresHealthChanged()
        {
            double reportedCurrent = 0.0;
            double reportedMax = 0.0;
            _queen.OnHealthChanged += (current, max) =>
            {
                reportedCurrent = current;
                reportedMax = max;
            };

            _queen.TakeDamage(25.5);

            Assert.That(_queen.CurrentHealth, Is.EqualTo(74.5).Within(0.0001));
            Assert.That(reportedCurrent, Is.EqualTo(74.5).Within(0.0001));
            Assert.That(reportedMax, Is.EqualTo(100.0).Within(0.0001));
        }

        [Test]
        public void HandleEnemyReachedDestination_SubtractsEnemyDamageFromQueenHealth()
        {
            _enemy.Damage = 12;
            double initialHealth = _queen.CurrentHealth;

            _queen.HandleEnemyReachedDestination(_enemy);

            Assert.That(_queen.CurrentHealth, Is.EqualTo(initialHealth - 12.0));
        }

        [Test]
        public void EnemyReachingDestination_ViaStaticEvent_ReducesQueenHealth()
        {
            _enemy.Damage = 15;
            double initialHealth = _queen.CurrentHealth;

            // Trigger enemy destination reached
            _enemy.ReachDestination();

            Assert.That(_queen.CurrentHealth, Is.EqualTo(initialHealth - 15.0));
        }

        [Test]
        public void HealthDepletedToZeroOrBelow_FiresGameOverEventExactlyOnce()
        {
            int gameOverCount = 0;
            _queen.OnGameOver += () => gameOverCount++;

            _queen.TakeDamage(150.0);

            Assert.That(_queen.CurrentHealth, Is.EqualTo(0.0));
            Assert.That(_queen.IsGameOver, Is.True);
            Assert.That(gameOverCount, Is.EqualTo(1));

            // Subsequent damage should not trigger GameOver a second time
            _queen.TakeDamage(10.0);
            Assert.That(gameOverCount, Is.EqualTo(1));
        }

        [Test]
        public void Heal_RestoresHealthUpToMaxLimit()
        {
            _queen.TakeDamage(40.0);
            Assert.That(_queen.CurrentHealth, Is.EqualTo(60.0).Within(0.0001));

            _queen.Heal(20.0);
            Assert.That(_queen.CurrentHealth, Is.EqualTo(80.0).Within(0.0001));

            // Overhealing should clamp to MaxHealth
            _queen.Heal(50.0);
            Assert.That(_queen.CurrentHealth, Is.EqualTo(100.0).Within(0.0001));
        }

        [Test]
        public void ResetHealth_RestoresHealthAndResetsGameOver()
        {
            _queen.TakeDamage(100.0);
            Assert.That(_queen.IsGameOver, Is.True);

            _queen.ResetHealth();
            Assert.That(_queen.CurrentHealth, Is.EqualTo(100.0));
            Assert.That(_queen.IsGameOver, Is.False);
        }
    }
}
