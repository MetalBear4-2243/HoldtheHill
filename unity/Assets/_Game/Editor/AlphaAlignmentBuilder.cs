using HoldTheHill.Features.Levels;
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
    /// What items 7-20 of the 10-29 alignment report need in the scenes and settings. In the <c>UI</c>
    /// scene: the tower card's order heading wired, so it hides with the order buttons (UI-6); raycast
    /// off on the Queen bar's images, so it no longer eats build clicks (G1-4); Adam's list scrolling the
    /// right way at a usable speed, as a scene override on his instance (his prefab is untouched); and
    /// the controls help box tall enough for its lines (UI-4). In both level scenes: the camera fit's
    /// top band raised for the Queen bar and a right band kept for Adam's list (CT-1, G1-4). In Build
    /// Settings: the legacy Hill01 pair unticked (CT-4); its files stay, the level builder copies them.
    /// Leaves the Bootstrap scene open, ready for Play.
    /// </summary>
    /// <remarks>Safe to run again: every value is set, nothing is added twice.</remarks>
    internal static class AlphaAlignmentBuilder
    {
        private const string UiScenePath = "Assets/_Game/Scenes/UI.unity";
        private const string BootstrapScenePath = "Assets/_Game/Scenes/Bootstrap.unity";
        private const string LegacyLevelFolder = "Assets/_Game/Scenes/Levels/Hill01/";
        private const string OrderTitleName = "Order Title";

        // The input module scrolls 6 units per wheel notch times this, about one card. Adam's prefab has -1: backwards and slow.
        private const float ListScrollSensitivity = 20f;

        // LevelCameraFit's defaults; the level scenes were saved with the 10-31 values (top 0.07, no right band).
        private const float CameraTopBand = 0.15f;
        private const float CameraRightBand = 0.3f;

        // The help box (AlphaSellAndHelpBuilder): its first height, and the text's padding inside it.
        private const float HelpBoxMinHeight = 330f;
        private static readonly Vector2 HelpTextPadding = new Vector2(16f, 12f);
        private const float HelpBoxSpare = 8f;

        private static readonly string[] LevelGameplayScenes =
        {
            "Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity",
            "Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Gameplay.unity",
        };

        [MenuItem("Tools/Hold the Hill/Alpha/Apply Alignment Fixes (10-29 items 7-20)")]
        private static void Build()
        {
            // From the menu, the open scenes are about to be replaced: offer to save them first.
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
            WireOrderTitle(ui);
            LetClicksThroughQueenBar(ui);
            SetListScroll(ui);
            FitHelpBox(ui);
            EditorSceneManager.MarkSceneDirty(ui);
            EditorSceneManager.SaveScene(ui);

            foreach (string path in LevelGameplayScenes)
            {
                Scene level = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                SetCameraBands(level);
                EditorSceneManager.MarkSceneDirty(level);
                EditorSceneManager.SaveScene(level);
            }

            UntickLegacyLevel();

            ReportAfterReload();
            EditorSceneManager.OpenScene(BootstrapScenePath, OpenSceneMode.Single);
        }

        /// <summary>Entry point for a batch run: <c>-executeMethod HoldTheHill.EditorTools.AlphaAlignmentBuilder.BuildFromCommandLine</c>.</summary>
        public static void BuildFromCommandLine()
        {
            Build();
        }

        private static void WireOrderTitle(Scene ui)
        {
            TowerCardPanel card = AlphaRunFrameBuilder.FindInScene<TowerCardPanel>(ui);
            if (card == null)
            {
                Debug.LogError("The UI scene has no tower card.");
                return;
            }

            var so = new SerializedObject(card);
            Transform title = FindOrderTitle(so);
            if (title == null)
            {
                Debug.LogError($"The tower card has no '{OrderTitleName}' to hide with the order buttons.");
                return;
            }

            so.FindProperty("_orderTitle").objectReferenceValue = title.gameObject;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Transform FindOrderTitle(SerializedObject card)
        {
            var panel = card.FindProperty("_panel").objectReferenceValue as GameObject;
            if (panel == null)
            {
                return null;
            }

            foreach (Transform child in panel.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == OrderTitleName)
                {
                    return child;
                }
            }
            return null;
        }

        // The bar only shows the Queen's health; clicks on it belong to the map underneath.
        private static void LetClicksThroughQueenBar(Scene ui)
        {
            QueenHealthBar bar = AlphaRunFrameBuilder.FindInScene<QueenHealthBar>(ui);
            if (bar == null)
            {
                Debug.LogError("The UI scene has no Queen bar.");
                return;
            }

            foreach (Graphic graphic in bar.GetComponentsInChildren<Graphic>(true))
            {
                var so = new SerializedObject(graphic);
                so.FindProperty("m_RaycastTarget").boolValue = false;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        // A scene override on Adam's instance: his TowerList prefab keeps its own value.
        private static void SetListScroll(Scene ui)
        {
            ScrollRect scroll = FindListScroll(ui);
            if (scroll == null)
            {
                Debug.LogError("Adam's TowerList (with its scroll view) is missing from the UI scene.");
                return;
            }

            var so = new SerializedObject(scroll);
            so.FindProperty("m_ScrollSensitivity").floatValue = ListScrollSensitivity;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static ScrollRect FindListScroll(Scene ui)
        {
            TowerShopManager shop = AlphaRunFrameBuilder.FindInScene<TowerShopManager>(ui);
            return shop != null ? shop.GetComponentInChildren<ScrollRect>(true) : null;
        }

        // The box grows to fit its lines (two more since UI-4); measured with the text's own font and size.
        private static void FitHelpBox(Scene ui)
        {
            ControlsOverlay overlay = AlphaRunFrameBuilder.FindInScene<ControlsOverlay>(ui);
            if (overlay == null)
            {
                Debug.LogError("The UI scene has no controls help box.");
                return;
            }

            var so = new SerializedObject(overlay);
            var box = so.FindProperty("_panel").objectReferenceValue as GameObject;
            var text = so.FindProperty("_text").objectReferenceValue as TMP_Text;
            if (box == null || text == null)
            {
                Debug.LogError("The controls help box is not wired to its panel and text.");
                return;
            }

            // The scene's copy of the text matches what the game shows, so the box looks right in the Editor too.
            text.text = UiStrings.ControlsText;
            EditorUtility.SetDirty(text);

            var boxRect = (RectTransform)box.transform;
            float textWidth = boxRect.sizeDelta.x - 2f * HelpTextPadding.x;
            float textHeight = text.GetPreferredValues(UiStrings.ControlsText, textWidth, 0f).y;
            if (textHeight <= 0f)
            {
                Debug.LogWarning("Could not measure the controls help text; the box keeps its height.");
                return;
            }

            float height = Mathf.Max(HelpBoxMinHeight, Mathf.Ceil(textHeight + 2f * HelpTextPadding.y + HelpBoxSpare));
            boxRect.sizeDelta = new Vector2(boxRect.sizeDelta.x, height);
            EditorUtility.SetDirty(boxRect);
            Debug.Log($"Controls help: the text needs {textHeight:0} units at width {textWidth:0}; box height {height:0}.");
        }

        private static void SetCameraBands(Scene level)
        {
            Camera camera = AlphaRunFrameBuilder.FindInScene<Camera>(level);
            if (camera == null)
            {
                Debug.LogError($"{level.name} has no camera to fit.");
                return;
            }

            AlphaPlaytestToolsBuilder.EnsureCameraFit(camera);
            var so = new SerializedObject(camera.GetComponent<LevelCameraFit>());
            so.FindProperty("_topBand").floatValue = CameraTopBand;
            so.FindProperty("_rightBand").floatValue = CameraRightBand;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // Nothing opens Hill01 any more, so a player build should not carry it. Its files stay: the
        // level builder copies them, and its EnsureInBuildSettings skips listed scenes, so a rerun
        // does not tick them back on.
        private static void UntickLegacyLevel()
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            bool changed = false;
            foreach (EditorBuildSettingsScene scene in scenes)
            {
                if (scene.enabled && scene.path.StartsWith(LegacyLevelFolder))
                {
                    scene.enabled = false;
                    changed = true;
                }
            }

            if (changed)
            {
                EditorBuildSettings.scenes = scenes;
            }
        }

        // Opens the saved scenes again and says what they hold, so a batch log shows the truth.
        private static void ReportAfterReload()
        {
            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);

            TowerCardPanel card = AlphaRunFrameBuilder.FindInScene<TowerCardPanel>(ui);
            string orderTitle = card != null ? AlphaRunFrameBuilder.Name(new SerializedObject(card), "_orderTitle") : "no card";

            QueenHealthBar bar = AlphaRunFrameBuilder.FindInScene<QueenHealthBar>(ui);
            int graphics = 0;
            int catching = 0;
            if (bar != null)
            {
                foreach (Graphic graphic in bar.GetComponentsInChildren<Graphic>(true))
                {
                    graphics++;
                    catching += graphic.raycastTarget ? 1 : 0;
                }
            }

            ScrollRect scroll = FindListScroll(ui);
            string sensitivity = scroll != null ? scroll.scrollSensitivity.ToString("0.##") : "MISSING";

            ControlsOverlay overlay = AlphaRunFrameBuilder.FindInScene<ControlsOverlay>(ui);
            var box = overlay != null ? new SerializedObject(overlay).FindProperty("_panel").objectReferenceValue as GameObject : null;
            string helpBox = box != null ? ((RectTransform)box.transform).sizeDelta.ToString("0") : "MISSING";

            string report = $"Alignment fixes: order title = {orderTitle}, Queen bar graphics catching clicks = {catching} of {graphics}, "
                + $"list scroll sensitivity = {sensitivity}, help box = {helpBox}";

            foreach (string path in LevelGameplayScenes)
            {
                Scene level = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                Camera camera = AlphaRunFrameBuilder.FindInScene<Camera>(level);
                LevelCameraFit fit = camera != null ? camera.GetComponent<LevelCameraFit>() : null;
                if (fit == null)
                {
                    report += $", {level.name} camera fit = MISSING";
                    continue;
                }

                var so = new SerializedObject(fit);
                report += $", {level.name} bands top {so.FindProperty("_topBand").floatValue:0.##} right {so.FindProperty("_rightBand").floatValue:0.##}";
            }

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.path.StartsWith(LegacyLevelFolder))
                {
                    report += $", {System.IO.Path.GetFileNameWithoutExtension(scene.path)} in build = {(scene.enabled ? "yes" : "no")}";
                }
            }

            Debug.Log(report);
        }
    }
}
