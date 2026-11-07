using HoldTheHill.Features.Economy;
using HoldTheHill.Features.Enemies;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// Checks the enemy data and prefabs, not code: every enemy has a data asset with the numbers
    /// it had before the refactor (2026-11-06), every prefab points at its own asset, eight of them
    /// are variants of Enemy_Base carrying only their ability components, the Queen Mantis keeps her
    /// own prefab, and everything that referenced the prefabs (the wave lists, her spawn) still does.
    /// </summary>
    public class EnemyDataTests
    {
        private const string PrefabFolder = "Assets/_Game/Features/Enemies/Prefabs";
        private const string DataFolder = "Assets/_Game/Features/Enemies/Data/Enemies";
        private const string QueenMantisDataPath = "Assets/_Game/Features/Enemies/Data/QueenMantisData.asset";

        private static readonly string[] WaveAssets =
        {
            "Assets/_Game/Features/Enemies/Data/L00_TutorialHill_Waves.asset",
            "Assets/_Game/Features/Enemies/Data/L01_Meadow_Waves.asset",
            "Assets/_Game/Features/Enemies/Data/Hill01_Waves.asset",
        };

        // Jason's template wave list, on his Triangle test enemy: no data asset, but still an enemy.
        private const string TemplateWaveAsset = "Assets/_Game/Features/Enemies/Scripts/EnemySpawner/Map_01.asset";

        private static readonly string[] Variants =
        {
            "Basic", "Scarab", "MantisSpawn", "Centipede", "RhinoBeetle", "RollyPolly", "Silverfish", "Wasp",
        };

        private static GameObject Prefab(string enemy)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"{PrefabFolder}/Enemy_{enemy}.prefab");
            Assert.That(prefab, Is.Not.Null, $"Enemy_{enemy}.prefab is missing.");
            return prefab;
        }

        private static EnemyData Data(string enemy)
        {
            var data = AssetDatabase.LoadAssetAtPath<EnemyData>($"{DataFolder}/{enemy}.asset");
            Assert.That(data, Is.Not.Null, $"{enemy}.asset is missing.");
            return data;
        }

        // The numbers each enemy had on its old prefab: health, damage, speed, tolerance, bounty, flying, click.
        [TestCase("Basic", 30.0, 10, 1.5f, 0.05f, 5, false, false)]
        [TestCase("Centipede", 60.0, 20, 1.1f, 0.05f, 10, false, false)]
        [TestCase("MantisSpawn", 12.0, 5, 1.7f, 0.05f, 2, false, false)]
        [TestCase("QueenMantis", 300.0, 40, 0.8f, 0.05f, 80, false, false)]
        [TestCase("RhinoBeetle", 60.0, 15, 1.1f, 0.05f, 12, false, false)]
        [TestCase("RollyPolly", 45.0, 10, 1.3f, 0.05f, 8, false, false)]
        [TestCase("Scarab", 10.0, 5, 2.8f, 0.05f, 2, false, false)]
        [TestCase("Silverfish", 25.0, 10, 1.8f, 0.05f, 6, false, true)]
        [TestCase("Wasp", 24.0, 10, 2.2f, 0.05f, 8, true, false)]
        public void EachEnemy_HasADataAssetWithItsNumbers(string enemy, double health, int damage, float speed,
            float tolerance, int honeydew, bool flying, bool diesToClick)
        {
            EnemyData data = Data(enemy);

            Assert.That(data.MaxHealth, Is.EqualTo(health));
            Assert.That(data.Damage, Is.EqualTo(damage));
            Assert.That(data.MoveSpeed, Is.EqualTo(speed).Within(0.0001f));
            Assert.That(data.WaypointTolerance, Is.EqualTo(tolerance).Within(0.0001f));
            Assert.That(data.Honeydew, Is.EqualTo(honeydew));
            Assert.That(data.IsFlying, Is.EqualTo(flying));
            Assert.That(data.DiesToClick, Is.EqualTo(diesToClick));
            Assert.That(data.FlipSpriteFacing, Is.False, "The sheet art turns its Body instead of flipping.");
        }

        [TestCase("Basic")]
        [TestCase("Centipede")]
        [TestCase("MantisSpawn")]
        [TestCase("QueenMantis")]
        [TestCase("RhinoBeetle")]
        [TestCase("RollyPolly")]
        [TestCase("Scarab")]
        [TestCase("Silverfish")]
        [TestCase("Wasp")]
        public void EachPrefab_PointsAtItsOwnDataAsset_AndKeepsTheSharedParts(string enemy)
        {
            GameObject prefab = Prefab(enemy);
            var basic = prefab.GetComponent<BasicEnemy>();

            Assert.That(basic, Is.Not.Null, $"Enemy_{enemy} has no BasicEnemy.");
            Assert.That(basic.Data, Is.SameAs(Data(enemy)), $"Enemy_{enemy} does not point at {enemy}.asset.");
            Assert.That(prefab.GetComponent<RegisteredEnemy>(), Is.Not.Null);
            Assert.That(prefab.GetComponent<Bounty>(), Is.Not.Null);
        }

        [Test]
        public void EnemyBase_IsTheSharedEnemy_WithNoDataAndNoAbilities()
        {
            GameObject prefab = Prefab("Base");

            Assert.That(PrefabUtility.GetCorrespondingObjectFromSource(prefab), Is.Null, "Enemy_Base is a variant of nothing.");
            Assert.That(prefab.GetComponent<BasicEnemy>().Data, Is.Null, "Enemy_Base has no numbers of its own; each variant sets its asset.");
            Assert.That(prefab.GetComponent<IDamageModifier>(), Is.Null);
            Assert.That(prefab.GetComponent<IDeathHandler>(), Is.Null);
        }

        [TestCaseSource(nameof(Variants))]
        public void EachVariant_IsMadeFromEnemyBase(string enemy)
        {
            GameObject prefab = Prefab(enemy);
            GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(prefab);

            Assert.That(source, Is.SameAs(Prefab("Base")), $"Enemy_{enemy} is not a variant of Enemy_Base.");
        }

        [Test]
        public void TheQueenMantis_KeepsHerOwnPrefab_OnQueenMantisBoss()
        {
            GameObject prefab = Prefab("QueenMantis");

            Assert.That(PrefabUtility.GetCorrespondingObjectFromSource(prefab), Is.Null, "QueenMantisBoss replaces BasicEnemy, which a variant cannot do.");
            Assert.That(prefab.GetComponent<QueenMantisBoss>(), Is.Not.Null);
        }

        [TestCase("Basic")]
        [TestCase("Scarab")]
        [TestCase("MantisSpawn")]
        [TestCase("Silverfish")]
        [TestCase("Wasp")]
        [TestCase("QueenMantis")]
        public void APlainWalker_HasNoAbilityComponents(string enemy)
        {
            GameObject prefab = Prefab(enemy);

            Assert.That(prefab.GetComponent<IDamageModifier>(), Is.Null);
            Assert.That(prefab.GetComponent<IDeathHandler>(), Is.Null);
        }

        [Test]
        public void TheCentipede_SplitsTwice_AndItsChildrenSpeedUp()
        {
            GameObject prefab = Prefab("Centipede");
            var splitter = prefab.GetComponent<CentipedeSplitter>();
            var speedUp = prefab.GetComponent<CentipedeSpeedUp>();

            Assert.That(splitter, Is.Not.Null);
            Assert.That(speedUp, Is.Not.Null);

            // The numbers are on the data asset; the components read them from there.
            EnemyAbilitySettings abilities = Data("Centipede").Abilities;
            Assert.That(abilities.Splits, Is.EqualTo(2));
            Assert.That(abilities.SplitSizeMultiplier, Is.EqualTo(0.8f).Within(0.0001f));
            Assert.That(abilities.SplitForwardDistance, Is.EqualTo(0.6f).Within(0.0001f));
            Assert.That(abilities.ChildSpeedFactor, Is.EqualTo(1.25f).Within(0.0001f));

            Assert.That(splitter.SplitsRemaining, Is.EqualTo(2));
            Assert.That(splitter.SizeMultiplier, Is.EqualTo(0.8f).Within(0.0001f));
            Assert.That(splitter.SplitForwardDistance, Is.EqualTo(0.6f).Within(0.0001f));
            Assert.That(splitter.SplitPrefab, Is.Null, "Children are copies of the parent, so they bring every component along.");
            Assert.That(speedUp.ChildSpeedFactor, Is.EqualTo(1.25f).Within(0.0001f));
            Assert.That(new SerializedObject(splitter).FindProperty("_splitsRemaining"), Is.Null, "the count moved to the data asset");
        }

        [Test]
        public void TheRhinoBeetle_HasItsShield_OverTheShieldChild()
        {
            GameObject prefab = Prefab("RhinoBeetle");
            var shield = prefab.GetComponent<RhinoBeetleShield>();
            Assert.That(shield, Is.Not.Null);

            // 60 health plus a 40 shield, kept apart: the shield is on the data asset beside the health.
            EnemyData data = Data("RhinoBeetle");
            Assert.That(data.MaxHealth, Is.EqualTo(60.0));
            Assert.That(data.Abilities.ShieldCapacity, Is.EqualTo(40f));
            Assert.That(data.Abilities.ShieldRegrowDelay, Is.EqualTo(3f));
            Assert.That(data.Abilities.ShieldRegrowPerSecond, Is.EqualTo(25f));
            Assert.That(shield.Shield.Capacity, Is.EqualTo(40.0), "the component reads the data asset");

            var so = new SerializedObject(shield);
            Assert.That(so.FindProperty("_shieldCapacity"), Is.Null, "the capacity moved to the data asset");
            var visual = so.FindProperty("_shieldVisual").objectReferenceValue as SpriteRenderer;
            Assert.That(visual, Is.Not.Null, "The shield has no sprite to fade.");
            Assert.That(visual.transform.parent, Is.SameAs(prefab.transform));
            Assert.That(visual.name, Is.EqualTo("Shield"));
        }

        [Test]
        public void TheRollyPolly_HasItsCurl()
        {
            GameObject prefab = Prefab("RollyPolly");
            var curler = prefab.GetComponent<RollyPollyCurler>();
            Assert.That(curler, Is.Not.Null);

            EnemyAbilitySettings abilities = Data("RollyPolly").Abilities;
            Assert.That(abilities.MaxCurls, Is.EqualTo(3));
            Assert.That(abilities.RollDistance, Is.EqualTo(3f));
            Assert.That(abilities.CurledDamageShare, Is.EqualTo(0.4f).Within(0.0001f));
            Assert.That(abilities.CurledSpeedMultiplier, Is.EqualTo(2f));

            // The look stays on the component.
            var so = new SerializedObject(curler);
            Assert.That(so.FindProperty("_maxCurls"), Is.Null, "the count moved to the data asset");
            Assert.That(so.FindProperty("_curledScale").floatValue, Is.EqualTo(0.75f).Within(0.0001f));
            Color tint = so.FindProperty("_curledTint").colorValue;
            Assert.That(tint.r, Is.EqualTo(0.55f).Within(0.0001f));
            Assert.That(tint.g, Is.EqualTo(0.55f).Within(0.0001f));
            Assert.That(tint.b, Is.EqualTo(0.65f).Within(0.0001f));
        }

        [Test]
        public void TheSilverfish_HasTheColliderTheClickReaderUses()
        {
            GameObject prefab = Prefab("Silverfish");
            var collider = prefab.GetComponent<CircleCollider2D>();

            Assert.That(collider, Is.Not.Null);
            Assert.That(collider.radius, Is.EqualTo(0.8f).Within(0.0001f));
        }

        [Test]
        public void TheWasp_DropsStingers()
        {
            GameObject prefab = Prefab("Wasp");
            Assert.That(prefab.GetComponent<HoldTheHill.Features.Hazards.WaspStingerDropper>(), Is.Not.Null);
        }

        [TestCaseSource(nameof(WaveAssets))]
        public void EveryWaveEntry_StillPointsAtAnEnemy(string path)
        {
            AssertWaveEntries(path, requireData: true);
        }

        [Test]
        public void TheTemplateWaveList_StillPointsAtAnEnemy()
        {
            AssertWaveEntries(TemplateWaveAsset, requireData: false);
        }

        private static void AssertWaveEntries(string path, bool requireData)
        {
            var waves = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(path);
            Assert.That(waves, Is.Not.Null, $"{path} is missing.");
            Assert.That(waves.waves, Is.Not.Empty);

            for (int w = 0; w < waves.waves.Count; w++)
            {
                Wave wave = waves.waves[w];
                // Grouped rows unrolled plus single entries: what the spawner sends.
                System.Collections.Generic.List<EnemySpawnEntry> entries = WaveSpawnPlan.Expand(wave);
                Assert.That(entries, Is.Not.Empty, $"{waves.name} wave {w + 1} is empty.");
                for (int e = 0; e < entries.Count; e++)
                {
                    GameObject enemyPrefab = entries[e].enemyPrefab;
                    Assert.That(enemyPrefab, Is.Not.Null, $"{waves.name} wave {w + 1} entry {e + 1} points at nothing.");
                    Assert.That(enemyPrefab.GetComponent<BasicEnemy>(), Is.Not.Null, $"{waves.name} wave {w + 1} entry {e + 1} ({enemyPrefab.name}) is no enemy.");
                    if (requireData)
                    {
                        Assert.That(enemyPrefab.GetComponent<BasicEnemy>().Data, Is.Not.Null, $"{waves.name} wave {w + 1} entry {e + 1} ({enemyPrefab.name}) has no data asset.");
                    }
                }
            }
        }

        [Test]
        public void TheQueenMantis_StillSummonsTheMantisSpawn()
        {
            var data = AssetDatabase.LoadAssetAtPath<QueenMantisData>(QueenMantisDataPath);
            Assert.That(data, Is.Not.Null);

            Assert.That(data.SpawnPrefab, Is.SameAs(Prefab("MantisSpawn").GetComponent<BasicEnemy>()));
        }
    }
}
