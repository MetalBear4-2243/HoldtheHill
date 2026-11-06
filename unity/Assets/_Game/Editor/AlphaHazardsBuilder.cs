using System.Collections.Generic;
using HoldTheHill.Features.Hazards;
using HoldTheHill.Features.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// Alpha step 7, burrow and the hazards. Makes the four hazard prefabs and the wasp's stinger,
    /// gives the Wasp prefab its stinger dropper, writes one hazard schedule per level, puts a
    /// scheduler in each level's gameplay scene, and adds the burrow button to the tower card.
    /// </summary>
    /// <remarks>
    /// Safe to run again: prefabs and schedules that exist are kept (Inspector tuning survives),
    /// scene objects are found by component and only their references are refreshed. Runs after
    /// Build Towers (the card) and Build Enemies (the Wasp prefab).
    /// </remarks>
    internal static class AlphaHazardsBuilder
    {
        private const string HazardPrefabFolder = "Assets/_Game/Features/Hazards/Prefabs";
        private const string HazardDataFolder = "Assets/_Game/Features/Hazards/Data";
        private const string EnemyPrefabFolder = "Assets/_Game/Features/Enemies/Prefabs";
        private const string LevelsFolder = "Assets/_Game/Scenes/Levels";
        private const string UiScenePath = "Assets/_Game/Scenes/UI.unity";

        private const string DiatomaceousEarthPath = HazardPrefabFolder + "/Hazard_DiatomaceousEarth.prefab";
        private const string WaspStingerPath = HazardPrefabFolder + "/Hazard_WaspStinger.prefab";
        private const string BirdShadowPath = HazardPrefabFolder + "/Hazard_BirdShadow.prefab";
        private const string PesticideHumanPath = HazardPrefabFolder + "/Hazard_PesticideHuman.prefab";
        private const string WaspPath = EnemyPrefabFolder + "/Enemy_Wasp.prefab";

        private static readonly Vector2 BurrowButtonSize = new Vector2(360f - 32f, 56f);
        private const float CardGrowth = 64f;

        /// <summary>One scheduled hazard, in plain numbers. Positions are world units on the level's route.</summary>
        private readonly struct EntrySpec
        {
            public readonly int Wave;
            public readonly float Delay;
            public readonly string Prefab;
            public readonly Vector2 Position;

            public EntrySpec(int wave, float delay, string prefab, Vector2 position)
            {
                Wave = wave;
                Delay = delay;
                Prefab = prefab;
                Position = position;
            }
        }

        // Tutorial Hill's route runs along y = 0.5 to x = 0.5, down to y = -2.5, then right.
        // Meadow's winds between y = 3.5 and y = -2.5 at x = -5.5, -0.5 and 4.5.
        private static readonly (string id, string scene, EntrySpec[] entries)[] Levels =
        {
            ("L00_TutorialHill", $"{LevelsFolder}/L00_TutorialHill/L00_TutorialHill_Gameplay.unity", new[]
            {
                new EntrySpec(3, 2f, DiatomaceousEarthPath, new Vector2(3.5f, -2.5f)),
                new EntrySpec(4, 6f, PesticideHumanPath, Vector2.zero),
                new EntrySpec(5, 4f, BirdShadowPath, new Vector2(-4.5f, 0.5f)),
            }),
            ("L01_Meadow", $"{LevelsFolder}/L01_Meadow/L01_Meadow_Gameplay.unity", new[]
            {
                new EntrySpec(3, 3f, DiatomaceousEarthPath, new Vector2(-3.5f, -2.5f)),
                new EntrySpec(5, 5f, BirdShadowPath, new Vector2(1.5f, 3.5f)),
                new EntrySpec(6, 8f, PesticideHumanPath, Vector2.zero),
                new EntrySpec(8, 2f, DiatomaceousEarthPath, new Vector2(6.5f, -2.5f)),
                new EntrySpec(9, 6f, BirdShadowPath, new Vector2(-6.5f, 0.5f)),
            }),
        };

        [MenuItem("Tools/Hold the Hill/Alpha/Build Hazards (burrow button, four hazards, schedules)")]
        private static void Build()
        {
            Sprite circle = AlphaCoreLoopBuilder.EnsureSprite("Circle", circle: true);
            Sprite square = AlphaCoreLoopBuilder.EnsureSprite("Square", circle: false);
            AlphaCoreLoopBuilder.EnsureFolder(HazardPrefabFolder);
            AlphaCoreLoopBuilder.EnsureFolder(HazardDataFolder);

            EnsureDiatomaceousEarth(circle);
            EnsureWaspStinger(circle);
            EnsureBirdShadow(circle);
            EnsurePesticideHuman(square);
            AssetDatabase.SaveAssets();

            GiveWaspItsStinger();

            for (int i = 0; i < Levels.Length; i++)
            {
                LevelHazardSchedule schedule = EnsureSchedule(Levels[i].id, Levels[i].entries);
                WireScheduler(Levels[i].scene, AssetDatabase.GetAssetPath(schedule));
            }

            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
            EnsureBurrowButton(ui);
            EditorSceneManager.MarkSceneDirty(ui);
            EditorSceneManager.SaveScene(ui);

            AssetDatabase.SaveAssets();
            ReportAfterReload();
            Debug.Log("Alpha hazards built: B or the card burrows towers; dust, stingers, the bird and the pesticide run from each level's schedule.");
        }

        /// <summary>Entry point for a batch run: <c>-executeMethod HoldTheHill.EditorTools.AlphaHazardsBuilder.BuildFromCommandLine</c>.</summary>
        public static void BuildFromCommandLine()
        {
            Build();
        }

        // A pale disc on the ground. DiatomaceousEarth scales it to its radius at Start.
        private static void EnsureDiatomaceousEarth(Sprite circle)
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(DiatomaceousEarthPath) != null)
            {
                return;
            }

            GameObject go = NewSpriteObject("Hazard_DiatomaceousEarth", circle, new Color(0.92f, 0.9f, 0.78f, 0.55f), 1f, sortingOrder: -5);
            go.AddComponent<DiatomaceousEarth>();
            SavePrefab(go, DiatomaceousEarthPath);
        }

        // A small dark drop; WaspStinger swells it over its fuse.
        private static void EnsureWaspStinger(Sprite circle)
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(WaspStingerPath) != null)
            {
                return;
            }

            GameObject go = NewSpriteObject("Hazard_WaspStinger", circle, new Color(0.25f, 0.18f, 0.05f), 0.25f, sortingOrder: 8);
            go.AddComponent<WaspStinger>();
            SavePrefab(go, WaspStingerPath);
        }

        // A translucent dark disc drawn over everything on the map.
        private static void EnsureBirdShadow(Sprite circle)
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(BirdShadowPath) != null)
            {
                return;
            }

            GameObject go = NewSpriteObject("Hazard_BirdShadow", circle, new Color(0f, 0f, 0f, 0.45f), 1.8f, sortingOrder: 50);
            go.AddComponent<BirdShadow>();
            SavePrefab(go, BirdShadowPath);
        }

        // A cloud over the whole visible map; PesticideHuman fades it in and out.
        private static void EnsurePesticideHuman(Sprite square)
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(PesticideHumanPath) != null)
            {
                return;
            }

            var go = new GameObject("Hazard_PesticideHuman");
            var human = go.AddComponent<PesticideHuman>();

            GameObject cloud = NewSpriteObject("Cloud", square, new Color(0.75f, 1f, 0.6f, 0f), 1f, sortingOrder: 60);
            cloud.transform.SetParent(go.transform, false);
            cloud.transform.localScale = new Vector3(24f, 14f, 1f);

            var so = new SerializedObject(human);
            so.FindProperty("_cloud").objectReferenceValue = cloud.GetComponent<SpriteRenderer>();
            so.ApplyModifiedPropertiesWithoutUndo();

            SavePrefab(go, PesticideHumanPath);
        }

        // Adds the dropper to the Wasp prefab (an Alpha prefab, not Jason's) and points it at the stinger.
        private static void GiveWaspItsStinger()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(WaspPath) == null)
            {
                Debug.LogWarning($"{WaspPath} does not exist yet; run Build Enemies first. Wasps will drop nothing until it is set.");
                return;
            }

            GameObject root = PrefabUtility.LoadPrefabContents(WaspPath);
            try
            {
                WaspStingerDropper dropper = root.GetComponent<WaspStingerDropper>();
                if (dropper == null)
                {
                    dropper = root.AddComponent<WaspStingerDropper>();
                }

                var stinger = AssetDatabase.LoadAssetAtPath<GameObject>(WaspStingerPath);
                var so = new SerializedObject(dropper);
                so.FindProperty("_stingerPrefab").objectReferenceValue = stinger != null ? stinger.GetComponent<WaspStinger>() : null;
                so.ApplyModifiedPropertiesWithoutUndo();

                PrefabUtility.SaveAsPrefabAsset(root, WaspPath);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        private static LevelHazardSchedule EnsureSchedule(string levelId, EntrySpec[] specs)
        {
            string path = $"{HazardDataFolder}/{levelId}_Hazards.asset";
            var schedule = AssetDatabase.LoadAssetAtPath<LevelHazardSchedule>(path);
            if (schedule != null)
            {
                return schedule;
            }

            schedule = ScriptableObject.CreateInstance<LevelHazardSchedule>();
            for (int i = 0; i < specs.Length; i++)
            {
                schedule.Entries.Add(new LevelHazardSchedule.Entry
                {
                    Wave = specs[i].Wave,
                    Delay = specs[i].Delay,
                    Hazard = AssetDatabase.LoadAssetAtPath<GameObject>(specs[i].Prefab),
                    Position = specs[i].Position,
                });
            }

            AssetDatabase.CreateAsset(schedule, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            return AssetDatabase.LoadAssetAtPath<LevelHazardSchedule>(path);
        }

        private static void WireScheduler(string scenePath, string schedulePath)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) == null)
            {
                Debug.LogWarning($"{scenePath} does not exist yet; run Build Home and Levels first.");
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // Loaded after the scene opened: opening a scene unloads loose assets.
            var schedule = AssetDatabase.LoadAssetAtPath<LevelHazardSchedule>(schedulePath);
            EnemySpawner spawner = AlphaRunFrameBuilder.FindInScene<EnemySpawner>(scene);

            HazardScheduler scheduler = AlphaRunFrameBuilder.FindInScene<HazardScheduler>(scene);
            if (scheduler == null)
            {
                scheduler = new GameObject("Hazard Scheduler").AddComponent<HazardScheduler>();
            }

            var so = new SerializedObject(scheduler);
            so.FindProperty("_spawner").objectReferenceValue = spawner;
            so.FindProperty("_schedule").objectReferenceValue = schedule;
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        // Grows the card and puts the burrow button between the upgrade button and the standing orders.
        private static void EnsureBurrowButton(Scene scene)
        {
            TowerCardPanel panel = AlphaRunFrameBuilder.FindInScene<TowerCardPanel>(scene);
            if (panel == null)
            {
                Debug.LogError("The UI scene has no tower card yet; run Build Towers first.");
                return;
            }

            var so = new SerializedObject(panel);
            if (so.FindProperty("_burrow").objectReferenceValue != null)
            {
                return;
            }

            var card = so.FindProperty("_panel").objectReferenceValue as GameObject;
            if (card == null)
            {
                Debug.LogError("The tower card panel has no card object to add a button to.");
                return;
            }

            var cardRect = (RectTransform)card.transform;
            cardRect.sizeDelta = new Vector2(cardRect.sizeDelta.x, cardRect.sizeDelta.y + CardGrowth);

            Button burrow = AlphaRunFrameBuilder.MakeButton(card.transform, "Burrow Button", UiStrings.Burrow, card.layer);
            AlphaRunFrameBuilder.Anchor((RectTransform)burrow.transform, new Vector2(0.5f, 1f), new Vector2(0f, -128f), BurrowButtonSize);

            so.FindProperty("_burrow").objectReferenceValue = burrow;
            so.FindProperty("_burrowLabel").objectReferenceValue = burrow.GetComponentInChildren<TMP_Text>();
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // Opens the saved assets and scenes again and says what they hold, so a batch log shows the truth.
        private static void ReportAfterReload()
        {
            string[] prefabs = { DiatomaceousEarthPath, WaspStingerPath, BirdShadowPath, PesticideHumanPath };
            for (int i = 0; i < prefabs.Length; i++)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabs[i]);
                Debug.Log($"{System.IO.Path.GetFileNameWithoutExtension(prefabs[i])}: {(prefab != null ? "ok" : "MISSING")}");
            }

            var wasp = AssetDatabase.LoadAssetAtPath<GameObject>(WaspPath);
            WaspStingerDropper dropper = wasp != null ? wasp.GetComponent<WaspStingerDropper>() : null;
            string stinger = dropper != null ? AlphaRunFrameBuilder.Name(new SerializedObject(dropper), "_stingerPrefab") : "no dropper";
            Debug.Log($"Enemy_Wasp: stinger = {stinger}");

            for (int i = 0; i < Levels.Length; i++)
            {
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Levels[i].scene) == null)
                {
                    continue;
                }

                Scene scene = EditorSceneManager.OpenScene(Levels[i].scene, OpenSceneMode.Single);
                HazardScheduler scheduler = AlphaRunFrameBuilder.FindInScene<HazardScheduler>(scene);
                if (scheduler == null)
                {
                    Debug.LogError($"{Levels[i].id}: no hazard scheduler after save and reload.");
                    continue;
                }

                var so = new SerializedObject(scheduler);
                var schedule = so.FindProperty("_schedule").objectReferenceValue as LevelHazardSchedule;
                var text = new System.Text.StringBuilder();
                text.Append($"{Levels[i].id}: spawner = {AlphaRunFrameBuilder.Name(so, "_spawner")}, schedule = {(schedule != null ? schedule.name : "none")}");
                if (schedule != null)
                {
                    for (int e = 0; e < schedule.Entries.Count; e++)
                    {
                        LevelHazardSchedule.Entry entry = schedule.Entries[e];
                        text.Append($"\n  wave {entry.Wave} +{entry.Delay}s: {(entry.Hazard != null ? entry.Hazard.name : "MISSING")} at {entry.Position}");
                    }
                }
                Debug.Log(text.ToString());
            }

            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
            TowerCardPanel card = AlphaRunFrameBuilder.FindInScene<TowerCardPanel>(ui);
            string burrow = card != null ? AlphaRunFrameBuilder.Name(new SerializedObject(card), "_burrow") : "no card";
            Debug.Log($"UI scene tower card: burrow button = {burrow}");
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

        private static void SavePrefab(GameObject go, string path)
        {
            PrefabUtility.SaveAsPrefabAsset(go, path);
            Object.DestroyImmediate(go);
            Debug.Log($"Made {System.IO.Path.GetFileNameWithoutExtension(path)}.");
        }
    }
}
