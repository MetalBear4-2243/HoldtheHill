using System.Collections.Generic;
using HoldTheHill.Features.Towers;
using NUnit.Framework;
using UnityEngine;

namespace HoldTheHill.Tests
{
    public class TargetPickerTests
    {
        // Three enemies in range: A is far along the path but far from the tower,
        // B is nearest the tower, C has just arrived.
        private static readonly List<TargetPicker.Candidate> Candidates = new List<TargetPicker.Candidate>
        {
            new TargetPicker.Candidate { PathProgress = 9f, SqrDistance = 4f },
            new TargetPicker.Candidate { PathProgress = 5f, SqrDistance = 1f },
            new TargetPicker.Candidate { PathProgress = 1f, SqrDistance = 2f },
        };

        [Test]
        public void First_PicksTheEnemyFurthestAlongThePath()
        {
            Assert.AreEqual(0, TargetPicker.Pick(Candidates, TargetingPriority.First));
        }

        [Test]
        public void Last_PicksTheEnemyLeastFarAlongThePath()
        {
            Assert.AreEqual(2, TargetPicker.Pick(Candidates, TargetingPriority.Last));
        }

        [Test]
        public void Closest_PicksTheEnemyNearestTheTower()
        {
            Assert.AreEqual(1, TargetPicker.Pick(Candidates, TargetingPriority.Closest));
        }

        [Test]
        public void NoCandidates_PicksNothing_AndTiesGoToTheEarlierOne()
        {
            Assert.AreEqual(-1, TargetPicker.Pick(new List<TargetPicker.Candidate>(), TargetingPriority.First));

            var tied = new List<TargetPicker.Candidate>
            {
                new TargetPicker.Candidate { PathProgress = 3f, SqrDistance = 3f },
                new TargetPicker.Candidate { PathProgress = 3f, SqrDistance = 3f },
            };
            Assert.AreEqual(0, TargetPicker.Pick(tied, TargetingPriority.Closest));
        }

        [Test]
        public void CanReach_OnlyAStraightShotReachesAFlyer_EverythingReachesTheGround()
        {
            // The wasp rule.
            Assert.IsFalse(TargetPicker.CanReach(enemyFlying: true, hasProjectile: false, lobbed: false), "Melee skips a flyer.");
            Assert.IsFalse(TargetPicker.CanReach(enemyFlying: true, hasProjectile: true, lobbed: true), "A lobbed shot lands on the ground.");
            Assert.IsTrue(TargetPicker.CanReach(enemyFlying: true, hasProjectile: true, lobbed: false), "A straight shot hits a flyer.");

            Assert.IsTrue(TargetPicker.CanReach(enemyFlying: false, hasProjectile: false, lobbed: false));
            Assert.IsTrue(TargetPicker.CanReach(enemyFlying: false, hasProjectile: true, lobbed: true));
            Assert.IsTrue(TargetPicker.CanReach(enemyFlying: false, hasProjectile: true, lobbed: false));
        }

        [Test]
        public void PathProgress_IsDistanceWalked_FromWaypointIndexAndPosition()
        {
            // An L-shaped path: 4 units right, then 3 units up.
            var path = new List<Vector3> { new Vector3(0f, 0f), new Vector3(4f, 0f), new Vector3(4f, 3f) };

            // Heading for the first corner, 1 unit short of it.
            Assert.AreEqual(3f, TargetPicker.PathProgress(path, 1, new Vector3(3f, 0f)), 0.001f);
            // Round the corner, 1 unit up the second leg.
            Assert.AreEqual(5f, TargetPicker.PathProgress(path, 2, new Vector3(4f, 1f)), 0.001f);
            // Past the end counts as the whole path.
            Assert.AreEqual(7f, TargetPicker.PathProgress(path, 3, new Vector3(9f, 9f)), 0.001f);
            // Not yet at the first waypoint reads negative, so it sorts behind everyone on the path.
            Assert.Less(TargetPicker.PathProgress(path, 0, new Vector3(-2f, 0f)), 0f);
            // No path at all is zero, never a throw.
            Assert.AreEqual(0f, TargetPicker.PathProgress(new List<Vector3>(), 0, Vector3.zero));
        }
    }
}
