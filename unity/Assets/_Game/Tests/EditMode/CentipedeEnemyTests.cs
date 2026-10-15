using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using HoldTheHill.Features.Enemies;

namespace HoldTheHill.Tests
{
    public class CentipedeEnemyTests
    {
        private GameObject _centipedeObject;
        private CentipedeEnemy _centipede;
        private List<GameObject> _spawnedObjects;

        [SetUp]
        public void SetUp()
        {
            _spawnedObjects = new List<GameObject>();
            _centipedeObject = new GameObject("TestCentipede");
            _centipede = _centipedeObject.AddComponent<CentipedeEnemy>();
            _spawnedObjects.Add(_centipedeObject);
        }

        [TearDown]
        public void TearDown()
        {
            if (_spawnedObjects != null)
            {
                foreach (var obj in _spawnedObjects)
                {
                    if (obj != null)
                    {
                        Object.DestroyImmediate(obj);
                    }
                }
                _spawnedObjects.Clear();
            }
        }

        [Test]
        public void SplitsRemaining_DefaultsToMinusOne()
        {
            Assert.That(_centipede.SplitsRemaining, Is.EqualTo(-1));
        }

        [Test]
        public void CanSplit_WithHealthGreaterThanOne_AndMinusOneSplits_ReturnsTrue()
        {
            _centipede.SetMaxHealth(10.0, resetCurrentHealth: true);
            _centipede.SplitsRemaining = -1;

            Assert.That(_centipede.CanSplit(), Is.True);
        }

        [Test]
        public void CanSplit_WhenHealthIsOneOrLess_ReturnsFalse()
        {
            _centipede.SetMaxHealth(1.0, resetCurrentHealth: true);
            _centipede.SplitsRemaining = -1;

            Assert.That(_centipede.CanSplit(), Is.False);

            _centipede.SetMaxHealth(0.5, resetCurrentHealth: true);
            Assert.That(_centipede.CanSplit(), Is.False);
        }

        [Test]
        public void CanSplit_WhenHalvedHealthIsLessThanOne_ReturnsFalse()
        {
            // If MaxHealth is 1.8, half health is 0.9 (< 1.0), so it cannot split
            _centipede.SetMaxHealth(1.8, resetCurrentHealth: true);
            _centipede.SplitsRemaining = -1;

            Assert.That(_centipede.CanSplit(), Is.False);
        }

        [Test]
        public void CanSplit_WhenSplitsRemainingIsZero_ReturnsFalse()
        {
            _centipede.SetMaxHealth(20.0, resetCurrentHealth: true);
            _centipede.SplitsRemaining = 0;

            Assert.That(_centipede.CanSplit(), Is.False);
        }

        [Test]
        public void CanSplit_WhenSplitsRemainingPositive_ReturnsTrue()
        {
            _centipede.SetMaxHealth(20.0, resetCurrentHealth: true);
            _centipede.SplitsRemaining = 2;

            Assert.That(_centipede.CanSplit(), Is.True);
        }

        [Test]
        public void Split_CreatesTwoChildrenWithHalfHealth_AndDecrementsSplitsRemaining()
        {
            _centipede.SetMaxHealth(20.0, resetCurrentHealth: true);
            _centipede.SplitsRemaining = 2;

            List<CentipedeEnemy> children = _centipede.Split();

            Assert.That(children.Count, Is.EqualTo(2));
            foreach (var child in children)
            {
                _spawnedObjects.Add(child.gameObject);
                Assert.That(child.MaxHealth, Is.EqualTo(10.0).Within(0.0001));
                Assert.That(child.CurrentHealth, Is.EqualTo(10.0).Within(0.0001));
                Assert.That(child.SplitsRemaining, Is.EqualTo(1));
            }
        }

        [Test]
        public void Split_WithMinusOneSplitsRemaining_PreservesMinusOneInChildren()
        {
            _centipede.SetMaxHealth(8.0, resetCurrentHealth: true);
            _centipede.SplitsRemaining = -1;

            List<CentipedeEnemy> children = _centipede.Split();

            Assert.That(children.Count, Is.EqualTo(2));
            foreach (var child in children)
            {
                _spawnedObjects.Add(child.gameObject);
                Assert.That(child.MaxHealth, Is.EqualTo(4.0).Within(0.0001));
                Assert.That(child.SplitsRemaining, Is.EqualTo(-1));
            }
        }

        [Test]
        public void Split_ScalesDownChildrenVisualScale()
        {
            _centipede.SetMaxHealth(10.0, resetCurrentHealth: true);
            _centipede.transform.localScale = Vector3.one;
            _centipede.SizeMultiplier = 0.75f;

            List<CentipedeEnemy> children = _centipede.Split();

            Assert.That(children.Count, Is.EqualTo(2));
            foreach (var child in children)
            {
                _spawnedObjects.Add(child.gameObject);
                Assert.That(child.transform.localScale.x, Is.EqualTo(0.75f).Within(0.001f));
                Assert.That(child.transform.localScale.y, Is.EqualTo(0.75f).Within(0.001f));
            }
        }

        [Test]
        public void Split_ChildrenInheritPathAndCurrentWaypointIndex()
        {
            _centipede.SetMaxHealth(10.0, resetCurrentHealth: true);
            var pathPoints = new List<Vector3>
            {
                new Vector3(0, 0, 0),
                new Vector3(5, 0, 0),
                new Vector3(5, 5, 0)
            };
            _centipede.SetPath(pathPoints);
            _centipede.CurrentWaypointIndex = 1;

            List<CentipedeEnemy> children = _centipede.Split();

            Assert.That(children.Count, Is.EqualTo(2));
            foreach (var child in children)
            {
                _spawnedObjects.Add(child.gameObject);
                Assert.That(child.PathCoordinates.Count, Is.EqualTo(3));
                Assert.That(child.CurrentWaypointIndex, Is.EqualTo(1));
            }
        }

        [Test]
        public void TakeDamage_Fatal_WhenSplitsRemainingIsZero_DoesNotSplit()
        {
            _centipede.SetMaxHealth(10.0, resetCurrentHealth: true);
            _centipede.SplitsRemaining = 0;

            bool splitFired = false;
            _centipede.OnSplit += (parent, children) => splitFired = true;

            _centipede.TakeDamage(10.0);

            Assert.That(_centipede.IsDead, Is.True);
            Assert.That(splitFired, Is.False);
        }

        [Test]
        public void TakeDamage_Fatal_WhenOneHp_DoesNotSplit()
        {
            _centipede.SetMaxHealth(1.0, resetCurrentHealth: true);
            _centipede.SplitsRemaining = -1;

            bool splitFired = false;
            _centipede.OnSplit += (parent, children) => splitFired = true;

            _centipede.TakeDamage(1.0);

            Assert.That(_centipede.IsDead, Is.True);
            Assert.That(splitFired, Is.False);
        }

        [Test]
        public void TakeDamage_Fatal_WhenEligible_TriggersSplitEvent()
        {
            _centipede.SetMaxHealth(4.0, resetCurrentHealth: true);
            _centipede.SplitsRemaining = 1;

            bool splitFired = false;
            List<CentipedeEnemy> spawned = null;
            _centipede.OnSplit += (parent, children) =>
            {
                splitFired = true;
                spawned = children;
                foreach (var child in children)
                {
                    _spawnedObjects.Add(child.gameObject);
                }
            };

            _centipede.TakeDamage(4.0);

            Assert.That(_centipede.IsDead, Is.True);
            Assert.That(splitFired, Is.True);
            Assert.That(spawned, Is.Not.Null);
            Assert.That(spawned.Count, Is.EqualTo(2));
            Assert.That(spawned[0].MaxHealth, Is.EqualTo(2.0).Within(0.0001));
            Assert.That(spawned[0].SplitsRemaining, Is.EqualTo(0));
        }

        [Test]
        public void Split_SpawnsFirstChildAtOriginalPosition_AndSecondChildForwardOnPath()
        {
            _centipede.SetMaxHealth(10.0, resetCurrentHealth: true);
            var pathPoints = new List<Vector3>
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(10f, 0f, 0f)
            };
            _centipede.SetPath(pathPoints, startingWaypointIndex: 1, teleportToStart: false);
            _centipede.transform.position = new Vector3(2f, 0f, 0f);

            List<CentipedeEnemy> children = _centipede.Split();

            Assert.That(children.Count, Is.EqualTo(2));
            _spawnedObjects.Add(children[0].gameObject);
            _spawnedObjects.Add(children[1].gameObject);

            // First child at original position (2, 0, 0)
            Assert.That(children[0].transform.position.x, Is.EqualTo(2f).Within(0.001f));
            Assert.That(children[0].transform.position.y, Is.EqualTo(0f).Within(0.001f));
            Assert.That(children[0].CurrentWaypointIndex, Is.EqualTo(1));

            // Second child 1 unit forward on the path (3, 0, 0)
            Assert.That(children[1].transform.position.x, Is.EqualTo(3f).Within(0.001f));
            Assert.That(children[1].transform.position.y, Is.EqualTo(0f).Within(0.001f));
            Assert.That(children[1].CurrentWaypointIndex, Is.EqualTo(1));
        }

        [Test]
        public void CalculatePositionAlongPath_AdvancesAlongCornersAndUpdatesWaypointIndex()
        {
            var pathPoints = new List<Vector3>
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(2f, 0f, 0f),
                new Vector3(2f, 5f, 0f)
            };
            _centipede.SetPath(pathPoints, startingWaypointIndex: 1, teleportToStart: false);
            _centipede.transform.position = new Vector3(1.5f, 0f, 0f);

            // 0.5 units to waypoint 1 (2, 0, 0), remaining 0.5 units towards waypoint 2 (2, 5, 0)
            Vector3 advancedPos = _centipede.CalculatePositionAlongPath(1.0f, out int targetWaypointIndex);

            Assert.That(advancedPos.x, Is.EqualTo(2f).Within(0.001f));
            Assert.That(advancedPos.y, Is.EqualTo(0.5f).Within(0.001f));
            Assert.That(targetWaypointIndex, Is.EqualTo(2));
        }
    }
}

