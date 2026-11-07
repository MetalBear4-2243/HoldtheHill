using System.Collections.Generic;
using System.Text;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.Towers;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// Moves the tuning numbers into data (2026-11-06): each tower's range, damage, interval, cone,
    /// upgrade prices and ability strength onto its <see cref="TowerData"/> level table, each enemy
    /// ability's numbers onto its <see cref="EnemyData"/>, the placers' per-scene (data, prefab) pairs
    /// into one list of definitions, and the level wave lists into grouped rows. The numbers written
    /// are the ones the prefabs and components carried on Indev 67004d9a, so nothing plays differently.
    /// </summary>
    /// <remarks>
    /// Safe to run again: a data asset that already has its table or its abilities is left alone, a
    /// prefab or scene is saved again only to drop the fields that moved. The other builders call the
    /// helpers here when they make tower assets, so a fresh project ends up the same way.
    /// </remarks>
    internal static class BalanceDataBuilder
    {
        private const string TowerDataFolder = "Assets/_Game/Features/Towers/Data";
        private const string TowerPrefabFolder = "Assets/_Game/Features/Towers/Prefabs";
        private const string EnemyDataFolder = "Assets/_Game/Features/Enemies/Data/Enemies";
        private const string EnemyPrefabFolder = "Assets/_Game/Features/Enemies/Prefabs";
        private const string WaveDataFolder = "Assets/_Game/Features/Enemies/Data";
        private const string LevelsFolder = "Assets/_Game/Scenes/Levels";

        /// <summary>The six towers in number-key and shop order (TowerListDataTests).</summary>
        internal static readonly string[] TowerKeyOrder = { "Ant", "BulletAnt", "CrazyAnt", "FireAnt", "SapAnt", "CarpenterAnt" };

        /// <summary>The gameplay scenes with a placer and a spawner.</summary>
        internal static readonly string[] GameplayScenes =
        {
            $"{LevelsFolder}/Hill01/Hill01_Gameplay.unity",
            $"{LevelsFolder}/L00_TutorialHill/L00_TutorialHill_Gameplay.unity",
            $"{LevelsFolder}/L01_Meadow/L01_Meadow_Gameplay.unity",
        };

        /// <summary>The level wave lists (Jason's template Map_01 is not one; it keeps its single entries).</summary>
        internal static readonly string[] LevelWaveAssets =
        {
            $"{WaveDataFolder}/Hill01_Waves.asset",
            $"{WaveDataFolder}/L00_TutorialHill_Waves.asset",
            $"{WaveDataFolder}/L01_Meadow_Waves.asset",
        };

        private static readonly string[] EnemyVariants = { "Basic", "Scarab", "MantisSpawn", "Centipede", "RhinoBeetle", "RollyPolly", "Silverfish", "Wasp" };

        private static TowerLevel Level(int cost, float range, float damage, float interval, TowerAbilitySettings ability)
        {
            return new TowerLevel { Cost = cost, Range = range, Damage = damage, AttackInterval = interval, Ability = ability };
        }

        private static readonly TowerAbilitySettings None = default;
        private static readonly TowerAbilitySettings Flame = new TowerAbilitySettings { ConeDegrees = 70f };
        private static readonly TowerAbilitySettings Sap = new TowerAbilitySettings { AreaRadius = 0.35f, SlowMultiplier = 0.5f, SlowSeconds = 4f };
        private static readonly TowerAbilitySettings Mines = new TowerAbilitySettings { AreaRadius = 0.9f, MaxMines = 3 };
        private static readonly TowerAbilitySettings Poison = new TowerAbilitySettings { PoisonMaxStacks = 5, PoisonDamageShare = 0.25f };

        // The numbers each tower prefab, its projectile, puddle and mine carried on Indev 67004d9a:
        // build price, then per level cost / range / damage / interval, and the ability strength.
        private static readonly (string data, TowerLevel[] levels)[] Towers =
        {
            ("Ant", new[] { Level(75, 1.5f, 8f, 0.8f, None), Level(45, 1.6f, 12f, 0.7f, None), Level(70, 1.7f, 18f, 0.6f, None) }),
            ("BulletAnt", new[] { Level(100, 3f, 12f, 1.2f, None), Level(60, 3.3f, 18f, 1.1f, None), Level(90, 3.6f, 26f, 1f, None) }),
            ("CrazyAnt", new[] { Level(125, 1.5f, 4f, 0.35f, Poison), Level(75, 1.6f, 6f, 0.3f, Poison), Level(110, 1.7f, 9f, 0.25f, Poison) }),
            ("FireAnt", new[] { Level(200, 2f, 3f, 0.2f, Flame), Level(120, 2.2f, 4.5f, 0.2f, Flame), Level(180, 2.4f, 6f, 0.2f, Flame) }),
            ("SapAnt", new[] { Level(225, 2f, 0f, 2f, Sap), Level(135, 2.4f, 0f, 1.8f, Sap), Level(200, 2.8f, 0f, 1.6f, Sap) }),
            ("CarpenterAnt", new[] { Level(250, 2.5f, 30f, 3f, Mines), Level(150, 2.8f, 45f, 2.5f, Mines), Level(225, 3.1f, 65f, 2f, Mines) }),
        };

        /// <summary>The numbers an enemy's ability components carried on nilly/enemy-data-assets d2cbccbf; zeros for an enemy without them.</summary>
        internal static EnemyAbilitySettings EnemyAbilitiesFor(string enemy)
        {
            switch (enemy)
            {
                case "RhinoBeetle":
                    return new EnemyAbilitySettings { ShieldCapacity = 40f, ShieldRegrowDelay = 3f, ShieldRegrowPerSecond = 25f };
                case "RollyPolly":
                    return new EnemyAbilitySettings { MaxCurls = 3, RollDistance = 3f, CurledDamageShare = 0.4f, CurledSpeedMultiplier = 2f };
                case "Centipede":
                    return new EnemyAbilitySettings { Splits = 2, SplitSizeMultiplier = 0.8f, SplitForwardDistance = 0.6f, ChildSpeedFactor = 1.25f };
                default:
                    return default;
            }
        }

        [MenuItem("Tools/Hold the Hill/Balance/Migrate Tuning Into Data")]
        private static void Build()
        {
            BuildAll();
        }

        /// <summary>Entry point for a batch run: <c>-executeMethod HoldTheHill.EditorTools.BalanceDataBuilder.BuildFromCommandLine</c>.</summary>
        public static void BuildFromCommandLine()
        {
            BuildAll();
        }

        private static void BuildAll()
        {
            MigrateTowers();
            MigrateEnemies();
            AssetDatabase.SaveAssets();
            MigratePlacers();
            MigrateWaves();
            AssetDatabase.SaveAssets();
            ReportAfterReload();
            Debug.Log("Balance data migrated: tower levels and enemy abilities on their data assets, placers on definitions, waves in grouped rows.");
        }

        // ---- Towers ---------------------------------------------------------------------------

        private static void MigrateTowers()
        {
            for (int i = 0; i < Towers.Length; i++)
            {
                string dataPath = $"{TowerDataFolder}/{Towers[i].data}.asset";
                string prefabPath = $"{TowerPrefabFolder}/Tower_{Towers[i].data}.prefab";
                var data = AssetDatabase.LoadAssetAtPath<TowerData>(dataPath);
                if (data == null)
                {
                    Debug.LogError($"{dataPath} is missing; run Build Core Loop and Build Towers first.");
                    continue;
                }

                // An asset from before the table has one default row (cost 0): the class initializer.
                if (data.LevelCount <= 1 && data.honeydewCost == 0)
                {
                    SetLevels(data, Towers[i].levels);
                    Debug.Log($"{data.name}: level table written ({Towers[i].levels.Length} levels, build price {data.honeydewCost}).");
                }

                if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) == null)
                {
                    Debug.LogError($"{prefabPath} is missing; run Build Core Loop and Build Towers first.");
                    continue;
                }

                // Saved again even when nothing changed, so the fields that moved to the data leave the prefab.
                GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
                try
                {
                    Tower tower = root.GetComponent<Tower>();
                    if (tower.Definition != data)
                    {
                        SetDefinition(tower, data);
                        Debug.Log($"{root.name}: points at {data.name}.");
                    }
                    PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }

                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (data.Prefab != prefab.GetComponent<Tower>())
                {
                    SetPrefab(data, prefab.GetComponent<Tower>());
                    Debug.Log($"{data.name}: prefab is {prefab.name}.");
                }
            }
        }

        /// <summary>Writes the level table onto a tower's data asset (no Undo; builders only).</summary>
        internal static void SetLevels(TowerData data, IList<TowerLevel> levels)
        {
            var so = new SerializedObject(data);
            SerializedProperty table = so.FindProperty("_levels");
            table.arraySize = levels.Count;
            for (int i = 0; i < levels.Count; i++)
            {
                WriteLevel(table.GetArrayElementAtIndex(i), levels[i]);
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(data);
        }

        /// <summary>Writes one level into a <c>_levels</c> element.</summary>
        internal static void WriteLevel(SerializedProperty element, TowerLevel level)
        {
            element.FindPropertyRelative("Cost").intValue = level.Cost;
            element.FindPropertyRelative("Damage").floatValue = level.Damage;
            element.FindPropertyRelative("AttackInterval").floatValue = level.AttackInterval;
            element.FindPropertyRelative("Range").floatValue = level.Range;
            SerializedProperty ability = element.FindPropertyRelative("Ability");
            ability.FindPropertyRelative("ConeDegrees").floatValue = level.Ability.ConeDegrees;
            ability.FindPropertyRelative("AreaRadius").floatValue = level.Ability.AreaRadius;
            ability.FindPropertyRelative("SlowMultiplier").floatValue = level.Ability.SlowMultiplier;
            ability.FindPropertyRelative("SlowSeconds").floatValue = level.Ability.SlowSeconds;
            ability.FindPropertyRelative("MaxMines").intValue = level.Ability.MaxMines;
            ability.FindPropertyRelative("PoisonMaxStacks").intValue = level.Ability.PoisonMaxStacks;
            ability.FindPropertyRelative("PoisonDamageShare").floatValue = level.Ability.PoisonDamageShare;
        }

        /// <summary>Points a tower's data asset at the prefab it builds.</summary>
        internal static void SetPrefab(TowerData data, Tower prefab)
        {
            var so = new SerializedObject(data);
            so.FindProperty("_prefab").objectReferenceValue = prefab;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(data);
        }

        /// <summary>Points a tower (a prefab's, or prefab contents') at its data asset.</summary>
        internal static void SetDefinition(Tower tower, TowerData data)
        {
            var so = new SerializedObject(tower);
            so.FindProperty("_data").objectReferenceValue = data;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(tower);
        }

        /// <summary>Puts the definitions on offer in a placer, in order.</summary>
        internal static void SetPlacerTowers(TowerPlacer placer, IList<TowerData> datas)
        {
            var so = new SerializedObject(placer);
            SerializedProperty towers = so.FindProperty("_definitions");
            towers.arraySize = datas.Count;
            for (int i = 0; i < datas.Count; i++)
            {
                towers.GetArrayElementAtIndex(i).objectReferenceValue = datas[i];
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(placer);
        }

        // ---- Enemies --------------------------------------------------------------------------

        private static void MigrateEnemies()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:EnemyData", new[] { EnemyDataFolder }))
            {
                var data = AssetDatabase.LoadAssetAtPath<EnemyData>(AssetDatabase.GUIDToAssetPath(guid));
                EnemyAbilitySettings abilities = EnemyAbilitiesFor(data.name);
                if (data.Abilities.Equals(default(EnemyAbilitySettings)) && !abilities.Equals(default(EnemyAbilitySettings)))
                {
                    data.Abilities = abilities;
                    EditorUtility.SetDirty(data);
                    Debug.Log($"{data.name}: abilities written ({DescribeAbilities(abilities)}).");
                }
            }

            // Saved again so the ability fields that moved to the data leave the variants (an unused
            // override on a variant is a field the base no longer has).
            for (int i = 0; i < EnemyVariants.Length; i++)
            {
                string path = $"{EnemyPrefabFolder}/Enemy_{EnemyVariants[i]}.prefab";
                if (AssetDatabase.LoadAssetAtPath<GameObject>(path) == null)
                {
                    continue;
                }

                GameObject root = PrefabUtility.LoadPrefabContents(path);
                try
                {
                    if (PrefabUtility.GetCorrespondingObjectFromSource(root) != null)
                    {
                        PrefabUtility.RemoveUnusedOverrides(new[] { root }, InteractionMode.AutomatedAction);
                    }
                    PrefabUtility.SaveAsPrefabAsset(root, path);
                }
                finally
                {
                    PrefabUtility.UnloadPrefabContents(root);
                }
            }
        }

        private static string DescribeAbilities(EnemyAbilitySettings a)
        {
            var parts = new List<string>();
            if (a.ShieldCapacity > 0f)
            {
                parts.Add($"shield {a.ShieldCapacity}, regrows {a.ShieldRegrowPerSecond}/s after {a.ShieldRegrowDelay} s");
            }
            if (a.MaxCurls > 0)
            {
                parts.Add($"{a.MaxCurls} curls of {a.RollDistance} tiles at {a.CurledDamageShare} damage, x{a.CurledSpeedMultiplier} speed");
            }
            if (a.Splits != 0)
            {
                parts.Add($"{a.Splits} splits, halves x{a.SplitSizeMultiplier} at {a.SplitForwardDistance} forward, x{a.ChildSpeedFactor} speed");
            }
            return parts.Count > 0 ? string.Join("; ", parts) : "none";
        }

        // ---- Placers --------------------------------------------------------------------------

        private static void MigratePlacers()
        {
            for (int s = 0; s < GameplayScenes.Length; s++)
            {
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(GameplayScenes[s]) == null)
                {
                    continue;
                }

                Scene scene = EditorSceneManager.OpenScene(GameplayScenes[s], OpenSceneMode.Single);
                TowerPlacer placer = Object.FindAnyObjectByType<TowerPlacer>();
                if (placer == null)
                {
                    Debug.LogWarning($"{scene.name} has no Tower Placer; nothing to migrate.");
                    continue;
                }

                // Loaded after the scene is open (opening one unloads assets held only in locals).
                var datas = new TowerData[TowerKeyOrder.Length];
                for (int i = 0; i < TowerKeyOrder.Length; i++)
                {
                    datas[i] = AssetDatabase.LoadAssetAtPath<TowerData>($"{TowerDataFolder}/{TowerKeyOrder[i]}.asset");
                }

                bool same = placer.Offers.Count == datas.Length;
                for (int i = 0; same && i < datas.Length; i++)
                {
                    same = placer.Offers[i] == datas[i];
                }

                if (!same)
                {
                    SetPlacerTowers(placer, datas);
                    Debug.Log($"{scene.name}: placer offers the six definitions in key order.");
                }

                // Saved either way: the old per-scene (data, prefab) pairs leave the file.
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
        }

        // ---- Waves ----------------------------------------------------------------------------

        private static void MigrateWaves()
        {
            var leftSingle = new List<EnemySpawnEntry>();
            for (int i = 0; i < LevelWaveAssets.Length; i++)
            {
                var asset = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(LevelWaveAssets[i]);
                if (asset == null)
                {
                    continue;
                }

                bool changed = false;
                for (int w = 0; w < asset.waves.Count; w++)
                {
                    Wave wave = asset.waves[w];
                    if (wave.groups.Count > 0 || wave.enemies.Count == 0)
                    {
                        continue;
                    }

                    int before = WaveSpawnPlan.Count(wave);
                    float secondsBefore = WaveSpawnPlan.SpawnSeconds(wave);
                    wave.groups = WaveSpawnPlan.Group(wave.enemies, leftSingle);
                    wave.enemies = new List<EnemySpawnEntry>(leftSingle);
                    changed = true;
                    Debug.Log($"{asset.name} {wave.waveName}: {before} entries -> {wave.groups.Count} rows + {wave.enemies.Count} single; "
                              + $"{WaveSpawnPlan.Count(wave)} enemies over {WaveSpawnPlan.SpawnSeconds(wave):F1} s (was {secondsBefore:F1} s).");
                }

                if (changed)
                {
                    EditorUtility.SetDirty(asset);
                }
            }
        }

        // ---- Report ---------------------------------------------------------------------------

        // Loads everything again and says what it holds, so a batch log shows the truth.
        private static void ReportAfterReload()
        {
            for (int i = 0; i < TowerKeyOrder.Length; i++)
            {
                var data = AssetDatabase.LoadAssetAtPath<TowerData>($"{TowerDataFolder}/{TowerKeyOrder[i]}.asset");
                if (data == null)
                {
                    Debug.LogError($"{TowerKeyOrder[i]}.asset is missing after the build.");
                    continue;
                }

                var text = new StringBuilder($"{data.name}: prefab {(data.Prefab != null ? data.Prefab.name : "NONE")}");
                text.Append(data.Prefab != null && data.Prefab.Definition == data ? " (points back)" : " (DOES NOT POINT BACK)");
                for (int level = 1; level <= data.LevelCount; level++)
                {
                    TowerLevel row = data.LevelAt(level);
                    text.Append($"; L{level} {row.Cost} hd, {row.Range}/{row.Damage}/{row.AttackInterval} s, dps {row.SingleTargetDps:F1}");
                }
                Debug.Log(text.ToString());
            }

            foreach (string guid in AssetDatabase.FindAssets("t:EnemyData", new[] { EnemyDataFolder }))
            {
                var data = AssetDatabase.LoadAssetAtPath<EnemyData>(AssetDatabase.GUIDToAssetPath(guid));
                Debug.Log($"{data.name}: hp {data.MaxHealth}, speed {data.MoveSpeed}, abilities {DescribeAbilities(data.Abilities)}");
            }

            for (int s = 0; s < GameplayScenes.Length; s++)
            {
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(GameplayScenes[s]) == null)
                {
                    continue;
                }

                Scene scene = EditorSceneManager.OpenScene(GameplayScenes[s], OpenSceneMode.Single);
                TowerPlacer placer = Object.FindAnyObjectByType<TowerPlacer>();
                var names = new List<string>();
                for (int i = 0; placer != null && i < placer.Offers.Count; i++)
                {
                    names.Add(placer.Offers[i] != null ? placer.Offers[i].name : "NONE");
                }
                Debug.Log($"{scene.name}: placer offers [{string.Join(", ", names)}]");
            }

            for (int i = 0; i < LevelWaveAssets.Length; i++)
            {
                var asset = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(LevelWaveAssets[i]);
                if (asset == null)
                {
                    continue;
                }

                var text = new StringBuilder($"{asset.name}:");
                for (int w = 0; w < asset.waves.Count; w++)
                {
                    Wave wave = asset.waves[w];
                    text.Append($" {wave.waveName} = {wave.groups.Count} rows + {wave.enemies.Count} single, {WaveSpawnPlan.Count(wave)} enemies;");
                }
                Debug.Log(text.ToString());
            }
        }
    }
}
