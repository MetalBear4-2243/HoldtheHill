using System.Collections.Generic;
using System.IO;
using HoldTheHill.Features.Economy;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.GameFlow;
using HoldTheHill.Features.Towers;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// Sets up everything the Alpha core loop needs and nothing else: placeholder sprites, the
    /// first two tower prefabs and their shop data, a basic enemy prefab, a wave list for Hill01,
    /// and the wallet, placer and path in the Hill01 gameplay scene.
    /// </summary>
    /// <remarks>
    /// Safe to run again: assets that exist are kept (so Inspector tuning survives), scene
    /// objects are found by component and only their references are refreshed.
    /// </remarks>
    internal static class AlphaCoreLoopBuilder
    {
        private const string PlaceholderArtFolder = "Assets/_Game/Shared/Art/Placeholders";
        private const string TowerPrefabFolder = "Assets/_Game/Features/Towers/Prefabs";
        private const string TowerDataFolder = "Assets/_Game/Features/Towers/Data";
        private const string EnemyPrefabFolder = "Assets/_Game/Features/Enemies/Prefabs";
        private const string EnemyDataFolder = "Assets/_Game/Features/Enemies/Data";
        private const string ActionsPath = "Assets/_Game/Input/HoldTheHill.inputactions";
        private const string GameplayScenePath = "Assets/_Game/Scenes/Levels/Hill01/Hill01_Gameplay.unity";
        private const string Hill01WavesPath = EnemyDataFolder + "/Hill01_Waves.asset";

        private const int SpriteSize = 32;

        /// <summary>The basic enemy prefab the placeholder waves use.</summary>
        internal const string EnemyPrefabPath = EnemyPrefabFolder + "/Enemy_Basic.prefab";

        /// <summary>One placeholder wave: how many basic enemies, and the gap between them.</summary>
        internal readonly struct WaveSpec
        {
            public WaveSpec(int count, float delay)
            {
                Count = count;
                Delay = delay;
            }

            public int Count { get; }
            public float Delay { get; }
        }

        private static readonly WaveSpec[] Hill01Waves =
        {
            new WaveSpec(5, 1.2f), new WaveSpec(8, 0.9f), new WaveSpec(12, 0.6f),
        };

        /// <summary>How a tower attacks; decides which extra component or projectile its prefab gets.</summary>
        internal enum TowerKind
        {
            Melee,
            Shot,
            Lob,
            Slow,
            Mines
        }

        /// <summary>One of the GDD's six towers: its prefab, shop data and numbers for all three levels.</summary>
        internal readonly struct TowerSpec
        {
            public TowerSpec(string prefabName, string dataFile, string title, int cost, Color color, TowerKind kind,
                float range, float damage, float interval, TowerUpgrade level2, TowerUpgrade level3)
            {
                PrefabName = prefabName;
                DataFile = dataFile;
                Title = title;
                Cost = cost;
                Color = color;
                Kind = kind;
                Range = range;
                Damage = damage;
                Interval = interval;
                Level2 = level2;
                Level3 = level3;
            }

            public string PrefabName { get; }
            public string DataFile { get; }
            public string Title { get; }
            public int Cost { get; }
            public Color Color { get; }
            public TowerKind Kind { get; }
            public float Range { get; }
            public float Damage { get; }
            public float Interval { get; }
            public TowerUpgrade Level2 { get; }
            public TowerUpgrade Level3 { get; }

            public string PrefabPath => $"{TowerPrefabFolder}/{PrefabName}.prefab";
            public string DataPath => $"{TowerDataFolder}/{DataFile}.asset";
        }

        private static TowerUpgrade Up(int cost, float range, float damage, float interval)
        {
            return new TowerUpgrade { Cost = cost, Range = range, Damage = damage, FireInterval = interval };
        }

        /// <summary>The six towers of the GDD, in number-key and shop order, at the GDD costs.</summary>
        internal static readonly TowerSpec[] Towers =
        {
            new TowerSpec("Tower_Ant", "Ant", "Ant", 75, new Color(0.55f, 0.35f, 0.15f), TowerKind.Melee,
                1.2f, 8f, 0.8f, Up(45, 1.3f, 12f, 0.7f), Up(70, 1.4f, 18f, 0.6f)),
            new TowerSpec("Tower_BulletAnt", "BulletAnt", "Bullet Ant", 100, new Color(0.2f, 0.2f, 0.25f), TowerKind.Shot,
                3f, 12f, 1.2f, Up(60, 3.3f, 18f, 1.1f), Up(90, 3.6f, 26f, 1f)),
            new TowerSpec("Tower_CrazyAnt", "CrazyAnt", "Crazy Ant", 125, new Color(0.9f, 0.55f, 0.2f), TowerKind.Melee,
                1.2f, 6f, 0.35f, Up(75, 1.3f, 9f, 0.3f), Up(110, 1.4f, 13f, 0.25f)),
            new TowerSpec("Tower_FireAnt", "FireAnt", "Fire Ant", 200, new Color(0.85f, 0.2f, 0.1f), TowerKind.Lob,
                2.8f, 14f, 1.6f, Up(120, 3f, 20f, 1.5f), Up(180, 3.3f, 30f, 1.4f)),
            new TowerSpec("Tower_SapAnt", "SapAnt", "Sap Ant", 225, new Color(0.3f, 0.7f, 0.3f), TowerKind.Slow,
                2f, 0f, 1f, Up(135, 2.4f, 0f, 1f), Up(200, 2.8f, 0f, 1f)),
            new TowerSpec("Tower_CarpenterAnt", "CarpenterAnt", "Carpenter Ant", 250, new Color(0.45f, 0.3f, 0.5f), TowerKind.Mines,
                2.5f, 30f, 3f, Up(150, 2.8f, 45f, 2.5f), Up(225, 3.1f, 65f, 2f)),
        };

        [MenuItem("Tools/Hold the Hill/Alpha/Build Core Loop (Hill01)")]
        private static void Build()
        {
            EnsureTowerAssets();

            Sprite circle = EnsureSprite("Circle", circle: true);
            GameObject enemy = EnsureEnemyPrefab(circle);
            EnsureWaves(Hill01WavesPath, "Hill01", enemy, Hill01Waves);

            WireGameplayScene(GameplayScenePath, Hill01WavesPath);

            AssetDatabase.SaveAssets();
            Debug.Log("Alpha core loop built: open Hill01_Gameplay and press Play. Keys 1 to 6 pick a tower, left-click builds, right-click cancels.");
        }

        /// <summary>Makes the sprites, projectiles, mine, the six tower prefabs and their shop data; keeps what exists.</summary>
        internal static void EnsureTowerAssets()
        {
            Sprite square = EnsureSprite("Square", circle: false);
            Sprite circle = EnsureSprite("Circle", circle: true);

            GameObject bullet = EnsureProjectilePrefab("Projectile_Bullet", circle, new Color(1f, 0.9f, 0.3f), lobbed: false);
            GameObject lob = EnsureProjectilePrefab("Projectile_Lob", circle, new Color(1f, 0.45f, 0.1f), lobbed: true);
            GameObject mine = EnsureMinePrefab(circle);

            for (int i = 0; i < Towers.Length; i++)
            {
                TowerSpec spec = Towers[i];
                Projectile projectile = spec.Kind == TowerKind.Shot ? bullet.GetComponent<Projectile>()
                    : spec.Kind == TowerKind.Lob ? lob.GetComponent<Projectile>() : null;
                EnsureTowerPrefab(spec, square, projectile, mine.GetComponent<Mine>());
                EnsureTowerData(spec.DataFile, spec.Title, spec.Cost, square);
            }

            AssetDatabase.SaveAssets();
        }

        /// <summary>Entry point for a batch run: <c>-executeMethod HoldTheHill.EditorTools.AlphaCoreLoopBuilder.BuildFromCommandLine</c>.</summary>
        public static void BuildFromCommandLine()
        {
            Build();
        }

        // A flat white square or disc, 32 pixels = 1 world unit, so prefabs can reference a real asset.
        internal static Sprite EnsureSprite(string name, bool circle)
        {
            EnsureFolder(PlaceholderArtFolder);
            string path = $"{PlaceholderArtFolder}/{name}.png";

            if (AssetDatabase.LoadAssetAtPath<Sprite>(path) == null)
            {
                var texture = new Texture2D(SpriteSize, SpriteSize, TextureFormat.RGBA32, false);
                var pixels = new Color32[SpriteSize * SpriteSize];
                float radius = SpriteSize * 0.5f;
                for (int y = 0; y < SpriteSize; y++)
                {
                    for (int x = 0; x < SpriteSize; x++)
                    {
                        float dx = x + 0.5f - radius;
                        float dy = y + 0.5f - radius;
                        bool inside = !circle || dx * dx + dy * dy <= radius * radius;
                        pixels[y * SpriteSize + x] = inside ? new Color32(255, 255, 255, 255) : new Color32(0, 0, 0, 0);
                    }
                }
                texture.SetPixels32(pixels);
                texture.Apply();

                File.WriteAllBytes(path, texture.EncodeToPNG());
                Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(path);

                var importer = (TextureImporter)AssetImporter.GetAtPath(path);
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = SpriteSize;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.mipmapEnabled = false;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static GameObject EnsureProjectilePrefab(string name, Sprite sprite, Color color, bool lobbed)
        {
            EnsureFolder(TowerPrefabFolder);
            string path = $"{TowerPrefabFolder}/{name}.prefab";

            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null)
            {
                return existing;
            }

            GameObject go = NewSpriteObject(name, sprite, color, lobbed ? 0.3f : 0.2f, sortingOrder: 20);
            var projectile = go.AddComponent<Projectile>();
            var so = new SerializedObject(projectile);
            so.FindProperty("_lobbed").boolValue = lobbed;
            if (lobbed)
            {
                so.FindProperty("_speed").floatValue = 6f;
                so.FindProperty("_arcHeight").floatValue = 1.5f;
                so.FindProperty("_splashRadius").floatValue = 1f;
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            return SavePrefab(go, path);
        }

        private static GameObject EnsureMinePrefab(Sprite sprite)
        {
            EnsureFolder(TowerPrefabFolder);
            string path = $"{TowerPrefabFolder}/Mine_Carpenter.prefab";

            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null)
            {
                return existing;
            }

            GameObject go = NewSpriteObject("Mine_Carpenter", sprite, new Color(0.35f, 0.25f, 0.2f), 0.3f, sortingOrder: 3);
            go.AddComponent<Mine>();
            return SavePrefab(go, path);
        }

        // A new prefab gets everything; one that exists only gains the upgrade levels it lacks, so
        // Inspector tuning of the level-1 numbers survives.
        private static GameObject EnsureTowerPrefab(TowerSpec spec, Sprite sprite, Projectile projectile, Mine mine)
        {
            EnsureFolder(TowerPrefabFolder);

            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(spec.PrefabPath);
            if (existing != null)
            {
                Tower existingTower = existing.GetComponent<Tower>();
                var existingSo = new SerializedObject(existingTower);
                if (existingSo.FindProperty("_upgrades").arraySize == 0)
                {
                    SetUpgrades(existingSo, spec);
                    existingSo.ApplyModifiedPropertiesWithoutUndo();
                    Debug.Log($"Added upgrade levels to the existing {spec.PrefabName}.");
                }
                return existing;
            }

            GameObject go = NewSpriteObject(spec.PrefabName, sprite, spec.Color, 0.8f, sortingOrder: 10);
            var tower = go.AddComponent<Tower>();
            var so = new SerializedObject(tower);
            so.FindProperty("_range").floatValue = spec.Range;
            so.FindProperty("_damage").floatValue = spec.Damage;
            so.FindProperty("_fireInterval").floatValue = spec.Interval;
            so.FindProperty("_projectilePrefab").objectReferenceValue = projectile;
            SetUpgrades(so, spec);
            so.ApplyModifiedPropertiesWithoutUndo();

            switch (spec.Kind)
            {
                case TowerKind.Slow:
                    go.AddComponent<SlowAura>();
                    break;
                case TowerKind.Mines:
                    var layer = go.AddComponent<MineLayer>();
                    var layerSo = new SerializedObject(layer);
                    layerSo.FindProperty("_minePrefab").objectReferenceValue = mine;
                    layerSo.ApplyModifiedPropertiesWithoutUndo();
                    break;
            }

            return SavePrefab(go, spec.PrefabPath);
        }

        private static void SetUpgrades(SerializedObject tower, TowerSpec spec)
        {
            SerializedProperty upgrades = tower.FindProperty("_upgrades");
            upgrades.arraySize = 2;
            SetUpgrade(upgrades.GetArrayElementAtIndex(0), spec.Level2);
            SetUpgrade(upgrades.GetArrayElementAtIndex(1), spec.Level3);
        }

        private static void SetUpgrade(SerializedProperty element, TowerUpgrade upgrade)
        {
            element.FindPropertyRelative("Cost").intValue = upgrade.Cost;
            element.FindPropertyRelative("Range").floatValue = upgrade.Range;
            element.FindPropertyRelative("Damage").floatValue = upgrade.Damage;
            element.FindPropertyRelative("FireInterval").floatValue = upgrade.FireInterval;
        }

        private static TowerData EnsureTowerData(string fileName, string towerName, int cost, Sprite icon)
        {
            EnsureFolder(TowerDataFolder);
            string path = $"{TowerDataFolder}/{fileName}.asset";

            TowerData data = AssetDatabase.LoadAssetAtPath<TowerData>(path);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<TowerData>();
                data.towerName = towerName;
                data.honeydewCost = cost;
                data.towerIcon = icon;
                AssetDatabase.CreateAsset(data, path);
                data = Reimport<TowerData>(path);
            }

            return data;
        }

        // A freshly created asset is not accepted as a serialized reference until it has been saved and
        // imported, so hand back the imported copy rather than the in-memory one.
        private static T Reimport<T>(string path) where T : Object
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }

        internal static GameObject EnsureEnemyPrefab(Sprite sprite)
        {
            EnsureFolder(EnemyPrefabFolder);
            string path = EnemyPrefabPath;

            GameObject existing = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (existing != null)
            {
                return existing;
            }

            GameObject go = NewSpriteObject("Enemy_Basic", sprite, new Color(0.85f, 0.25f, 0.2f), 0.6f, sortingOrder: 5);
            var enemy = go.AddComponent<BasicEnemy>();
            var so = new SerializedObject(enemy);
            so.FindProperty("_maxHealth").doubleValue = 30.0;
            so.FindProperty("_damage").intValue = 10;
            so.FindProperty("_moveSpeed").floatValue = 1.5f;
            so.ApplyModifiedPropertiesWithoutUndo();

            go.AddComponent<RegisteredEnemy>();
            go.AddComponent<Bounty>();
            return SavePrefab(go, path);
        }

        /// <summary>
        /// A wave list asset for one level, made only if it is missing so Inspector tuning survives.
        /// Placeholder contents: every wave is <paramref name="enemyPrefab"/> repeated.
        /// </summary>
        internal static MapWaveDataSO EnsureWaves(string path, string mapId, GameObject enemyPrefab, IReadOnlyList<WaveSpec> specs)
        {
            EnsureFolder(Path.GetDirectoryName(path).Replace('\\', '/'));

            MapWaveDataSO waves = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(path);
            if (waves == null)
            {
                waves = ScriptableObject.CreateInstance<MapWaveDataSO>();
                waves.mapId = mapId;
                waves.waves = new List<Wave>(specs.Count);
                for (int i = 0; i < specs.Count; i++)
                {
                    waves.waves.Add(MakeWave($"Wave {i + 1}", enemyPrefab, specs[i].Count, specs[i].Delay));
                }
                AssetDatabase.CreateAsset(waves, path);
                waves = Reimport<MapWaveDataSO>(path);
            }

            return waves;
        }

        private static Wave MakeWave(string name, GameObject enemyPrefab, int count, float delay)
        {
            var wave = new Wave { waveName = name };
            for (int i = 0; i < count; i++)
            {
                wave.enemies.Add(new EnemySpawnEntry { enemyPrefab = enemyPrefab, delayBeforeNext = delay });
            }
            return wave;
        }

        /// <summary>
        /// Opens a level's gameplay scene and makes sure it has a path, a spawner on the given wave
        /// list, a wallet with the wave reward, and a placer offering the Ant and Bullet Ant. Objects
        /// that exist are kept and only their references are refreshed. Saves the scene.
        /// </summary>
        internal static void WireGameplayScene(string scenePath, string wavesAssetPath)
        {
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // Opening a scene unloads assets nothing in a scene points at, which includes ScriptableObjects
            // held only in a local variable, so load them now, after the scene is open.
            var waves = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(wavesAssetPath);
            var datas = new TowerData[Towers.Length];
            var prefabs = new GameObject[Towers.Length];
            bool missing = waves == null;
            for (int i = 0; i < Towers.Length; i++)
            {
                datas[i] = AssetDatabase.LoadAssetAtPath<TowerData>(Towers[i].DataPath);
                prefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(Towers[i].PrefabPath);
                missing |= datas[i] == null || prefabs[i] == null;
            }
            if (missing)
            {
                Debug.LogError($"The wave or tower assets could not be loaded after opening {scenePath}. Run Build Core Loop first.");
                return;
            }

            RemoveBrokenObjects(scene);

            EnemyPath path = Object.FindAnyObjectByType<EnemyPath>();
            if (path == null)
            {
                path = HoldTheHillMenu.CreateExamplePathObject().GetComponent<EnemyPath>();
            }

            EnemySpawner spawner = Object.FindAnyObjectByType<EnemySpawner>();
            if (spawner == null)
            {
                // The scene already has an empty "Enemy Spawner" object; use it rather than add a twin.
                GameObject holder = FindEmptyRoot(scene, "Enemy Spawner");
                if (holder == null)
                {
                    holder = new GameObject("Enemy Spawner");
                }
                spawner = holder.AddComponent<EnemySpawner>();
            }

            // An earlier run may have left the original empty object beside the real spawner.
            GameObject spare = FindEmptyRoot(scene, "Enemy Spawner");
            if (spare != null && spare != spawner.gameObject)
            {
                Object.DestroyImmediate(spare);
            }

            var spawnerSo = new SerializedObject(spawner);
            spawnerSo.FindProperty("defaultSpawnPoint").objectReferenceValue =
                path.transform.childCount > 0 ? path.transform.GetChild(0) : null;
            // The player builds first; a wave starts from the button (later: Space).
            spawnerSo.FindProperty("autoStartFirstWave").boolValue = false;
            spawnerSo.ApplyModifiedPropertiesWithoutUndo();
            SetAssetReference(spawner, "activeMapConfigOverride", waves);

            WireNextWaveButton(spawner);

            HoneydewWallet wallet = Object.FindAnyObjectByType<HoneydewWallet>();
            if (wallet == null)
            {
                wallet = new GameObject("Honeydew Wallet").AddComponent<HoneydewWallet>();
            }

            WaveReward reward = wallet.GetComponent<WaveReward>();
            if (reward == null)
            {
                reward = wallet.gameObject.AddComponent<WaveReward>();
            }
            var rewardSo = new SerializedObject(reward);
            rewardSo.FindProperty("_spawner").objectReferenceValue = spawner;
            rewardSo.ApplyModifiedPropertiesWithoutUndo();

            TowerPlacer placer = Object.FindAnyObjectByType<TowerPlacer>();
            if (placer == null)
            {
                placer = new GameObject("Tower Placer").AddComponent<TowerPlacer>();
            }

            var placerSo = new SerializedObject(placer);
            placerSo.FindProperty("_path").objectReferenceValue = path;
            placerSo.FindProperty("_wallet").objectReferenceValue = wallet;
            placerSo.FindProperty("_actions").objectReferenceValue = AssetDatabase.LoadAssetAtPath<InputActionAsset>(ActionsPath);

            SerializedProperty offers = placerSo.FindProperty("_offers");
            offers.arraySize = Towers.Length;
            bool refused = false;
            for (int i = 0; i < Towers.Length; i++)
            {
                refused |= !SetOffer(offers.GetArrayElementAtIndex(i), datas[i], prefabs[i]);
            }
            placerSo.ApplyModifiedPropertiesWithoutUndo();

            if (refused)
            {
                // Same fallback as SetAssetReference: write the list straight into the field.
                var list = new List<TowerPlacer.Offer>(Towers.Length);
                for (int i = 0; i < Towers.Length; i++)
                {
                    list.Add(new TowerPlacer.Offer { Data = datas[i], Prefab = prefabs[i].GetComponent<Tower>() });
                }
                typeof(TowerPlacer).GetField("_offers", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                    ?.SetValue(placer, list);
                EditorUtility.SetDirty(placer);
            }

            // The run reaches the placer for the tower card and the U key (Alpha step 4).
            LevelRun run = Object.FindAnyObjectByType<LevelRun>();
            if (run != null)
            {
                var runSo = new SerializedObject(run);
                runSo.FindProperty("_placer").objectReferenceValue = placer;
                runSo.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            ReportOffersAfterReload(scenePath);
        }

        // False when Unity's type check refused the TowerData asset.
        private static bool SetOffer(SerializedProperty offer, TowerData data, GameObject prefab)
        {
            SerializedProperty dataProperty = offer.FindPropertyRelative("Data");
            dataProperty.objectReferenceValue = data;
            offer.FindPropertyRelative("Prefab").objectReferenceValue = prefab.GetComponent<Tower>();

            bool accepted = data == null || dataProperty.objectReferenceValue != null;
            if (!accepted)
            {
                Debug.LogWarning($"SerializedProperty refused '{data.name}' ({data.GetType().AssemblyQualifiedName}) for TowerPlacer offer data.");
            }
            return accepted;
        }

        private static GameObject FindEmptyRoot(Scene scene, string name)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (root.name == name && root.transform.childCount == 0 && root.GetComponents<Component>().Length == 1)
                {
                    return root;
                }
            }
            return null;
        }

        // Opens the saved scene again and says what the placer actually holds, so a batch log shows the truth.
        private static void ReportOffersAfterReload(string scenePath)
        {
            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            TowerPlacer placer = Object.FindAnyObjectByType<TowerPlacer>();
            if (placer == null)
            {
                Debug.LogError($"Tower Placer is missing after {scenePath} was saved and reopened.");
                return;
            }

            SerializedProperty offers = new SerializedObject(placer).FindProperty("_offers");
            for (int i = 0; i < offers.arraySize; i++)
            {
                SerializedProperty offer = offers.GetArrayElementAtIndex(i);
                Object data = offer.FindPropertyRelative("Data").objectReferenceValue;
                Object prefab = offer.FindPropertyRelative("Prefab").objectReferenceValue;
                Debug.Log($"Offer {i + 1}: data = {(data != null ? data.name : "none")}, prefab = {(prefab != null ? prefab.name : "none")}");
            }

            EnemySpawner spawner = Object.FindAnyObjectByType<EnemySpawner>();
            if (spawner != null)
            {
                var waves = new SerializedObject(spawner).FindProperty("activeMapConfigOverride").objectReferenceValue as MapWaveDataSO;
                Debug.Log($"Spawner waves = {(waves != null ? $"{waves.name} ({waves.waves.Count} waves)" : "none")}");
            }
        }

        // Assigns an asset reference. The SerializedProperty route is tried first; if Unity's type check
        // refuses the asset, the field is set directly and the reason is logged so it can be chased.
        private static void SetAssetReference(Object owner, string fieldName, Object value)
        {
            var so = new SerializedObject(owner);
            SerializedProperty property = so.FindProperty(fieldName);
            property.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
            if (value == null || property.objectReferenceValue != null)
            {
                return;
            }

            System.Reflection.FieldInfo field = owner.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            Debug.LogWarning($"SerializedProperty refused '{value.name}' ({value.GetType().AssemblyQualifiedName}) for {owner.GetType().Name}.{fieldName} "
                             + $"(field type {field?.FieldType.AssemblyQualifiedName}); setting the field directly.");
            field?.SetValue(owner, value);
            EditorUtility.SetDirty(owner);
        }

        // The Indev copy of this scene carries objects whose prefab or script no longer exists; they
        // can't be fixed or used, only removed.
        private static void RemoveBrokenObjects(Scene scene)
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                if (PrefabUtility.IsPrefabAssetMissing(root))
                {
                    Debug.Log($"Removed '{root.name}': its prefab asset is missing.");
                    Object.DestroyImmediate(root);
                    continue;
                }

                // Children first, so a parent emptied by the sweep is seen empty.
                Transform[] all = root.GetComponentsInChildren<Transform>(true);
                for (int i = all.Length - 1; i >= 0; i--)
                {
                    RemoveMissingScripts(all[i].gameObject);
                }
            }
        }

        private static void RemoveMissingScripts(GameObject go)
        {
            // A missing script shows up as a null entry in the component list.
            int missing = 0;
            foreach (Component component in go.GetComponents<Component>())
            {
                if (component == null)
                {
                    missing++;
                }
            }

            if (missing == 0)
            {
                return;
            }

            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            if (go.transform.childCount == 0 && go.GetComponents<Component>().Length == 1)
            {
                Debug.Log($"Removed '{go.name}': only a missing script was on it.");
                Object.DestroyImmediate(go);
            }
            else
            {
                Debug.Log($"Removed a missing script from '{go.name}'.");
            }
        }

        private static void WireNextWaveButton(EnemySpawner spawner)
        {
            var button = Object.FindAnyObjectByType<UnityEngine.UI.Button>();
            if (button == null)
            {
                return;
            }

            // The old listener pointed at a spawner class that no longer exists.
            for (int i = button.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
            {
                UnityEventTools.RemovePersistentListener(button.onClick, i);
            }
            UnityEventTools.AddVoidPersistentListener(button.onClick, spawner.StartNextWave);
        }

        private static GameObject NewSpriteObject(string name, Sprite sprite, Color color, float scale, int sortingOrder)
        {
            var go = new GameObject(name);
            go.transform.localScale = Vector3.one * scale;
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = sortingOrder;
            return go;
        }

        private static GameObject SavePrefab(GameObject go, string path)
        {
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            return prefab;
        }

        internal static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
    }
}
