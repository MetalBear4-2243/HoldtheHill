using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using HoldTheHill.Features.Enemies;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// Jason's CentipedeEnemy tests, carried over to the <see cref="CentipedeSplitter"/> ability
    /// beside a plain <see cref="BasicEnemy"/>: the enemy's numbers go through the enemy, the
    /// split's through the splitter, and the children are enemies carrying a splitter of their own.
    /// </summary>
    public class CentipedeSplitterTests
    {
        private GameObject _centipedeObject;
        private BasicEnemy _enemy;
        private CentipedeSplitter _splitter;
        private List<GameObject> _spawnedObjects;

        [SetUp]
        public void SetUp()
        {
            _spawnedObjects = new List<GameObject>();
            _centipedeObject = new GameObject("TestCentipede");
            _enemy = _centipedeObject.AddComponent<BasicEnemy>();
            _splitter = _centipedeObject.AddComponent<CentipedeSplitter>();
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

        private static int SplitsOf(BasicEnemy child)
        {
            return child.GetComponent<CentipedeSplitter>().SplitsRemaining;
        }

        [Test]
        public void SplitsRemaining_DefaultsToMinusOne()
        {
            Assert.That(_splitter.SplitsRemaining, Is.EqualTo(-1));
        }

        [Test]
        public void CanSplit_WithHealthGreaterThanOne_AndMinusOneSplits_ReturnsTrue()
        {
            _enemy.SetMaxHealth(10.0, resetCurrentHealth: true);
            _splitter.SplitsRemaining = -1;

            Assert.That(_splitter.CanSplit(), Is.True);
        }

        [Test]
        public void CanSplit_WhenHealthIsOneOrLess_ReturnsFalse()
        {
            _enemy.SetMaxHealth(1.0, resetCurrentHealth: true);
            _splitter.SplitsRemaining = -1;

            Assert.That(_splitter.CanSplit(), Is.False);

            _enemy.SetMaxHealth(0.5, resetCurrentHealth: true);
            Assert.That(_splitter.CanSplit(), Is.False);
        }

        [Test]
        public void CanSplit_WhenHalvedHealthIsLessThanOne_ReturnsFalse()
        {
            // If MaxHealth is 1.8, half health is 0.9 (< 1.0), so it cannot split
            _enemy.SetMaxHealth(1.8, resetCurrentHealth: true);
            _splitter.SplitsRemaining = -1;

            Assert.That(_splitter.CanSplit(), Is.False);
        }

        [Test]
        public void CanSplit_WhenSplitsRemainingIsZero_ReturnsFalse()
        {
            _enemy.SetMaxHealth(20.0, resetCurrentHealth: true);
            _splitter.SplitsRemaining = 0;

            Assert.That(_splitter.CanSplit(), Is.False);
        }

        [Test]
        public void CanSplit_WhenSplitsRemainingPositive_ReturnsTrue()
        {
            _enemy.SetMaxHealth(20.0, resetCurrentHealth: true);
            _splitter.SplitsRemaining = 2;

            Assert.That(_splitter.CanSplit(), Is.True);
        }

        [Test]
        public void Split_CreatesTwoChildrenWithHalfHealth_AndDecrementsSplitsRemaining()
        {
            _enemy.SetMaxHealth(20.0, resetCurrentHealth: true);
            _splitter.SplitsRemaining = 2;

            List<BasicEnemy> children = _splitter.Split();

            Assert.That(children.Count, Is.EqualTo(2));
            foreach (var child in children)
            {
                _spawnedObjects.Add(child.gameObject);
                Assert.That(child.MaxHealth, Is.EqualTo(10.0).Within(0.0001));
                Assert.That(child.CurrentHealth, Is.EqualTo(10.0).Within(0.0001));
                Assert.That(SplitsOf(child), Is.EqualTo(1));
            }
        }

        [Test]
        public void Split_WithMinusOneSplitsRemaining_PreservesMinusOneInChildren()
        {
            _enemy.SetMaxHealth(8.0, resetCurrentHealth: true);
            _splitter.SplitsRemaining = -1;

            List<BasicEnemy> children = _splitter.Split();

            Assert.That(children.Count, Is.EqualTo(2));
            foreach (var child in children)
            {
                _spawnedObjects.Add(child.gameObject);
                Assert.That(child.MaxHealth, Is.EqualTo(4.0).Within(0.0001));
                Assert.That(SplitsOf(child), Is.EqualTo(-1));
            }
        }

        [Test]
        public void Split_ScalesDownChildrenVisualScale()
        {
            _enemy.SetMaxHealth(10.0, resetCurrentHealth: true);
            _enemy.transform.localScale = Vector3.one;
            _splitter.SizeMultiplier = 0.75f;

            List<BasicEnemy> children = _splitter.Split();

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
            _enemy.SetMaxHealth(10.0, resetCurrentHealth: true);
            var pathPoints = new List<Vector3>
            {
                new Vector3(0, 0, 0),
                new Vector3(5, 0, 0),
                new Vector3(5, 5, 0)
            };
            _enemy.SetPath(pathPoints);
            _enemy.CurrentWaypointIndex = 1;

            List<BasicEnemy> children = _splitter.Split();

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
            _enemy.SetMaxHealth(10.0, resetCurrentHealth: true);
            _splitter.SplitsRemaining = 0;

            bool splitFired = false;
            _splitter.OnSplit += (parent, children) => splitFired = true;

            _enemy.TakeDamage(10.0);

            Assert.That(_enemy.IsDead, Is.True);
            Assert.That(splitFired, Is.False);
        }

        [Test]
        public void TakeDamage_Fatal_WhenOneHp_DoesNotSplit()
        {
            _enemy.SetMaxHealth(1.0, resetCurrentHealth: true);
            _splitter.SplitsRemaining = -1;

            bool splitFired = false;
            _splitter.OnSplit += (parent, children) => splitFired = true;

            _enemy.TakeDamage(1.0);

            Assert.That(_enemy.IsDead, Is.True);
            Assert.That(splitFired, Is.False);
        }

        [Test]
        public void TakeDamage_Fatal_WhenEligible_TriggersSplitEvent()
        {
            _enemy.SetMaxHealth(4.0, resetCurrentHealth: true);
            _splitter.SplitsRemaining = 1;

            bool splitFired = false;
            List<BasicEnemy> spawned = null;
            _splitter.OnSplit += (parent, children) =>
            {
                splitFired = true;
                spawned = children;
                foreach (var child in children)
                {
                    _spawnedObjects.Add(child.gameObject);
                }
            };

            _enemy.TakeDamage(4.0);

            Assert.That(_enemy.IsDead, Is.True);
            Assert.That(splitFired, Is.True);
            Assert.That(spawned, Is.Not.Null);
            Assert.That(spawned.Count, Is.EqualTo(2));
            Assert.That(spawned[0].MaxHealth, Is.EqualTo(2.0).Within(0.0001));
            Assert.That(SplitsOf(spawned[0]), Is.EqualTo(0));
        }

        [Test]
        public void TakeDamage_Fatal_SplitsBeforeTheDeathIsAnnounced()
        {
            _enemy.SetMaxHealth(4.0, resetCurrentHealth: true);
            _splitter.SplitsRemaining = 1;

            var order = new List<string>();
            _splitter.OnSplit += (parent, children) =>
            {
                order.Add("split");
                foreach (var child in children)
                {
                    _spawnedObjects.Add(child.gameObject);
                }
            };
            _enemy.OnDied += enemy => order.Add("died");

            _enemy.TakeDamage(4.0);

            Assert.That(order, Is.EqualTo(new[] { "split", "died" }));
        }

        [Test]
        public void Split_SpawnsFirstChildAtOriginalPosition_AndSecondChildForwardOnPath()
        {
            _enemy.SetMaxHealth(10.0, resetCurrentHealth: true);
            var pathPoints = new List<Vector3>
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(10f, 0f, 0f)
            };
            _enemy.SetPath(pathPoints, startingWaypointIndex: 1, teleportToStart: false);
            _enemy.transform.position = new Vector3(2f, 0f, 0f);

            List<BasicEnemy> children = _splitter.Split();

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
            _enemy.SetPath(pathPoints, startingWaypointIndex: 1, teleportToStart: false);
            _enemy.transform.position = new Vector3(1.5f, 0f, 0f);

            // 0.5 units to waypoint 1 (2, 0, 0), remaining 0.5 units towards waypoint 2 (2, 5, 0)
            Vector3 advancedPos = _splitter.CalculatePositionAlongPath(1.0f, out int targetWaypointIndex);

            Assert.That(advancedPos.x, Is.EqualTo(2f).Within(0.001f));
            Assert.That(advancedPos.y, Is.EqualTo(0.5f).Within(0.001f));
            Assert.That(targetWaypointIndex, Is.EqualTo(2));
        }

        [Test]
        public void Split_NearTheEndOfThePath_KeepsTheSecondChildOffTheLastWaypoint()
        {
            _enemy.SetMaxHealth(10.0, resetCurrentHealth: true);
            var pathPoints = new List<Vector3> { new Vector3(0f, 0f, 0f), new Vector3(10f, 0f, 0f) };
            _enemy.SetPath(pathPoints, startingWaypointIndex: 1, teleportToStart: false);
            _enemy.transform.position = new Vector3(9.5f, 0f, 0f);

            List<BasicEnemy> children = _splitter.Split();
            _spawnedObjects.Add(children[0].gameObject);
            _spawnedObjects.Add(children[1].gameObject);

            // Half a unit from the end: a child put a unit forward would sit on the Queen.
            Assert.That(children[1].transform.position.x, Is.EqualTo(9.5f).Within(0.001f));
            Assert.That(children[1].CurrentWaypointIndex, Is.EqualTo(1));
        }

        [Test]
        public void Split_PastTheLastWaypoint_GivesNoChildAnIndexItCanNeverWalkTo()
        {
            _enemy.SetMaxHealth(10.0, resetCurrentHealth: true);
            var pathPoints = new List<Vector3> { new Vector3(0f, 0f, 0f), new Vector3(10f, 0f, 0f) };
            _enemy.SetPath(pathPoints, startingWaypointIndex: pathPoints.Count, teleportToStart: false);
            _enemy.transform.position = new Vector3(10f, 0f, 0f);

            List<BasicEnemy> children = _splitter.Split();
            foreach (BasicEnemy child in children)
            {
                _spawnedObjects.Add(child.gameObject);
                Assert.That(child.CurrentWaypointIndex, Is.LessThan(pathPoints.Count));
            }
        }

        [Test]
        public void Split_WithASplitPrefabThatHasNoSplitter_ErrorsAndCopiesItselfInstead()
        {
            var notACentipede = new GameObject("PlainEnemy");
            notACentipede.AddComponent<BasicEnemy>();
            _spawnedObjects.Add(notACentipede);

            _enemy.SetMaxHealth(10.0, resetCurrentHealth: true);
            _splitter.SplitPrefab = notACentipede;

            LogAssert.Expect(LogType.Error, new Regex("has no CentipedeSplitter"));
            List<BasicEnemy> children = _splitter.Split();

            Assert.That(children.Count, Is.EqualTo(2));
            foreach (BasicEnemy child in children)
            {
                _spawnedObjects.Add(child.gameObject);
                Assert.That(child, Is.Not.Null);
                Assert.That(child.GetComponent<CentipedeSplitter>(), Is.Not.Null, "A child is a copy of the parent, splitter and all.");
            }
        }

        [Test]
        public void Split_HalvesDamageInChildren()
        {
            _enemy.SetMaxHealth(20.0, resetCurrentHealth: true);
            _enemy.Damage = 20;

            List<BasicEnemy> children = _splitter.Split();

            Assert.That(children.Count, Is.EqualTo(2));
            foreach (var child in children)
            {
                _spawnedObjects.Add(child.gameObject);
                Assert.That(child.Damage, Is.EqualTo(10));
            }
        }

        [Test]
        public void Split_WithDamageTen_HalvesDamageToFive()
        {
            _enemy.SetMaxHealth(20.0, resetCurrentHealth: true);
            _enemy.Damage = 10;

            List<BasicEnemy> children = _splitter.Split();

            Assert.That(children.Count, Is.EqualTo(2));
            foreach (var child in children)
            {
                _spawnedObjects.Add(child.gameObject);
                Assert.That(child.Damage, Is.EqualTo(5));
            }
        }

        [Test]
        public void Split_WithDamageOne_MaintainsMinimumDamageOfOne()
        {
            _enemy.SetMaxHealth(20.0, resetCurrentHealth: true);
            _enemy.Damage = 1;

            List<BasicEnemy> children = _splitter.Split();

            Assert.That(children.Count, Is.EqualTo(2));
            foreach (var child in children)
            {
                _spawnedObjects.Add(child.gameObject);
                Assert.That(child.Damage, Is.EqualTo(1));
            }
        }

        [Test]
        public void Split_WithDamageZero_PreservesZeroDamage()
        {
            _enemy.SetMaxHealth(20.0, resetCurrentHealth: true);
            _enemy.Damage = 0;

            List<BasicEnemy> children = _splitter.Split();

            Assert.That(children.Count, Is.EqualTo(2));
            foreach (var child in children)
            {
                _spawnedObjects.Add(child.gameObject);
                Assert.That(child.Damage, Is.EqualTo(0));
            }
        }
    }
}
