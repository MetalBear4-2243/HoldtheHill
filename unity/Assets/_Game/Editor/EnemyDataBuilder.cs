using System;
using System.Collections.Generic;
using System.Text;
using HoldTheHill.Core.Animation;
using HoldTheHill.Features.Economy;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.GameFlow;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// Enemies tuned in data and assembled from prefab variants (2026-11-06). Makes one
    /// <see cref="EnemyData"/> asset per enemy prefab holding the numbers that prefab had, the
    /// shared Enemy_Base prefab, and turns each enemy prefab into a variant of it that overrides
    /// only its data asset, its art and its ability components. A rebuilt prefab keeps its GUID
    /// but its objects get new ids, so every reference to the prefabs (the wave lists, the Queen
    /// Mantis's spawn, the dev keys in the UI scene) is pointed at the rebuilt prefabs again.
    /// </summary>
    /// <remarks>
    /// Safe to run again: a data asset that exists is kept (Inspector tuning survives), Enemy_Base
    /// is made only when missing, and a prefab that is already a variant of Enemy_Base is left
    /// alone. The Queen Mantis keeps her own prefab: her <see cref="QueenMantisBoss"/> replaces
    /// <see cref="BasicEnemy"/>, which a variant cannot do; she only gets her data asset. The old
    /// species subclasses (CentipedeEnemy, RhinoBeetleEnemy, RollyPollyEnemy, WaspEnemy,
    /// SilverfishEnemy) are read by field name, so this still compiles once they are gone.
    /// </remarks>
    internal static class EnemyDataBuilder
    {
        private const string PrefabFolder = "Assets/_Game/Features/Enemies/Prefabs";
        private const string BasePath = PrefabFolder + "/Enemy_Base.prefab";
        private const string DataFolder = "Assets/_Game/Features/Enemies/Data/Enemies";
        private const string QueenMantisDataPath = "Assets/_Game/Features/Enemies/Data/QueenMantisData.asset";
        private const string UiScenePath = "Assets/_Game/Scenes/UI.unity";
        private const string BodyName = "Body";

        private static readonly string[] WaveAssets =
        {
            "Assets/_Game/Features/Enemies/Data/L00_TutorialHill_Waves.asset",
            "Assets/_Game/Features/Enemies/Data/L01_Meadow_Waves.asset",
            "Assets/_Game/Features/Enemies/Data/Hill01_Waves.asset",
            "Assets/_Game/Features/Enemies/Scripts/EnemySpawner/Map_01.asset",
        };

        // Scarab and Mantis Spawn first: today they are variants of Enemy_Basic, and their numbers
        // can only be read while Enemy_Basic is still the prefab they were made from.
        private static readonly string[] Enemies =
        {
            "Scarab", "MantisSpawn", "Basic", "Centipede", "RhinoBeetle", "RollyPolly", "Silverfish", "Wasp", "QueenMantis",
        };

        private const string QueenMantis = "QueenMantis";

        // The components every enemy has from Enemy_Base; a variant never drops them.
        private static readonly Type[] KeptOnEveryEnemy = { typeof(BasicEnemy), typeof(RegisteredEnemy), typeof(Bounty) };

        // Where a wave list pointed before the rebuild, by prefab path.
        private readonly struct WaveReference
        {
            public WaveReference(string asset, int wave, int entry, string prefabPath)
            {
                Asset = asset;
                Wave = wave;
                Entry = entry;
                PrefabPath = prefabPath;
            }

            public string Asset { get; }
            public int Wave { get; }
            public int Entry { get; }
            public string PrefabPath { get; }
        }

        [MenuItem("Tools/Hold the Hill/Alpha/Build Enemy Data (data assets, Enemy_Base, variants)")]
        private static void Build()
        {
            // From the menu, the open scenes are about to be replaced: offer to save them first.
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            BuildAll();
        }

        /// <summary>Entry point for a batch run: <c>-executeMethod HoldTheHill.EditorTools.EnemyDataBuilder.BuildFromCommandLine</c>.</summary>
        public static void BuildFromCommandLine()
        {
            BuildAll();
        }

        private static void BuildAll()
        {
            List<WaveReference> waveReferences = RecordWaveReferences();
            string spawnPrefabPath = RecordSpawnPrefab();

            AlphaCoreLoopBuilder.EnsureFolder(DataFolder);
            for (int i = 0; i < Enemies.Length; i++)
            {
                EnsureData(Enemies[i]);
            }
            AssetDatabase.SaveAssets();

            GameObject basePrefab = EnsureBase();
            for (int i = 0; i < Enemies.Length; i++)
            {
                if (Enemies[i] == QueenMantis)
                {
                    WireQueenMantis();
                }
                else
                {
                    ConvertToVariant(Enemies[i], basePrefab);
                }
            }
            AssetDatabase.SaveAssets();

            RestoreWaveReferences(waveReferences);
            RestoreSpawnPrefab(spawnPrefabPath);
            AssetDatabase.SaveAssets();

            WireDevKeys();
            ReportAfterReload();
            Debug.Log("Enemy data built: one data asset per enemy, Enemy_Base, and the enemies as its variants.");
        }

        private static string PrefabPath(string enemy) => $"{PrefabFolder}/Enemy_{enemy}.prefab";

        private static string DataPath(string enemy) => $"{DataFolder}/{enemy}.asset";

        // ---- References -----------------------------------------------------------------------

        private static List<WaveReference> RecordWaveReferences()
        {
            var references = new List<WaveReference>();
            for (int a = 0; a < WaveAssets.Length; a++)
            {
                var waves = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(WaveAssets[a]);
                if (waves == null)
                {
                    continue;
                }

                for (int w = 0; w < waves.waves.Count; w++)
                {
                    List<EnemySpawnEntry> entries = waves.waves[w].enemies;
                    for (int e = 0; e < entries.Count; e++)
                    {
                        if (entries[e].enemyPrefab == null)
                        {
                            continue;
                        }

                        references.Add(new WaveReference(WaveAssets[a], w, e, AssetDatabase.GetAssetPath(entries[e].enemyPrefab)));
                    }
                }
            }
            return references;
        }

        private static void RestoreWaveReferences(List<WaveReference> references)
        {
            for (int i = 0; i < references.Count; i++)
            {
                WaveReference reference = references[i];
                var waves = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(reference.Asset);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(reference.PrefabPath);
                if (waves == null || prefab == null)
                {
                    Debug.LogError($"{reference.Asset} wave {reference.Wave} entry {reference.Entry}: {reference.PrefabPath} could not be pointed at again.");
                    continue;
                }

                waves.waves[reference.Wave].enemies[reference.Entry].enemyPrefab = prefab;
                EditorUtility.SetDirty(waves);
            }
        }

        private static string RecordSpawnPrefab()
        {
            var data = AssetDatabase.LoadAssetAtPath<QueenMantisData>(QueenMantisDataPath);
            return data != null && data.SpawnPrefab != null ? AssetDatabase.GetAssetPath(data.SpawnPrefab) : null;
        }

        private static void RestoreSpawnPrefab(string prefabPath)
        {
            if (string.IsNullOrEmpty(prefabPath))
            {
                return;
            }

            var data = AssetDatabase.LoadAssetAtPath<QueenMantisData>(QueenMantisDataPath);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (data == null || prefab == null)
            {
                Debug.LogError($"QueenMantisData.SpawnPrefab could not be pointed at {prefabPath} again.");
                return;
            }

            data.SpawnPrefab = prefab.GetComponent<BasicEnemy>();
            EditorUtility.SetDirty(data);
        }

        // ---- Data assets ----------------------------------------------------------------------

        // The numbers the prefab has today, copied once. A subclass's own fields are read by name,
        // so the Wasp's flying flag and the Silverfish's click rule come across too.
        private static EnemyData EnsureData(string enemy)
        {
            var data = AssetDatabase.LoadAssetAtPath<EnemyData>(DataPath(enemy));
            if (data != null)
            {
                Debug.Log($"{enemy}.asset exists; kept as it is.");
                return data;
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath(enemy));
            BasicEnemy basic = prefab != null ? prefab.GetComponent<BasicEnemy>() : null;
            if (basic == null)
            {
                Debug.LogError($"{PrefabPath(enemy)} is missing or has no BasicEnemy; no data asset made.");
                return null;
            }

            var so = new SerializedObject(basic);
            data = ScriptableObject.CreateInstance<EnemyData>();
            data.MaxHealth = so.FindProperty("_maxHealth").doubleValue;
            data.Damage = so.FindProperty("_damage").intValue;
            data.MoveSpeed = so.FindProperty("_moveSpeed").floatValue;
            data.WaypointTolerance = so.FindProperty("_waypointTolerance").floatValue;
            data.FlipSpriteFacing = so.FindProperty("_flipSpriteFacing").boolValue;

            Bounty bounty = prefab.GetComponent<Bounty>();
            data.Honeydew = bounty != null ? new SerializedObject(bounty).FindProperty("_honeydew").intValue : 0;

            SerializedProperty flying = so.FindProperty("_isFlying");
            data.IsFlying = flying != null && flying.boolValue;
            data.DiesToClick = basic.GetType().Name == "SilverfishEnemy";

            AssetDatabase.CreateAsset(data, DataPath(enemy));
            Debug.Log($"Made {enemy}.asset from {prefab.name} ({basic.GetType().Name}): {Describe(data)}.");
            return data;
        }

        private static string Describe(EnemyData data)
        {
            return $"health {data.MaxHealth}, damage {data.Damage}, speed {data.MoveSpeed}, tolerance {data.WaypointTolerance}, "
                + $"flip {data.FlipSpriteFacing}, honeydew {data.Honeydew}, flying {data.IsFlying}, dies to click {data.DiesToClick}";
        }

        // ---- Enemy_Base -----------------------------------------------------------------------

        // The shared parts of every enemy, with no art: BasicEnemy, the registry entry, the bounty,
        // the Body that turns to face the road (FaceTravel) and loops its walk (SpriteFlipbook), and
        // the hurt and death rows. Enemy_Basic's Body lends its material and sorting.
        private static GameObject EnsureBase()
        {
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(BasePath);
            if (existing != null)
            {
                Debug.Log("Enemy_Base exists; kept as it is.");
                return existing;
            }

            var basic = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath("Basic"));
            SpriteRenderer model = basic != null ? basic.GetComponentInChildren<SpriteRenderer>() : null;
            FaceTravel faceModel = basic != null ? basic.GetComponent<FaceTravel>() : null;

            var root = new GameObject("Enemy_Base");
            var enemy = root.AddComponent<BasicEnemy>();
            var enemySo = new SerializedObject(enemy);
            // Every enemy sheet's Body turns to face the road, so Jason's left-right flip is off.
            enemySo.FindProperty("_flipSpriteFacing").boolValue = false;
            enemySo.ApplyModifiedPropertiesWithoutUndo();
            root.AddComponent<RegisteredEnemy>();
            root.AddComponent<Bounty>();

            var body = new GameObject(BodyName);
            body.transform.SetParent(root.transform, worldPositionStays: false);
            var renderer = body.AddComponent<SpriteRenderer>();
            if (model != null)
            {
                renderer.sharedMaterial = model.sharedMaterial;
                renderer.sortingLayerID = model.sortingLayerID;
                renderer.sortingOrder = model.sortingOrder;
            }
            var walk = body.AddComponent<SpriteFlipbook>();
            SetReference(walk, "_renderer", renderer);

            var face = root.AddComponent<FaceTravel>();
            if (faceModel != null)
            {
                CopyValues(faceModel, face, null, null);
            }
            SetReference(face, "_body", body.transform);

            var hurtAndDeath = root.AddComponent<EnemyHurtAndDeath>();
            SetReference(hurtAndDeath, "_body", renderer);
            SetReference(hurtAndDeath, "_walk", walk);

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, BasePath);
            Object.DestroyImmediate(root);
            Debug.Log("Made Enemy_Base (BasicEnemy, RegisteredEnemy, Bounty, FaceTravel, EnemyHurtAndDeath; Body with SpriteRenderer and SpriteFlipbook).");
            return prefab;
        }

        // ---- Variants -------------------------------------------------------------------------

        // The prefab becomes a variant of Enemy_Base at the same path, so its GUID survives: an
        // instance of the base takes over the old prefab's scale, art, extra children and extra
        // components, and its species subclass becomes an ability component. The base's numbers are
        // not copied onto the variant: its data asset carries them.
        private static void ConvertToVariant(string enemy, GameObject basePrefab)
        {
            string path = PrefabPath(enemy);
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset == null)
            {
                Debug.LogError($"{path} is missing; nothing to convert.");
                return;
            }

            if (PrefabUtility.GetPrefabAssetType(asset) == PrefabAssetType.Variant
                && PrefabUtility.GetCorrespondingObjectFromSource(asset) == basePrefab)
            {
                Debug.Log($"{asset.name} is already a variant of Enemy_Base; kept as it is.");
                return;
            }

            var data = AssetDatabase.LoadAssetAtPath<EnemyData>(DataPath(enemy));
            GameObject old = PrefabUtility.LoadPrefabContents(path);
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);
            string species;
            try
            {
                instance.name = old.name;
                species = old.GetComponent<BasicEnemy>().GetType().Name;
                CopyObject(old, instance, old.transform, instance.transform);
                RemoveWhatTheOldPrefabLacked(old, instance);
                AddAbility(old, instance, species);
                SetReference(instance.GetComponent<BasicEnemy>(), "_enemyData", data);
                PrefabUtility.RemoveUnusedOverrides(new[] { instance }, InteractionMode.AutomatedAction);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(old);
            }

            PrefabUtility.SaveAsPrefabAsset(instance, path);
            Object.DestroyImmediate(instance);
            Debug.Log($"Rebuilt Enemy_{enemy} as a variant of Enemy_Base (was {species}).");
        }

        // The Queen Mantis's prefab is kept as it is (her boss class replaces BasicEnemy, which no
        // variant can do); only her data asset is wired in.
        private static void WireQueenMantis()
        {
            string path = PrefabPath(QueenMantis);
            var data = AssetDatabase.LoadAssetAtPath<EnemyData>(DataPath(QueenMantis));
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) == null || data == null)
            {
                Debug.LogError("Enemy_QueenMantis or QueenMantis.asset is missing; her data is not wired.");
                return;
            }

            GameObject root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                SetReference(root.GetComponent<BasicEnemy>(), "_enemyData", data);
                PrefabUtility.SaveAsPrefabAsset(root, path);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
            Debug.Log("Enemy_QueenMantis keeps her own prefab; QueenMantis.asset wired into her QueenMantisBoss.");
        }

        // Layer, tag, transform, components and children, old onto new. A child or component the
        // new object lacks is added; BasicEnemy and its subclasses are left to the base and the
        // abilities. References into the old prefab are pointed at the same places in the new one.
        private static void CopyObject(GameObject old, GameObject target, Transform oldRoot, Transform newRoot)
        {
            target.layer = old.layer;
            target.tag = old.tag;
            target.SetActive(old.activeSelf);
            target.transform.localPosition = old.transform.localPosition;
            target.transform.localRotation = old.transform.localRotation;
            target.transform.localScale = old.transform.localScale;

            Component[] components = old.GetComponents<Component>();
            var seen = new Dictionary<Type, int>();
            for (int i = 0; i < components.Length; i++)
            {
                Component source = components[i];
                if (source == null || source is Transform || source is BasicEnemy)
                {
                    continue;
                }

                Type type = source.GetType();
                seen.TryGetValue(type, out int index);
                seen[type] = index + 1;

                Component destination = NthOfType(target, type, index);
                if (destination == null)
                {
                    destination = target.AddComponent(type);
                }
                CopyValues(source, destination, oldRoot, newRoot);
            }

            for (int i = 0; i < old.transform.childCount; i++)
            {
                Transform oldChild = old.transform.GetChild(i);
                Transform newChild = target.transform.Find(oldChild.name);
                if (newChild == null)
                {
                    newChild = new GameObject(oldChild.name).transform;
                    newChild.SetParent(target.transform, worldPositionStays: false);
                    newChild.SetSiblingIndex(i);
                }
                CopyObject(oldChild.gameObject, newChild.gameObject, oldRoot, newRoot);
            }
        }

        // A component Enemy_Base carries that the old prefab did not have (the Rolly Polly has no
        // hurt and death rows) is removed from the variant, so the enemy stays as it was.
        private static void RemoveWhatTheOldPrefabLacked(GameObject old, GameObject target)
        {
            Component[] components = target.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                if (component == null || component is Transform || Array.IndexOf(KeptOnEveryEnemy, component.GetType()) >= 0)
                {
                    continue;
                }

                if (old.GetComponent(component.GetType()) == null)
                {
                    Debug.Log($"{old.name}: Enemy_Base's {component.GetType().Name} removed, as the prefab had none.");
                    Object.DestroyImmediate(component);
                }
            }
        }

        // The species subclass's own fields, by name, onto the matching ability component.
        private static void AddAbility(GameObject old, GameObject target, string species)
        {
            var oldEnemy = new SerializedObject(old.GetComponent<BasicEnemy>());
            switch (species)
            {
                case "CentipedeEnemy":
                    CopyNamed(oldEnemy, Ensure<CentipedeSplitter>(target), old.transform, target.transform,
                        "_splitsRemaining", "_splitPrefab", "_sizeMultiplier", "_splitForwardDistance");
                    break;
                case "RhinoBeetleEnemy":
                    CopyNamed(oldEnemy, Ensure<RhinoBeetleShield>(target), old.transform, target.transform,
                        "_shieldCapacity", "_shieldRegrowDelay", "_shieldRegrowPerSecond", "_shieldVisual");
                    break;
                case "RollyPollyEnemy":
                    CopyNamed(oldEnemy, Ensure<RollyPollyCurler>(target), old.transform, target.transform,
                        "_maxCurls", "_rollDistance", "_curledDamageShare", "_curledSpeedMultiplier", "_curledScale", "_curledTint");
                    break;
                default:
                    // Basic, Scarab, Mantis Spawn: nothing of their own. Wasp and Silverfish: their rule is a flag in the data.
                    break;
            }
        }

        private static T Ensure<T>(GameObject go) where T : Component
        {
            T component = go.GetComponent<T>();
            return component != null ? component : go.AddComponent<T>();
        }

        private static Component NthOfType(GameObject go, Type type, int index)
        {
            Component[] all = go.GetComponents<Component>();
            int seen = 0;
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i] != null && all[i].GetType() == type)
                {
                    if (seen == index)
                    {
                        return all[i];
                    }
                    seen++;
                }
            }
            return null;
        }

        // Every visible serialized field of source onto destination (same type), then references
        // into the old prefab mapped into the new one.
        private static void CopyValues(Component source, Component destination, Transform oldRoot, Transform newRoot)
        {
            var from = new SerializedObject(source);
            var to = new SerializedObject(destination);
            SerializedProperty property = from.GetIterator();
            bool enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (IsBookkeeping(property.name))
                {
                    continue;
                }
                to.CopyFromSerializedProperty(property);
            }
            RemapReferences(to, oldRoot, newRoot);
            to.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CopyNamed(SerializedObject from, Component destination, Transform oldRoot, Transform newRoot, params string[] names)
        {
            var to = new SerializedObject(destination);
            for (int i = 0; i < names.Length; i++)
            {
                SerializedProperty property = from.FindProperty(names[i]);
                if (property == null)
                {
                    Debug.LogWarning($"{from.targetObject.name}: no {names[i]} to copy onto {destination.GetType().Name}; its default stays.");
                    continue;
                }
                to.CopyFromSerializedProperty(property);
            }
            RemapReferences(to, oldRoot, newRoot);
            to.ApplyModifiedPropertiesWithoutUndo();
        }

        private static bool IsBookkeeping(string name)
        {
            return name == "m_Script" || name == "m_GameObject" || name == "m_ObjectHideFlags" || name == "m_EditorHideFlags"
                || name == "m_EditorClassIdentifier" || name.StartsWith("m_Prefab", StringComparison.Ordinal)
                || name.StartsWith("m_Corresponding", StringComparison.Ordinal);
        }

        // An object reference into the old hierarchy becomes the object at the same path (and the
        // same component slot) in the new one; asset references are left alone.
        private static void RemapReferences(SerializedObject so, Transform oldRoot, Transform newRoot)
        {
            if (oldRoot == null || newRoot == null)
            {
                return;
            }

            SerializedProperty property = so.GetIterator();
            while (property.Next(true))
            {
                if (property.propertyType != SerializedPropertyType.ObjectReference || property.objectReferenceValue == null
                    || property.name == "m_GameObject")
                {
                    continue;
                }

                Object mapped = Mapped(property.objectReferenceValue, oldRoot, newRoot);
                if (mapped != null)
                {
                    property.objectReferenceValue = mapped;
                }
            }
        }

        private static Object Mapped(Object value, Transform oldRoot, Transform newRoot)
        {
            Transform where = value is GameObject go ? go.transform : value is Component component ? component.transform : null;
            if (where == null || where.root != oldRoot)
            {
                return null;
            }

            Transform target = SamePlace(where, oldRoot, newRoot);
            if (target == null)
            {
                Debug.LogWarning($"{value.name} ({value.GetType().Name}) has no match in the rebuilt prefab; the reference is left dangling.");
                return null;
            }

            if (value is GameObject)
            {
                return target.gameObject;
            }

            Component[] oldOnes = where.GetComponents(value.GetType());
            int index = Array.IndexOf(oldOnes, value);
            Component[] newOnes = target.GetComponents(value.GetType());
            return index >= 0 && index < newOnes.Length ? newOnes[index] : null;
        }

        private static Transform SamePlace(Transform old, Transform oldRoot, Transform newRoot)
        {
            if (old == oldRoot)
            {
                return newRoot;
            }

            Transform parent = SamePlace(old.parent, oldRoot, newRoot);
            return parent != null ? parent.Find(old.name) : null;
        }

        private static void SetReference(Object target, string field, Object value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(field).objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // ---- UI scene -------------------------------------------------------------------------

        // The dev keys (F5 centipede, F8 Queen Mantis) point at components inside the prefabs.
        private static void WireDevKeys()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(UiScenePath) == null)
            {
                return;
            }

            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
            DevKeys keys = AlphaRunFrameBuilder.FindInScene<DevKeys>(ui);
            if (keys == null)
            {
                Debug.LogWarning("UI scene has no DevKeys; nothing to point at the rebuilt prefabs.");
                return;
            }

            // Loaded by path after the scene opened, so nothing came loose with the old scene.
            var centipede = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath("Centipede"));
            var queen = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath(QueenMantis));
            var so = new SerializedObject(keys);
            so.FindProperty("_centipedePrefab").objectReferenceValue = centipede != null ? centipede.GetComponent<BasicEnemy>() : null;
            so.FindProperty("_queenMantisPrefab").objectReferenceValue = queen != null ? queen.GetComponent<BasicEnemy>() : null;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.MarkSceneDirty(ui);
            EditorSceneManager.SaveScene(ui);
        }

        // ---- Report ---------------------------------------------------------------------------

        // Loads everything again and says what it holds, so a batch log shows the truth.
        private static void ReportAfterReload()
        {
            var basePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BasePath);
            Debug.Log(basePrefab != null ? $"Enemy_Base: {Parts(basePrefab)}" : "Enemy_Base is MISSING.");

            for (int i = 0; i < Enemies.Length; i++)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath(Enemies[i]));
                var data = AssetDatabase.LoadAssetAtPath<EnemyData>(DataPath(Enemies[i]));
                if (prefab == null)
                {
                    Debug.LogError($"Enemy_{Enemies[i]} is missing after the build.");
                    continue;
                }

                BasicEnemy enemy = prefab.GetComponent<BasicEnemy>();
                string wired = enemy != null ? AlphaRunFrameBuilder.Name(new SerializedObject(enemy), "_enemyData") : "NO BasicEnemy";
                Object source = PrefabUtility.GetCorrespondingObjectFromSource(prefab);
                string kind = PrefabUtility.GetPrefabAssetType(prefab) == PrefabAssetType.Variant && source != null
                    ? $"variant of {source.name}"
                    : "own prefab";
                Debug.Log($"Enemy_{Enemies[i]}: {kind}; data {wired} ({(data != null ? Describe(data) : "MISSING")}); {Parts(prefab)}");
            }

            for (int a = 0; a < WaveAssets.Length; a++)
            {
                var waves = AssetDatabase.LoadAssetAtPath<MapWaveDataSO>(WaveAssets[a]);
                if (waves == null)
                {
                    continue;
                }

                var text = new StringBuilder($"{waves.name}: {waves.waves.Count} waves");
                for (int w = 0; w < waves.waves.Count; w++)
                {
                    text.Append($"\n  {waves.waves[w].waveName}: {AlphaEnemiesBuilder.Describe(waves.waves[w])}");
                }
                Debug.Log(text.ToString());
            }

            var queenData = AssetDatabase.LoadAssetAtPath<QueenMantisData>(QueenMantisDataPath);
            Debug.Log(queenData != null
                ? $"QueenMantisData.SpawnPrefab = {(queenData.SpawnPrefab != null ? queenData.SpawnPrefab.name : "none")}"
                : "QueenMantisData is missing.");

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(UiScenePath) != null)
            {
                Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
                DevKeys keys = AlphaRunFrameBuilder.FindInScene<DevKeys>(ui);
                if (keys != null)
                {
                    var so = new SerializedObject(keys);
                    Debug.Log($"DevKeys: centipede = {AlphaRunFrameBuilder.Name(so, "_centipedePrefab")}, Queen Mantis = {AlphaRunFrameBuilder.Name(so, "_queenMantisPrefab")}");
                }
            }
        }

        // "scale 0.9; BasicEnemy, RegisteredEnemy, ...; children Body (SpriteRenderer, SpriteFlipbook)"
        private static string Parts(GameObject prefab)
        {
            var text = new StringBuilder($"scale {prefab.transform.localScale.x}; ");
            AppendComponents(text, prefab);
            for (int i = 0; i < prefab.transform.childCount; i++)
            {
                Transform child = prefab.transform.GetChild(i);
                text.Append($"; child {child.name} (scale {child.localScale.x}: ");
                AppendComponents(text, child.gameObject);
                text.Append(')');
            }
            return text.ToString();
        }

        private static void AppendComponents(StringBuilder text, GameObject go)
        {
            Component[] components = go.GetComponents<Component>();
            bool first = true;
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] is Transform)
                {
                    continue;
                }
                text.Append(first ? "" : ", ").Append(components[i] != null ? components[i].GetType().Name : "MISSING SCRIPT");
                first = false;
            }
        }
    }
}
