using HoldTheHill.Features.GameFlow;
using HoldTheHill.Features.Towers;
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
    /// Alpha step 4, the rest of the towers. Makes the four remaining tower prefabs and their shop
    /// data, gives every tower its upgrade levels, puts all six on offer in each level's placer,
    /// and adds the tower card (name, level, upgrade button, standing orders) to the UI scene.
    /// </summary>
    /// <remarks>
    /// Safe to run again: assets that exist are kept, scene objects are found by component and only
    /// their references are refreshed. Runs after the core loop, run frame and levels builders.
    /// </remarks>
    internal static class AlphaTowersBuilder
    {
        private const string UiScenePath = "Assets/_Game/Scenes/UI.unity";
        private const string LevelsFolder = "Assets/_Game/Scenes/Levels";
        private const string WaveDataFolder = "Assets/_Game/Features/Enemies/Data";

        private static readonly (string scene, string waves)[] Levels =
        {
            ($"{LevelsFolder}/Hill01/Hill01_Gameplay.unity", $"{WaveDataFolder}/Hill01_Waves.asset"),
            ($"{LevelsFolder}/L00_TutorialHill/L00_TutorialHill_Gameplay.unity", $"{WaveDataFolder}/L00_TutorialHill_Waves.asset"),
            ($"{LevelsFolder}/L01_Meadow/L01_Meadow_Gameplay.unity", $"{WaveDataFolder}/L01_Meadow_Waves.asset"),
        };

        private static readonly Vector2 CardSize = new Vector2(360f, 230f);
        private static readonly Vector2 OrderButtonSize = new Vector2(104f, 48f);
        private static readonly Color CardColor = new Color(0.08f, 0.08f, 0.1f, 0.85f);
        private const float Margin = 24f;
        private const int CardFontSize = 30;
        private const int SmallFontSize = 24;

        [MenuItem("Tools/Hold the Hill/Alpha/Build Towers (all six, upgrades, card)")]
        private static void Build()
        {
            AlphaCoreLoopBuilder.EnsureTowerAssets();

            for (int i = 0; i < Levels.Length; i++)
            {
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Levels[i].scene) == null)
                {
                    Debug.LogWarning($"{Levels[i].scene} does not exist yet; run Build Home and Levels first.");
                    continue;
                }

                AlphaCoreLoopBuilder.WireGameplayScene(Levels[i].scene, Levels[i].waves);
            }

            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
            EnsureTowerCard(ui);
            EditorSceneManager.MarkSceneDirty(ui);
            EditorSceneManager.SaveScene(ui);

            AssetDatabase.SaveAssets();
            ReportAfterReload();
            Debug.Log("Alpha towers built: keys 1 to 6 sell all six towers; click a placed tower for its card, U or the card upgrades it.");
        }

        /// <summary>Entry point for a batch run: <c>-executeMethod HoldTheHill.EditorTools.AlphaTowersBuilder.BuildFromCommandLine</c>.</summary>
        public static void BuildFromCommandLine()
        {
            Build();
        }

        // Bottom-left of the HUD: name, level, the upgrade button, and the three standing orders.
        private static void EnsureTowerCard(Scene scene)
        {
            if (AlphaRunFrameBuilder.FindInScene<TowerCardPanel>(scene) != null)
            {
                return;
            }

            Canvas canvas = AlphaRunFrameBuilder.FindInScene<Canvas>(scene);
            if (canvas == null)
            {
                Debug.LogError("The UI scene has no canvas yet; run Build Run Frame first.");
                return;
            }

            int layer = canvas.gameObject.layer;
            Transform hud = canvas.transform.Find("HUD");
            Transform parent = hud != null ? hud : canvas.transform;

            Image card = AlphaRunFrameBuilder.MakeImage(parent, "Tower Card", CardColor, layer);
            AlphaRunFrameBuilder.Anchor(card.rectTransform, new Vector2(0f, 0f), new Vector2(Margin, Margin), CardSize);

            TextMeshProUGUI name = AlphaRunFrameBuilder.MakeText(card.transform, "Name", UiStrings.UnknownTower, CardFontSize,
                TextAlignmentOptions.MidlineLeft, layer);
            AlphaRunFrameBuilder.Anchor(name.rectTransform, new Vector2(0f, 1f), new Vector2(16f, -12f), new Vector2(220f, 40f));

            TextMeshProUGUI level = AlphaRunFrameBuilder.MakeText(card.transform, "Level", string.Format(UiStrings.TowerLevelFormat, 1, 3),
                SmallFontSize, TextAlignmentOptions.MidlineRight, layer);
            AlphaRunFrameBuilder.Anchor(level.rectTransform, new Vector2(1f, 1f), new Vector2(-16f, -12f), new Vector2(140f, 40f));

            Button upgrade = AlphaRunFrameBuilder.MakeButton(card.transform, "Upgrade Button", string.Format(UiStrings.UpgradeFormat, 0), layer);
            AlphaRunFrameBuilder.Anchor((RectTransform)upgrade.transform, new Vector2(0.5f, 1f), new Vector2(0f, -64f), new Vector2(CardSize.x - 32f, 56f));
            TMP_Text upgradeLabel = upgrade.GetComponentInChildren<TMP_Text>();

            TextMeshProUGUI orderTitle = AlphaRunFrameBuilder.MakeText(card.transform, "Order Title", UiStrings.StandingOrder, SmallFontSize,
                TextAlignmentOptions.MidlineLeft, layer);
            AlphaRunFrameBuilder.Anchor(orderTitle.rectTransform, new Vector2(0f, 0f), new Vector2(16f, 72f), new Vector2(200f, 36f));

            string[] orderNames = { UiStrings.OrderFirst, UiStrings.OrderLast, UiStrings.OrderClosest };
            var orderButtons = new Button[orderNames.Length];
            float step = (CardSize.x - 32f - OrderButtonSize.x * orderNames.Length) / (orderNames.Length - 1) + OrderButtonSize.x;
            for (int i = 0; i < orderNames.Length; i++)
            {
                orderButtons[i] = AlphaRunFrameBuilder.MakeButton(card.transform, $"Order {orderNames[i]}", orderNames[i], layer);
                TMP_Text text = orderButtons[i].GetComponentInChildren<TMP_Text>();
                text.fontSize = SmallFontSize;
                AlphaRunFrameBuilder.Anchor((RectTransform)orderButtons[i].transform, new Vector2(0f, 0f),
                    new Vector2(16f + i * step, 16f), OrderButtonSize);
            }

            // The panel component sits on the HUD object, above the card it toggles, so hiding the card
            // never silences the listener (same reason as HudPanel).
            TowerCardPanel panel = parent.gameObject.AddComponent<TowerCardPanel>();
            var so = new SerializedObject(panel);
            so.FindProperty("_panel").objectReferenceValue = card.gameObject;
            so.FindProperty("_name").objectReferenceValue = name;
            so.FindProperty("_level").objectReferenceValue = level;
            so.FindProperty("_upgrade").objectReferenceValue = upgrade;
            so.FindProperty("_upgradeLabel").objectReferenceValue = upgradeLabel;
            SerializedProperty buttons = so.FindProperty("_orderButtons");
            buttons.arraySize = orderButtons.Length;
            for (int i = 0; i < orderButtons.Length; i++)
            {
                buttons.GetArrayElementAtIndex(i).objectReferenceValue = orderButtons[i];
            }
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // Opens the saved scenes again and says what they hold, so a batch log shows the truth.
        private static void ReportAfterReload()
        {
            for (int i = 0; i < Levels.Length; i++)
            {
                if (AssetDatabase.LoadAssetAtPath<SceneAsset>(Levels[i].scene) == null)
                {
                    continue;
                }

                Scene scene = EditorSceneManager.OpenScene(Levels[i].scene, OpenSceneMode.Single);
                LevelRun run = AlphaRunFrameBuilder.FindInScene<LevelRun>(scene);
                TowerPlacer placer = AlphaRunFrameBuilder.FindInScene<TowerPlacer>(scene);
                int offers = placer != null ? new SerializedObject(placer).FindProperty("_offers").arraySize : 0;
                string runPlacer = run != null ? AlphaRunFrameBuilder.Name(new SerializedObject(run), "_placer") : "no run";
                Debug.Log($"{System.IO.Path.GetFileNameWithoutExtension(Levels[i].scene)}: {offers} offers, run placer = {runPlacer}");
            }

            for (int i = 0; i < AlphaCoreLoopBuilder.Towers.Length; i++)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AlphaCoreLoopBuilder.Towers[i].PrefabPath);
                Tower tower = prefab != null ? prefab.GetComponent<Tower>() : null;
                int levels = tower != null ? new SerializedObject(tower).FindProperty("_upgrades").arraySize : -1;
                string extras = tower == null ? "missing"
                    : tower.GetComponent<SlowAura>() != null ? "slow aura"
                    : tower.GetComponent<MineLayer>() != null ? "mine layer"
                    : new SerializedObject(tower).FindProperty("_projectilePrefab").objectReferenceValue != null ? "projectile" : "melee";
                Debug.Log($"{AlphaCoreLoopBuilder.Towers[i].PrefabName}: {levels} upgrade levels, {extras}");
            }

            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
            TowerCardPanel card = AlphaRunFrameBuilder.FindInScene<TowerCardPanel>(ui);
            if (card == null)
            {
                Debug.LogError("UI scene: no tower card after save and reload.");
                return;
            }

            var cardSo = new SerializedObject(card);
            Debug.Log($"UI scene tower card: panel = {AlphaRunFrameBuilder.Name(cardSo, "_panel")}, upgrade = {AlphaRunFrameBuilder.Name(cardSo, "_upgrade")}, "
                      + $"order buttons = {cardSo.FindProperty("_orderButtons").arraySize}");
        }
    }
}
