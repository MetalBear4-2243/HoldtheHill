using System.Collections.Generic;
using System.Text;
using HoldTheHill.Features.Economy;
using HoldTheHill.Features.Enemies;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// Alpha step 5, the enemies. Makes a prefab for each GDD enemy beside the basic one (Rhino
    /// Beetle, Wasp, Scarab, Rolly Polly, Mantis Spawn, Silverfish), fills the Tutorial Hill and
    /// Meadow wave lists with placeholder waves that bring in one new enemy at a time, and puts a
    /// <see cref="SilverfishClickReader"/> in both level scenes so a click kills a Silverfish
    /// under the Input System. Hill01 is left as it is.
    /// </summary>
    /// <remarks>
    /// Safe to run again: prefabs that exist are kept (so Inspector tuning survives), a wave list
    /// is rewritten only while it still holds nothing but the basic enemy, and scene objects are
    /// found by component and only their references are refreshed. Runs after the levels builder.
    /// </remarks>
    internal static class AlphaEnemiesBuilder
    {
        private const string EnemyPrefabFolder = "Assets/_Game/Features/Enemies/Prefabs";
        private const string WaveDataFolder = "Assets/_Game/Features/Enemies/Data";
        private const string LevelsFolder = "Assets/_Game/Scenes/Levels";
        private const string ActionsPath = "Assets/_Game/Input/HoldTheHill.inputactions";

        /// <summary>Every enemy the waves can name. Basic is the step 1 prefab; the rest are made here.</summary>
        internal enum Kind
        {
            Basic,
            Scarab,
            MantisSpawn,
            RhinoBeetle,
            Wasp,
            RollyPolly,
            Silverfish
        }

        /// <summary>One enemy prefab: a variant of the basic enemy, or a new prefab on its own subclass.</summary>
        internal readonly struct EnemySpec
        {
            public EnemySpec(Kind kind, string prefabName, Color color, float scale, double health, int damage,
                float speed, int bounty, bool variantOfBasic)
            {
                Kind = kind;
                PrefabName = prefabName;
                Color = color;
                Scale = scale;
                Health = health;
                Damage = damage;
                Speed = speed;
                Bounty = bounty;
                VariantOfBasic = variantOfBasic;
            }

            public Kind Kind { get; }
            public string PrefabName { get; }
            public Color Color { get; }
            public float Scale { get; }
            public double Health { get; }
            public int Damage { get; }
            public float Speed { get; }
            public int Bounty { get; }
            public bool VariantOfBasic { get; }

            public string PrefabPath => $"{EnemyPrefabFolder}/{PrefabName}.prefab";
        }

        /// <summary>The GDD enemies at placeholder numbers; the basic enemy is 30 health, 10 damage, speed 1.5, bounty 5.</summary>
        internal static readonly EnemySpec[] Enemies =
        {
            new EnemySpec(Kind.Scarab, "Enemy_Scarab", new Color(0.15f, 0.6f, 0.55f), 0.35f, 10.0, 5, 2.8f, 2, variantOfBasic: true),
            new EnemySpec(Kind.MantisSpawn, "Enemy_MantisSpawn", new Color(0.7f, 0.85f, 0.5f), 0.45f, 12.0, 5, 1.7f, 2, variantOfBasic: true),
            new EnemySpec(Kind.RhinoBeetle, "Enemy_RhinoBeetle", new Color(0.4f, 0.25f, 0.15f), 0.8f, 60.0, 15, 1.1f, 12, variantOfBasic: false),
            new EnemySpec(Kind.Wasp, "Enemy_Wasp", new Color(0.95f, 0.8f, 0.15f), 0.5f, 24.0, 10, 2.2f, 8, variantOfBasic: false),
            new EnemySpec(Kind.RollyPolly, "Enemy_RollyPolly", new Color(0.45f, 0.5f, 0.6f), 0.6f, 45.0, 10, 1.3f, 8, variantOfBasic: false),
            new EnemySpec(Kind.Silverfish, "Enemy_Silverfish", new Color(0.8f, 0.82f, 0.88f), 0.5f, 25.0, 10, 1.8f, 6, variantOfBasic: false),
        };

        /// <summary>A run of one enemy kind in a wave: how many, and the gap after each.</summary>
        internal readonly struct Group
        {
            public Group(Kind kind, int count, float delay)
            {
                Kind = kind;
                Count = count;
                Delay = delay;
            }

            public Kind Kind { get; }
            public int Count { get; }
            public float Delay { get; }
        }

        private static Group G(Kind kind, int count, float delay)
        {
            return new Group(kind, count, delay);
        }

        // Tutorial Hill, 5 waves: basics, then the scarab swarm, the rhino, the wasp, the silverfish.
        private static readonly Group[][] TutorialHillWaves =
        {
            new[] { G(Kind.Basic, 4, 1.4f) },
            new[] { G(Kind.Basic, 3, 1.2f), G(Kind.Scarab, 8, 0.35f) },
            new[] { G(Kind.RhinoBeetle, 2, 2f), G(Kind.Basic, 4, 1f) },
            new[] { G(Kind.Wasp, 3, 1.5f), G(Kind.Basic, 4, 1f), G(Kind.Scarab, 6, 0.35f) },
            new[] { G(Kind.Silverfish, 3, 1.2f), G(Kind.RhinoBeetle, 2, 1.5f), G(Kind.Wasp, 2, 1.2f), G(Kind.Basic, 5, 0.8f) },
        };

        // Meadow, 10 waves: one new kind per wave up to the mantis spawn, then mixes that grow.
        private static readonly Group[][] MeadowWaves =
        {
            new[] { G(Kind.Basic, 5, 1.2f) },
            new[] { G(Kind.Scarab, 10, 0.35f), G(Kind.Basic, 3, 1f) },
            new[] { G(Kind.RhinoBeetle, 3, 1.6f), G(Kind.Basic, 4, 1f) },
            new[] { G(Kind.Wasp, 4, 1.2f), G(Kind.Basic, 4, 1f) },
            new[] { G(Kind.RollyPolly, 3, 1.8f), G(Kind.Basic, 4, 1f) },
            new[] { G(Kind.Silverfish, 4, 1f), G(Kind.Scarab, 8, 0.35f) },
            new[] { G(Kind.MantisSpawn, 8, 0.6f), G(Kind.Basic, 4, 1f) },
            new[] { G(Kind.RhinoBeetle, 3, 1.4f), G(Kind.Wasp, 4, 1f), G(Kind.RollyPolly, 2, 1.5f), G(Kind.Basic, 5, 0.8f) },
            new[] { G(Kind.Scarab, 12, 0.3f), G(Kind.Silverfish, 4, 1f), G(Kind.Wasp, 4, 1f), G(Kind.RhinoBeetle, 3, 1.2f) },
            new[] { G(Kind.RollyPolly, 4, 1.2f), G(Kind.RhinoBeetle, 4, 1.2f), G(Kind.Wasp, 5, 0.9f), G(Kind.MantisSpawn, 6, 0.5f), G(Kind.Basic, 6, 0.7f), G(Kind.Scarab, 10, 0.3f) },
        };

        private static readonly (string id, string scene, string waves, Group[][] layout)[] Levels =
        {
            ("L00_TutorialHill", $"{LevelsFolder}/L00_TutorialHill/L00_TutorialHill_Gameplay.unity", $"{WaveDataFolder}/L00_TutorialHill_Waves.asset", TutorialHillWaves),
            ("L01_Meadow", $"{LevelsFolder}/L01_Meadow/L01_Meadow_Gameplay.unity", $"{WaveDataFolder}/L01_Meadow_Waves.asset", MeadowWaves),
        };

        [MenuItem("Tools/Hold the Hill/Alpha/Build Enemies (six kinds, waves, Silverfish click)")]
        private static void Build()
        {
            Sprite circle = AlphaCoreLoopBuilder.EnsureSprite("Circle", circle: true);
            GameObject basic = AlphaCoreLoopBuilder.EnsureEnemyPrefab(circle);

            var prefabs = new Dictionary<Kind, GameObject> { { Kind.Basic, basic } };
            for (int i = 0; i < Enemies.Length; i++)
            {
                prefabs[Enemies[i].Kind] = EnsureEnemyPrefab(Enemies[i], circle, basic);
            }
            AssetDatabase.SaveAssets();

            // Wave lists before any scene opens: opening a scene unloads loose ScriptableObjects.
            for (int i = 0; i < Levels.Length; i++)
            {
                FillWaves(Levels[i].waves, Levels[i].layout, prefabs);
            }
            AssetDatabase.SaveAssets();

            for (int i = 0; i < Levels.Length; i++)
            {
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Levels[i].scene) == null)
                {
                    Debug.LogWarning($"{Levels[i].scene} does not exist yet; run Build Home and Levels first.");
                    continue;
                }

                WireClickReader(Levels[i].scene);
            }

            AssetDatabase.SaveAssets();
            ReportAfterReload();
            Debug.Log("Alpha enemies built: Tutorial Hill and Meadow send all six kinds; click a Silverfish to kill it.");
        }

        /// <summary>Entry point for a batch run: <c>-executeMethod HoldTheHill.EditorTools.AlphaEnemiesBuilder.BuildFromCommandLine</c>.</summary>
        public static void BuildFromCommandLine()
        {
            Build();
        }

        // A prefab that exists is kept as it is, Inspector tuning included.
        private static GameObject EnsureEnemyPrefab(EnemySpec spec, Sprite circle, GameObject basic)
        {
            AlphaCoreLoopBuilder.EnsureFolder(EnemyPrefabFolder);

            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(spec.PrefabPath);
            if (existing != null)
            {
                return existing;
            }

            GameObject go = spec.VariantOfBasic ? NewVariantOfBasic(spec, basic) : NewSubclassEnemy(spec, circle);

            var enemySo = new SerializedObject(go.GetComponent<BasicEnemy>());
            enemySo.FindProperty("_maxHealth").doubleValue = spec.Health;
            enemySo.FindProperty("_damage").intValue = spec.Damage;
            enemySo.FindProperty("_moveSpeed").floatValue = spec.Speed;
            enemySo.ApplyModifiedPropertiesWithoutUndo();

            var bountySo = new SerializedObject(go.GetComponent<Bounty>());
            bountySo.FindProperty("_honeydew").intValue = spec.Bounty;
            bountySo.ApplyModifiedPropertiesWithoutUndo();

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, spec.PrefabPath);
            Object.DestroyImmediate(go);
            Debug.Log($"Made {spec.PrefabName} ({(spec.VariantOfBasic ? "variant of Enemy_Basic" : prefab.GetComponent<BasicEnemy>().GetType().Name)}).");
            return prefab;
        }

        // Scarab and Mantis Spawn: the basic enemy with other numbers, so a change to Enemy_Basic reaches them.
        private static GameObject NewVariantOfBasic(EnemySpec spec, GameObject basic)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(basic);
            instance.name = spec.PrefabName;
            instance.transform.localScale = Vector3.one * spec.Scale;
            instance.GetComponent<SpriteRenderer>().color = spec.Color;
            return instance;
        }

        // Rhino Beetle, Wasp, Rolly Polly, Silverfish: each on its own BasicEnemy subclass.
        private static GameObject NewSubclassEnemy(EnemySpec spec, Sprite circle)
        {
            var go = new GameObject(spec.PrefabName);
            go.transform.localScale = Vector3.one * spec.Scale;
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = circle;
            renderer.color = spec.Color;
            // A flyer is drawn above the walkers.
            renderer.sortingOrder = spec.Kind == Kind.Wasp ? 7 : 5;

            switch (spec.Kind)
            {
                case Kind.RhinoBeetle:
                    var beetle = go.AddComponent<RhinoBeetleEnemy>();
                    var shield = new GameObject("Shield");
                    shield.transform.SetParent(go.transform, false);
                    shield.transform.localScale = Vector3.one * 1.35f;
                    var shieldRenderer = shield.AddComponent<SpriteRenderer>();
                    shieldRenderer.sprite = circle;
                    shieldRenderer.color = new Color(0.55f, 0.8f, 1f, 0.55f);
                    shieldRenderer.sortingOrder = 6;
                    var beetleSo = new SerializedObject(beetle);
                    beetleSo.FindProperty("_shieldVisual").objectReferenceValue = shieldRenderer;
                    beetleSo.ApplyModifiedPropertiesWithoutUndo();
                    break;
                case Kind.Wasp:
                    go.AddComponent<WaspEnemy>();
                    break;
                case Kind.RollyPolly:
                    go.AddComponent<RollyPollyEnemy>();
                    break;
                case Kind.Silverfish:
                    // Jason's class requires a Collider2D for its OnMouseDown; the click reader uses the same collider.
                    var collider = go.AddComponent<CircleCollider2D>();
                    collider.radius = 0.8f;
                    go.AddComponent<SilverfishEnemy>();
                    break;
                default:
                    go.AddComponent<BasicEnemy>();
                    break;
            }

            go.AddComponent<RegisteredEnemy>();
            go.AddComponent<Bounty>();
            return go;
        }

        /// <summary>
        /// Writes the placeholder waves into a level's wave list, but only while that list still
        /// holds nothing but the basic enemy (the step 3 placeholder): a list someone has tuned is kept.
        /// </summary>
        private static void FillWaves(string path, Group[][] layout, Dictionary<Kind, GameObject> prefabs)
        {
            MapWaveDataSO waves = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(path);
            if (waves == null)
            {
                Debug.LogWarning($"{path} does not exist yet; run Build Home and Levels first.");
                return;
            }

            if (!HoldsOnlyBasic(waves, prefabs[Kind.Basic]))
            {
                Debug.Log($"{waves.name} already has other enemies in it; kept as it is.");
                return;
            }

            waves.waves = new List<Wave>(layout.Length);
            for (int w = 0; w < layout.Length; w++)
            {
                var wave = new Wave { waveName = $"Wave {w + 1}" };
                for (int g = 0; g < layout[w].Length; g++)
                {
                    Group group = layout[w][g];
                    for (int n = 0; n < group.Count; n++)
                    {
                        wave.enemies.Add(new EnemySpawnEntry { enemyPrefab = prefabs[group.Kind], delayBeforeNext = group.Delay });
                    }
                }
                waves.waves.Add(wave);
            }

            EditorUtility.SetDirty(waves);
            Debug.Log($"{waves.name}: {layout.Length} waves written with the new enemies.");
        }

        private static bool HoldsOnlyBasic(MapWaveDataSO waves, GameObject basic)
        {
            for (int w = 0; w < waves.waves.Count; w++)
            {
                List<EnemySpawnEntry> entries = waves.waves[w].enemies;
                for (int e = 0; e < entries.Count; e++)
                {
                    if (entries[e].enemyPrefab != null && entries[e].enemyPrefab != basic)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // One reader per level scene, on its own object, pointed at the actions asset.
        private static void WireClickReader(string scenePath)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(ActionsPath);

            SilverfishClickReader reader = AlphaRunFrameBuilder.FindInScene<SilverfishClickReader>(scene);
            if (reader == null)
            {
                reader = new GameObject("Silverfish Click Reader").AddComponent<SilverfishClickReader>();
            }

            var so = new SerializedObject(reader);
            so.FindProperty("_actions").objectReferenceValue = actions;
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        // Loads everything again and says what it holds, so a batch log shows the truth.
        private static void ReportAfterReload()
        {
            for (int i = 0; i < Enemies.Length; i++)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Enemies[i].PrefabPath);
                if (prefab == null)
                {
                    Debug.LogError($"{Enemies[i].PrefabName} is missing after the build.");
                    continue;
                }

                BasicEnemy enemy = prefab.GetComponent<BasicEnemy>();
                string parts = $"{enemy.GetType().Name}"
                    + (prefab.GetComponent<RegisteredEnemy>() != null ? ", registered" : ", NOT registered")
                    + (prefab.GetComponent<Bounty>() != null ? $", bounty {prefab.GetComponent<Bounty>().Amount}" : ", NO bounty")
                    + (prefab.GetComponent<Collider2D>() != null ? ", collider" : "")
                    + (PrefabUtility.GetPrefabAssetType(prefab) == PrefabAssetType.Variant ? ", variant" : "");
                Debug.Log($"{Enemies[i].PrefabName}: {parts}; health {enemy.MaxHealth}, speed {enemy.MoveSpeed}");
            }

            for (int i = 0; i < Levels.Length; i++)
            {
                var waves = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(Levels[i].waves);
                if (waves == null)
                {
                    continue;
                }

                var text = new StringBuilder($"{waves.name}: {waves.waves.Count} waves");
                for (int w = 0; w < waves.waves.Count; w++)
                {
                    text.Append($"\n  {waves.waves[w].waveName}: {Describe(waves.waves[w])}");
                }
                Debug.Log(text.ToString());
            }

            for (int i = 0; i < Levels.Length; i++)
            {
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Levels[i].scene) == null)
                {
                    continue;
                }

                Scene scene = EditorSceneManager.OpenScene(Levels[i].scene, OpenSceneMode.Single);
                SilverfishClickReader reader = AlphaRunFrameBuilder.FindInScene<SilverfishClickReader>(scene);
                string actions = reader != null ? AlphaRunFrameBuilder.Name(new SerializedObject(reader), "_actions") : "no reader";
                Debug.Log($"{Levels[i].id}: Silverfish click reader actions = {actions}");
            }
        }

        // "Enemy_Basic x4, Enemy_Scarab x8", in order of first appearance.
        internal static string Describe(Wave wave)
        {
            var names = new List<string>();
            var counts = new Dictionary<string, int>();
            for (int e = 0; e < wave.enemies.Count; e++)
            {
                string name = wave.enemies[e].enemyPrefab != null ? wave.enemies[e].enemyPrefab.name : "none";
                if (!counts.ContainsKey(name))
                {
                    names.Add(name);
                    counts[name] = 0;
                }
                counts[name]++;
            }

            var text = new StringBuilder();
            for (int i = 0; i < names.Count; i++)
            {
                text.Append(i > 0 ? ", " : "").Append(names[i]).Append(" x").Append(counts[names[i]]);
            }
            return text.ToString();
        }
    }
}
