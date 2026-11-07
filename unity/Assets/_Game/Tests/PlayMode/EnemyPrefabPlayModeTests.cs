#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using HoldTheHill.Features.Economy;
using HoldTheHill.Features.Enemies;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// Every enemy prefab, alive: spawned in an empty scene it takes its numbers from its data
    /// asset, walks its road, and dies to a lethal hit (the Centipede leaving its two halves).
    /// Editor only: the prefabs are loaded through the AssetDatabase.
    /// </summary>
    public class EnemyPrefabPlayModeTests
    {
        private const string PrefabFolder = "Assets/_Game/Features/Enemies/Prefabs";

        private static readonly string[] Enemies =
        {
            "Basic", "Scarab", "MantisSpawn", "Centipede", "RhinoBeetle", "RollyPolly", "Silverfish", "Wasp", "QueenMantis",
        };

        private Scene _scene;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _scene = SceneManager.CreateScene("EnemyPrefabPlayModeTests");
            SceneManager.SetActiveScene(_scene);
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            // The scene takes every enemy, half, body and stinger the test left with it.
            yield return SceneManager.UnloadSceneAsync(_scene);
        }

        private static BasicEnemy Spawn(string enemy)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabFolder}/Enemy_{enemy}.prefab");
            Assert.That(prefab, Is.Not.Null, $"Enemy_{enemy}.prefab is missing.");
            var instance = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            instance.name = $"Enemy_{enemy}";
            var basic = instance.GetComponent<BasicEnemy>();
            Assert.That(basic, Is.Not.Null, $"Enemy_{enemy} has no BasicEnemy.");
            return basic;
        }

        private static readonly List<Vector3> Road = new List<Vector3> { new Vector3(0f, 0f, 0f), new Vector3(20f, 0f, 0f) };

        [UnityTest]
        public IEnumerator EachEnemy_SpawnsWithItsDataAssetsNumbers()
        {
            var failures = new List<string>();
            foreach (string enemy in Enemies)
            {
                BasicEnemy basic = Spawn(enemy);
                yield return null;

                EnemyData data = basic.Data;
                if (data == null)
                {
                    failures.Add($"{enemy}: no data asset");
                    continue;
                }

                Check(failures, enemy, "health", basic.MaxHealth, data.MaxHealth);
                Check(failures, enemy, "current health", basic.CurrentHealth, data.MaxHealth);
                Check(failures, enemy, "damage", basic.Damage, data.Damage);
                Check(failures, enemy, "speed", basic.MoveSpeed, data.MoveSpeed);
                Check(failures, enemy, "tolerance", basic.WaypointTolerance, data.WaypointTolerance);
                Check(failures, enemy, "flying", basic.IsFlying, data.IsFlying);
                Check(failures, enemy, "bounty", basic.GetComponent<Bounty>().Amount, data.Honeydew);
            }

            Assert.That(failures, Is.Empty, string.Join("\n", failures));
        }

        [UnityTest]
        public IEnumerator EachEnemy_WalksItsRoad()
        {
            var spawned = new List<BasicEnemy>();
            foreach (string enemy in Enemies)
            {
                BasicEnemy basic = Spawn(enemy);
                basic.SetPath(Road, teleportToStart: true);
                spawned.Add(basic);
            }

            // Batch-mode frames are tiny, so wait on the clock, not on frames.
            yield return new WaitForSeconds(0.5f);

            var failures = new List<string>();
            foreach (BasicEnemy basic in spawned)
            {
                if (basic == null)
                {
                    failures.Add("an enemy vanished while walking");
                    continue;
                }

                float expected = basic.MoveSpeed * 0.5f;
                if (basic.transform.position.x < expected * 0.5f || basic.transform.position.x > expected * 1.5f)
                {
                    failures.Add($"{basic.name}: walked {basic.transform.position.x:0.00} in half a second at speed {basic.MoveSpeed} (expected about {expected:0.00})");
                }
                if (basic.DistanceWalked <= 0f)
                {
                    failures.Add($"{basic.name}: its odometer did not move");
                }
            }

            Assert.That(failures, Is.Empty, string.Join("\n", failures));
        }

        [UnityTest]
        public IEnumerator EachEnemy_DiesToALethalHit()
        {
            var failures = new List<string>();
            foreach (string enemy in Enemies)
            {
                BasicEnemy basic = Spawn(enemy);
                basic.SetPath(Road, teleportToStart: true);
                yield return null;

                int died = 0;
                basic.OnDied += e => died++;

                // Enough for any shield or curl in the way.
                basic.TakeDamage(10000.0);

                if (!basic.IsDead)
                {
                    failures.Add($"{enemy}: not dead after a lethal hit (health {basic.CurrentHealth})");
                }
                if (died != 1)
                {
                    failures.Add($"{enemy}: OnDied raised {died} times");
                }

                yield return null;
                if (basic != null)
                {
                    failures.Add($"{enemy}: still in the scene a frame after its death");
                }

                if (enemy == "Centipede")
                {
                    BasicEnemy[] halves = Object.FindObjectsByType<BasicEnemy>();
                    if (halves.Length != 2)
                    {
                        failures.Add($"Centipede: {halves.Length} enemies left after the split, expected its two halves");
                    }
                    foreach (BasicEnemy half in halves)
                    {
                        if (half.MaxHealth != 30.0)
                        {
                            failures.Add($"Centipede: a half has {half.MaxHealth} health, expected 30");
                        }
                        if (half.GetComponent<CentipedeSplitter>() == null)
                        {
                            failures.Add("Centipede: a half cannot split again");
                        }
                        if (half.GetComponent<Bounty>() == null)
                        {
                            failures.Add("Centipede: a half pays no bounty");
                        }
                    }
                }

                // A clean floor for the next enemy: the halves, and the Queen's escort.
                foreach (BasicEnemy left in Object.FindObjectsByType<BasicEnemy>())
                {
                    Object.Destroy(left.gameObject);
                }
                yield return null;
            }

            Assert.That(failures, Is.Empty, string.Join("\n", failures));
        }

        private static void Check<T>(List<string> failures, string enemy, string what, T actual, T expected)
        {
            if (!EqualityComparer<T>.Default.Equals(actual, expected))
            {
                failures.Add($"{enemy}: {what} is {actual}, data says {expected}");
            }
        }
    }
}
#endif
