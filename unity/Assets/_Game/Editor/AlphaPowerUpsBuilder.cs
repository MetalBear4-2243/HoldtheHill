using HoldTheHill.Features.GameFlow;
using HoldTheHill.Features.PowerUps;
using HoldTheHill.Features.Towers;
using HoldTheHill.Features.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// Alpha step 8, power-ups. Makes the settings asset and the drop prefab, puts a Power Ups
    /// object in each level's gameplay scene (wired to the run, the placer and the actions asset),
    /// and adds the one power-up slot to the HUD.
    /// </summary>
    /// <remarks>
    /// Safe to run again: the asset and prefab are kept (Inspector tuning survives), scene objects
    /// are found by component and only their references are refreshed. Runs after Build Towers
    /// (the placer and the run) and Build Run Frame (the HUD).
    /// </remarks>
    internal static class AlphaPowerUpsBuilder
    {
        private const string DataFolder = "Assets/_Game/Features/PowerUps/Data";
        private const string PrefabFolder = "Assets/_Game/Features/PowerUps/Prefabs";
        private const string SettingsPath = DataFolder + "/PowerUpSettings.asset";
        private const string PickupPath = PrefabFolder + "/PowerUp_Pickup.prefab";
        private const string ActionsPath = "Assets/_Game/Input/HoldTheHill.inputactions";
        private const string LevelsFolder = "Assets/_Game/Scenes/Levels";
        private const string UiScenePath = "Assets/_Game/Scenes/UI.unity";

        private const float Margin = 24f;
        private static readonly Vector2 SlotSize = new Vector2(300f, 64f);
        private const float SlotIconSize = 44f;

        private static readonly string[] LevelScenes =
        {
            $"{LevelsFolder}/L00_TutorialHill/L00_TutorialHill_Gameplay.unity",
            $"{LevelsFolder}/L01_Meadow/L01_Meadow_Gameplay.unity",
        };

        [MenuItem("Tools/Hold the Hill/Alpha/Build Power-ups (drops, slot, Swarm Frenzy, Royal Gift)")]
        private static void Build()
        {
            Sprite circle = AlphaCoreLoopBuilder.EnsureSprite("Circle", circle: true);
            Sprite square = AlphaCoreLoopBuilder.EnsureSprite("Square", circle: false);
            AlphaCoreLoopBuilder.EnsureFolder(DataFolder);
            AlphaCoreLoopBuilder.EnsureFolder(PrefabFolder);

            EnsureSettings(circle, square);
            EnsurePickupPrefab(circle);
            AssetDatabase.SaveAssets();

            for (int i = 0; i < LevelScenes.Length; i++)
            {
                WireLevel(LevelScenes[i]);
            }

            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
            EnsureSlot(ui);
            EditorSceneManager.MarkSceneDirty(ui);
            EditorSceneManager.SaveScene(ui);

            AssetDatabase.SaveAssets();
            ReportAfterReload();
            Debug.Log("Alpha power-ups built: kills sometimes drop one, a click collects it into the HUD slot, the slot uses it.");
        }

        /// <summary>Entry point for a batch run: <c>-executeMethod HoldTheHill.EditorTools.AlphaPowerUpsBuilder.BuildFromCommandLine</c>.</summary>
        public static void BuildFromCommandLine()
        {
            Build();
        }

        private static void EnsureSettings(Sprite circle, Sprite square)
        {
            if (AssetDatabase.LoadAssetAtPath<PowerUpSettings>(SettingsPath) != null)
            {
                return;
            }

            var settings = ScriptableObject.CreateInstance<PowerUpSettings>();
            // A disc for the frenzy, a square for the gift: two shapes, not just two colours.
            settings.FrenzyIcon = circle;
            settings.GiftIcon = square;
            AssetDatabase.CreateAsset(settings, SettingsPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(SettingsPath, ImportAssetOptions.ForceSynchronousImport);
            Debug.Log("Made PowerUpSettings.");
        }

        // A small bright disc above the enemies; PowerUps recolours and reshapes it per kind.
        private static void EnsurePickupPrefab(Sprite circle)
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(PickupPath) != null)
            {
                return;
            }

            var go = new GameObject("PowerUp_Pickup");
            go.transform.localScale = Vector3.one * 0.45f;
            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = circle;
            renderer.color = Color.white;
            renderer.sortingOrder = 30;
            go.AddComponent<PowerUpPickup>();

            PrefabUtility.SaveAsPrefabAsset(go, PickupPath);
            Object.DestroyImmediate(go);
            Debug.Log("Made PowerUp_Pickup.");
        }

        private static void WireLevel(string scenePath)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) == null)
            {
                Debug.LogWarning($"{scenePath} does not exist yet; run Build Home and Levels first.");
                return;
            }

            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

            // Loaded after the scene opened: opening a scene unloads loose assets.
            var settings = AssetDatabase.LoadAssetAtPath<PowerUpSettings>(SettingsPath);
            var pickup = AssetDatabase.LoadAssetAtPath<GameObject>(PickupPath);
            var actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(ActionsPath);
            TowerPlacer placer = AlphaRunFrameBuilder.FindInScene<TowerPlacer>(scene);
            LevelRun run = AlphaRunFrameBuilder.FindInScene<LevelRun>(scene);

            LevelPowerUps powerUps = AlphaRunFrameBuilder.FindInScene<LevelPowerUps>(scene);
            if (powerUps == null)
            {
                powerUps = new GameObject("Power Ups").AddComponent<LevelPowerUps>();
            }

            var so = new SerializedObject(powerUps);
            so.FindProperty("_settings").objectReferenceValue = settings;
            so.FindProperty("_pickupPrefab").objectReferenceValue = pickup != null ? pickup.GetComponent<PowerUpPickup>() : null;
            so.FindProperty("_placer").objectReferenceValue = placer;
            so.FindProperty("_actions").objectReferenceValue = actions;
            so.ApplyModifiedPropertiesWithoutUndo();

            if (run != null)
            {
                var runSo = new SerializedObject(run);
                runSo.FindProperty("_powerUps").objectReferenceValue = powerUps;
                runSo.ApplyModifiedPropertiesWithoutUndo();
            }
            else
            {
                Debug.LogWarning($"{scenePath} has no LevelRun; the HUD slot cannot reach its power-ups.");
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        // One button at the bottom centre of the HUD: an icon on the left, the name beside it.
        private static void EnsureSlot(Scene scene)
        {
            if (AlphaRunFrameBuilder.FindInScene<PowerUpSlotPanel>(scene) != null)
            {
                return;
            }

            HudPanel hudPanel = AlphaRunFrameBuilder.FindInScene<HudPanel>(scene);
            var hud = hudPanel != null ? new SerializedObject(hudPanel).FindProperty("_hud").objectReferenceValue as GameObject : null;
            if (hud == null)
            {
                Debug.LogError("The UI scene has no HUD yet; run Build Run Frame first.");
                return;
            }

            int layer = hud.layer;
            Button button = AlphaRunFrameBuilder.MakeButton(hud.transform, "Power-up Slot", UiStrings.PowerUpEmpty, layer);
            AlphaRunFrameBuilder.Anchor((RectTransform)button.transform, new Vector2(0.5f, 0f), new Vector2(0f, Margin), SlotSize);

            TMP_Text label = button.GetComponentInChildren<TMP_Text>();
            // Leave room for the icon on the left.
            label.rectTransform.offsetMin = new Vector2(SlotIconSize + 20f, 0f);
            label.alignment = TextAlignmentOptions.MidlineLeft;

            Image icon = AlphaRunFrameBuilder.MakeImage(button.transform, "Icon", Color.white, layer);
            AlphaRunFrameBuilder.Anchor(icon.rectTransform, new Vector2(0f, 0.5f), new Vector2(10f, 0f), new Vector2(SlotIconSize, SlotIconSize));
            icon.raycastTarget = false;
            icon.preserveAspect = true;
            icon.enabled = false;

            PowerUpSlotPanel panel = button.gameObject.AddComponent<PowerUpSlotPanel>();
            var so = new SerializedObject(panel);
            so.FindProperty("_button").objectReferenceValue = button;
            so.FindProperty("_icon").objectReferenceValue = icon;
            so.FindProperty("_label").objectReferenceValue = label;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // Opens the saved assets and scenes again and says what they hold, so a batch log shows the truth.
        private static void ReportAfterReload()
        {
            var settings = AssetDatabase.LoadAssetAtPath<PowerUpSettings>(SettingsPath);
            Debug.Log(settings != null
                ? $"PowerUpSettings: drop {settings.DropChance:P0}, lifetime {settings.DropLifetime}s, frenzy x{settings.FrenzyFireRateFactor} for {settings.FrenzyDuration}s, icons {(settings.FrenzyIcon != null ? settings.FrenzyIcon.name : "MISSING")} / {(settings.GiftIcon != null ? settings.GiftIcon.name : "MISSING")}"
                : "PowerUpSettings: MISSING");

            var pickup = AssetDatabase.LoadAssetAtPath<GameObject>(PickupPath);
            Debug.Log($"PowerUp_Pickup: {(pickup != null && pickup.GetComponent<PowerUpPickup>() != null ? "ok" : "MISSING")}");

            for (int i = 0; i < LevelScenes.Length; i++)
            {
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(LevelScenes[i]) == null)
                {
                    continue;
                }

                Scene scene = EditorSceneManager.OpenScene(LevelScenes[i], OpenSceneMode.Single);
                string level = System.IO.Path.GetFileNameWithoutExtension(LevelScenes[i]);
                LevelPowerUps powerUps = AlphaRunFrameBuilder.FindInScene<LevelPowerUps>(scene);
                LevelRun run = AlphaRunFrameBuilder.FindInScene<LevelRun>(scene);
                if (powerUps == null)
                {
                    Debug.LogError($"{level}: no Power Ups object after save and reload.");
                    continue;
                }

                var so = new SerializedObject(powerUps);
                string runRef = run != null ? AlphaRunFrameBuilder.Name(new SerializedObject(run), "_powerUps") : "no run";
                Debug.Log($"{level}: settings = {AlphaRunFrameBuilder.Name(so, "_settings")}, pickup = {AlphaRunFrameBuilder.Name(so, "_pickupPrefab")}, "
                          + $"placer = {AlphaRunFrameBuilder.Name(so, "_placer")}, actions = {AlphaRunFrameBuilder.Name(so, "_actions")}, run power-ups = {runRef}");
            }

            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
            PowerUpSlotPanel slot = AlphaRunFrameBuilder.FindInScene<PowerUpSlotPanel>(ui);
            if (slot == null)
            {
                Debug.LogError("UI scene: no power-up slot after save and reload.");
                return;
            }

            var slotSo = new SerializedObject(slot);
            Debug.Log($"UI scene power-up slot: button = {AlphaRunFrameBuilder.Name(slotSo, "_button")}, icon = {AlphaRunFrameBuilder.Name(slotSo, "_icon")}, label = {AlphaRunFrameBuilder.Name(slotSo, "_label")}");
        }
    }
}
