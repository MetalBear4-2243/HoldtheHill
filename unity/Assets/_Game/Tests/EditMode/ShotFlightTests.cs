using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.Towers;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// The Bullet Ant's dart is on screen for only two or three frames, so a shot has to face its
    /// flight from the frame it is fired, not one frame later. Uses the real shot prefabs (their
    /// FaceTravel turns the root).
    /// </summary>
    public class ShotFlightTests
    {
        private const string Folder = "Assets/_Game/Features/Towers/Prefabs";

        private GameObject _shot;
        private GameObject _target;

        [TearDown]
        public void TearDown()
        {
            if (_shot != null)
            {
                Object.DestroyImmediate(_shot);
            }

            if (_target != null)
            {
                Object.DestroyImmediate(_target);
            }
        }

        [Test]
        public void StraightShot_FacesItsTarget_FromLaunch()
        {
            Projectile shot = Fire("Projectile_Bullet", new Vector3(-2f, 2f, 0f));

            Assert.That(Mathf.DeltaAngle(shot.transform.eulerAngles.z, 135f), Is.EqualTo(0f).Within(0.5f));
        }

        [Test]
        public void LobbedShot_StartsClimbingTowardItsTarget_FromLaunch()
        {
            Projectile shot = Fire("Projectile_Sap", new Vector3(3f, 0f, 0f));

            float facing = Mathf.DeltaAngle(0f, shot.transform.eulerAngles.z);
            Assert.That(facing, Is.GreaterThan(10f).And.LessThan(90f), "A lob to the right leaves climbing to the right.");
        }

        [Test]
        public void StraightShotHit_ShowsOnTheTargetsNearEdge_NotShortOfIt()
        {
            // At speed 24 a dart counts its hit while still up to a step (0.8) plus the hit radius away.
            Vector3 point = Projectile.HitPoint(new Vector3(-0.9f, 0f, 0f), Vector3.zero, 0.2f);

            Assert.That(point.x, Is.EqualTo(-0.2f).Within(0.0001f));
            Assert.That(point.y, Is.EqualTo(0f).Within(0.0001f));
        }

        [Test]
        public void StraightShotHit_NearerThanTheEdge_ShowsWhereTheShotIs()
        {
            Vector3 point = Projectile.HitPoint(new Vector3(0.1f, 0f, 0f), Vector3.zero, 0.2f);

            Assert.That(point.x, Is.EqualTo(0.1f).Within(0.0001f));
        }

        private Projectile Fire(string prefabName, Vector3 targetAt)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{Folder}/{prefabName}.prefab");
            Assert.That(prefab, Is.Not.Null);
            _shot = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            _target = new GameObject("Target");
            _target.transform.position = targetAt;
            BasicEnemy target = _target.AddComponent<BasicEnemy>();

            Projectile shot = _shot.GetComponent<Projectile>();
            shot.Launch(target, 1);
            return shot;
        }
    }
}
