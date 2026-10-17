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
    /// After the first playtest: a Sell button on the tower card, and a developer help box on the
    /// HUD that lists the controls (F1 hides it). Both go into the <c>UI</c> scene.
    /// </summary>
    /// <remarks>
    /// Safe to run again: the button and the box are added only when missing. Runs after
    /// Build Run Frame, Build Towers and Build Hazards, which make the HUD and the card.
    /// </remarks>
    internal static class AlphaSellAndHelpBuilder
    {
        private const string UiScenePath = "Assets/_Game/Scenes/UI.unity";
        private const string ActionsPath = "Assets/_Game/Input/HoldTheHill.inputactions";

        private const float Margin = 24f;
        private const float CardGrowth = 64f;
        private static readonly Vector2 SellButtonSize = new Vector2(360f - 32f, 56f);
        private static readonly Vector2 HelpBoxSize = new Vector2(500f, 330f);
        private static readonly Color HelpBoxColor = new Color(0.08f, 0.08f, 0.1f, 0.7f);
        private const int HelpFontSize = 22;

        [MenuItem("Tools/Hold the Hill/Alpha/Build Sell Button and Controls Help")]
        private static void Build()
        {
            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
            EnsureSellButton(ui);
            EnsureControlsOverlay(ui);
            EditorSceneManager.MarkSceneDirty(ui);
            EditorSceneManager.SaveScene(ui);
            AssetDatabase.SaveAssets();

            ReportAfterReload();
            Debug.Log("Sell button and controls help built: click a placed tower to see Sell; F1 hides the help box.");
        }

        /// <summary>Entry point for a batch run: <c>-executeMethod HoldTheHill.EditorTools.AlphaSellAndHelpBuilder.BuildFromCommandLine</c>.</summary>
        public static void BuildFromCommandLine()
        {
            Build();
        }

        // Same shape as the burrow button in AlphaHazardsBuilder: the card grows and the button sits below burrow.
        private static void EnsureSellButton(Scene scene)
        {
            TowerCardPanel panel = AlphaRunFrameBuilder.FindInScene<TowerCardPanel>(scene);
            if (panel == null)
            {
                Debug.LogError("The UI scene has no tower card yet; run Build Towers first.");
                return;
            }

            var so = new SerializedObject(panel);
            if (so.FindProperty("_sell").objectReferenceValue != null)
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

            Button sell = AlphaRunFrameBuilder.MakeButton(card.transform, "Sell Button", string.Format(UiStrings.SellFormat, 0), card.layer);
            AlphaRunFrameBuilder.Anchor((RectTransform)sell.transform, new Vector2(0.5f, 1f), new Vector2(0f, -192f), SellButtonSize);

            so.FindProperty("_sell").objectReferenceValue = sell;
            so.FindProperty("_sellLabel").objectReferenceValue = sell.GetComponentInChildren<TMP_Text>();
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureControlsOverlay(Scene scene)
        {
            if (AlphaRunFrameBuilder.FindInScene<ControlsOverlay>(scene) != null)
            {
                return;
            }

            Canvas canvas = AlphaRunFrameBuilder.FindInScene<Canvas>(scene);
            if (canvas == null)
            {
                Debug.LogError("The UI scene has no canvas yet; run Build Run Frame first.");
                return;
            }

            var actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(ActionsPath);
            if (actions == null)
            {
                Debug.LogError($"The actions asset is missing at {ActionsPath}.");
            }

            int layer = canvas.gameObject.layer;
            Transform hud = canvas.transform.Find("HUD");
            Transform parent = hud != null ? hud : canvas.transform;

            // Under the Honeydew count, top-left; the tower card and the power-up slot sit along the bottom.
            Image box = AlphaRunFrameBuilder.MakeImage(parent, "Controls Help", HelpBoxColor, layer);
            AlphaRunFrameBuilder.Anchor(box.rectTransform, new Vector2(0f, 1f), new Vector2(Margin, -Margin - 64f), HelpBoxSize);
            box.raycastTarget = false;

            TextMeshProUGUI text = AlphaRunFrameBuilder.MakeText(box.transform, "Text",
                UiStrings.ControlsTitle + "\n" + string.Join("\n", UiStrings.ControlsLines), HelpFontSize, TextAlignmentOptions.TopLeft, layer);
            AlphaRunFrameBuilder.Stretch(text.rectTransform, new Vector2(16f, 12f));

            // The component sits on the HUD object, above the box it toggles, so hiding the box never silences F1.
            ControlsOverlay overlay = parent.gameObject.AddComponent<ControlsOverlay>();
            var so = new SerializedObject(overlay);
            so.FindProperty("_panel").objectReferenceValue = box.gameObject;
            so.FindProperty("_text").objectReferenceValue = text;
            so.FindProperty("_actions").objectReferenceValue = actions;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // Opens the saved scene again and says what it holds, so a batch log shows the truth.
        private static void ReportAfterReload()
        {
            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
            TowerCardPanel card = AlphaRunFrameBuilder.FindInScene<TowerCardPanel>(ui);
            string sell = card != null ? AlphaRunFrameBuilder.Name(new SerializedObject(card), "_sell") : "no card";
            Debug.Log($"Tower card: sell = {sell}");

            ControlsOverlay overlay = AlphaRunFrameBuilder.FindInScene<ControlsOverlay>(ui);
            if (overlay != null)
            {
                var so = new SerializedObject(overlay);
                Debug.Log($"Controls help: panel = {AlphaRunFrameBuilder.Name(so, "_panel")}, text = {AlphaRunFrameBuilder.Name(so, "_text")}, actions = {AlphaRunFrameBuilder.Name(so, "_actions")}");
            }
            else
            {
                Debug.LogError("Controls help is missing from the UI scene.");
            }
        }
    }
}
