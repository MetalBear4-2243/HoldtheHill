using System.Collections.Generic;
using HoldTheHill.Features.Economy;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.GameFlow;
using HoldTheHill.Features.UI;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// Alpha step 2, the run frame. In <c>Hill01_Gameplay</c>: the Queen, and a Level Run object
    /// that reads the run keys and decides win or lose. In the <c>UI</c> scene: one canvas with the
    /// Honeydew count, the wave count, the Queen bar, the Next Wave button, the pause panel and the
    /// result panel. The Canvas, its button and the EventSystem that the gameplay scene carried move
    /// to the UI scene, where the README says they belong.
    /// </summary>
    /// <remarks>
    /// Safe to run again: scene objects are found by component and only their references are refreshed.
    /// Runs after <see cref="AlphaCoreLoopBuilder"/>, which makes the spawner, wallet and path.
    /// </remarks>
    internal static class AlphaRunFrameBuilder
    {
        private const string GameplayScenePath = "Assets/_Game/Scenes/Levels/Hill01/Hill01_Gameplay.unity";
        private const string UiScenePath = "Assets/_Game/Scenes/UI.unity";
        private const string ActionsPath = "Assets/_Game/Input/HoldTheHill.inputactions";

        private static readonly Vector2 ReferenceResolution = new Vector2(1920f, 1080f);
        private static readonly Color DimColor = new Color(0f, 0f, 0f, 0.6f);
        private static readonly Color BarBackColor = new Color(0.1f, 0.1f, 0.12f, 0.9f);
        private static readonly Color BarFillColor = new Color(0.85f, 0.35f, 0.55f, 1f);
        private static readonly Color ButtonTextColor = new Color(0.2f, 0.2f, 0.2f, 1f);

        private const float Margin = 24f;
        private const int HudFontSize = 36;
        private const int TitleFontSize = 64;
        private const int ButtonFontSize = 28;
        private static readonly Vector2 MenuButtonSize = new Vector2(320f, 72f);
        private static readonly Vector2 NextWaveButtonSize = new Vector2(240f, 64f);

        [MenuItem("Tools/Hold the Hill/Alpha/Build Run Frame (HUD, keys, win and lose)")]
        private static void Build()
        {
            Sprite circle = AlphaCoreLoopBuilder.EnsureSprite("Circle", circle: true);

            Scene gameplay = EditorSceneManager.OpenScene(GameplayScenePath, OpenSceneMode.Single);
            WireGameplayScene(gameplay, circle);

            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Additive);
            SceneManager.SetActiveScene(ui);
            MoveUiOutOfGameplay(gameplay, ui);
            WireUiScene(ui);

            EditorSceneManager.MarkSceneDirty(gameplay);
            EditorSceneManager.MarkSceneDirty(ui);
            EditorSceneManager.SaveScene(gameplay);
            EditorSceneManager.SaveScene(ui);
            AssetDatabase.SaveAssets();

            ReportAfterReload();
            Debug.Log("Alpha run frame built: open Hill01_Gameplay and press Play. Space sends a wave, T toggles 2x, Esc pauses.");
        }

        /// <summary>Entry point for a batch run: <c>-executeMethod HoldTheHill.EditorTools.AlphaRunFrameBuilder.BuildFromCommandLine</c>.</summary>
        public static void BuildFromCommandLine()
        {
            Build();
        }

        // ---- Gameplay scene -------------------------------------------------------------------

        private static void WireGameplayScene(Scene scene, Sprite circle)
        {
            // Loaded after the scene opened: opening a scene unloads loose assets.
            circle = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(circle));
            var actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(ActionsPath);
            if (actions == null)
            {
                Debug.LogError($"The actions asset is missing at {ActionsPath}.");
            }

            // The Indev scene's camera sits at z = +10, behind everything it should look at, so the
            // Game view shows only its clear colour. A 2D camera looks down +z from z = -10.
            Camera camera = FindInScene<Camera>(scene);
            if (camera != null && camera.transform.position.z >= 0f)
            {
                Vector3 position = camera.transform.position;
                camera.transform.position = new Vector3(position.x, position.y, -10f);
                Debug.Log($"Moved '{camera.name}' from z = {position.z} to z = -10 so it sees the map.");
            }

            EnemyPath path = FindInScene<EnemyPath>(scene);
            EnemySpawner spawner = FindInScene<EnemySpawner>(scene);
            HoneydewWallet wallet = FindInScene<HoneydewWallet>(scene);
            if (spawner == null || wallet == null)
            {
                Debug.LogError("Hill01_Gameplay has no spawner or wallet yet. Run Build Core Loop first.");
            }

            Queen queen = FindInScene<Queen>(scene);
            if (queen == null)
            {
                var go = new GameObject("Queen");
                queen = go.AddComponent<Queen>();

                // The Queen stands where the path ends, so the player sees what the enemies walk to.
                if (path != null && path.transform.childCount > 0)
                {
                    go.transform.position = path.transform.GetChild(path.transform.childCount - 1).position;
                }

                go.transform.localScale = Vector3.one * 0.9f;
                var renderer = go.AddComponent<SpriteRenderer>();
                renderer.sprite = circle;
                renderer.color = new Color(0.95f, 0.8f, 0.25f);
                renderer.sortingOrder = 4;
            }

            LevelRun run = FindInScene<LevelRun>(scene);
            if (run == null)
            {
                run = new GameObject("Level Run").AddComponent<LevelRun>();
            }

            var runSo = new SerializedObject(run);
            runSo.FindProperty("_queen").objectReferenceValue = queen;
            runSo.FindProperty("_spawner").objectReferenceValue = spawner;
            runSo.FindProperty("_wallet").objectReferenceValue = wallet;
            runSo.ApplyModifiedPropertiesWithoutUndo();

            RunControls controls = run.GetComponent<RunControls>();
            if (controls == null)
            {
                controls = run.gameObject.AddComponent<RunControls>();
            }

            var controlsSo = new SerializedObject(controls);
            controlsSo.FindProperty("_actions").objectReferenceValue = actions;
            controlsSo.ApplyModifiedPropertiesWithoutUndo();
        }

        // ---- Moving the old UI objects --------------------------------------------------------

        // The Indev gameplay scene carried a Canvas with one button and an EventSystem. They belong in
        // the UI scene (README: "HUD, menus, EventSystem"), and two EventSystems would fight.
        private static void MoveUiOutOfGameplay(Scene gameplay, Scene ui)
        {
            EventSystem gameplayEvents = FindInScene<EventSystem>(gameplay);
            if (gameplayEvents != null)
            {
                if (FindInScene<EventSystem>(ui) == null)
                {
                    SceneManager.MoveGameObjectToScene(gameplayEvents.transform.root.gameObject, ui);
                    Debug.Log("Moved the EventSystem from Hill01_Gameplay to the UI scene.");
                }
                else
                {
                    Object.DestroyImmediate(gameplayEvents.transform.root.gameObject);
                    Debug.Log("Removed the EventSystem from Hill01_Gameplay: the UI scene already has one.");
                }
            }

            Canvas gameplayCanvas = FindInScene<Canvas>(gameplay);
            if (gameplayCanvas == null)
            {
                return;
            }

            GameObject canvasRoot = gameplayCanvas.transform.root.gameObject;
            Canvas uiCanvas = FindInScene<Canvas>(ui);
            if (uiCanvas == null)
            {
                SceneManager.MoveGameObjectToScene(canvasRoot, ui);
                Debug.Log("Moved the Canvas and its Next Wave button from Hill01_Gameplay to the UI scene.");
                return;
            }

            // The UI scene already has the HUD canvas; keep the button if it is the only one.
            Button oldButton = canvasRoot.GetComponentInChildren<Button>(true);
            if (oldButton != null && FindInScene<NextWaveButton>(ui) == null)
            {
                oldButton.transform.SetParent(uiCanvas.transform, worldPositionStays: false);
                Debug.Log("Moved the Next Wave button from Hill01_Gameplay into the UI scene's canvas.");
            }
            Object.DestroyImmediate(canvasRoot);
            Debug.Log("Removed the Canvas from Hill01_Gameplay: the UI scene already has one.");
        }

        // ---- UI scene -------------------------------------------------------------------------

        private static void WireUiScene(Scene scene)
        {
            int uiLayer = LayerMask.NameToLayer("UI");

            EventSystem events = FindInScene<EventSystem>(scene);
            if (events == null)
            {
                var go = new GameObject("EventSystem");
                go.AddComponent<EventSystem>();
                go.AddComponent<InputSystemUIInputModule>();
            }

            Canvas canvas = FindInScene<Canvas>(scene);
            if (canvas == null)
            {
                var go = new GameObject("Canvas", typeof(RectTransform));
                go.layer = uiLayer;
                canvas = go.AddComponent<Canvas>();
                go.AddComponent<CanvasScaler>();
                go.AddComponent<GraphicRaycaster>();
            }
            canvas.gameObject.name = "Canvas";
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            }
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = ReferenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            RectTransform hud = EnsureStretchedChild(canvas.transform, "HUD", uiLayer);
            EnsureHoneydewLabel(scene, hud, uiLayer);
            EnsureWaveLabel(scene, hud, uiLayer);
            EnsureQueenBar(scene, hud, uiLayer);
            EnsureNextWaveButton(scene, canvas, hud, uiLayer);
            EnsurePauseMenu(scene, canvas.transform, uiLayer);
            EnsureResultPanel(scene, canvas.transform, uiLayer);
        }

        private static void EnsureHoneydewLabel(Scene scene, RectTransform hud, int layer)
        {
            HoneydewLabel label = FindInScene<HoneydewLabel>(scene);
            if (label != null)
            {
                return;
            }

            TextMeshProUGUI text = MakeText(hud, "Honeydew", string.Format(UiStrings.HoneydewFormat, 0), HudFontSize,
                TextAlignmentOptions.MidlineLeft, layer);
            Anchor(text.rectTransform, new Vector2(0f, 1f), new Vector2(Margin, -Margin), new Vector2(480f, 48f));
            text.gameObject.AddComponent<HoneydewLabel>();
        }

        private static void EnsureWaveLabel(Scene scene, RectTransform hud, int layer)
        {
            WaveLabel label = FindInScene<WaveLabel>(scene);
            if (label != null)
            {
                return;
            }

            TextMeshProUGUI text = MakeText(hud, "Wave", string.Format(UiStrings.WaveFormat, 0, 0), HudFontSize,
                TextAlignmentOptions.Center, layer);
            Anchor(text.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -Margin), new Vector2(480f, 48f));
            text.gameObject.AddComponent<WaveLabel>();
        }

        private static void EnsureQueenBar(Scene scene, RectTransform hud, int layer)
        {
            QueenHealthBar bar = FindInScene<QueenHealthBar>(scene);
            if (bar == null)
            {
                Image back = MakeImage(hud, "Queen Bar", BarBackColor, layer);
                Anchor(back.rectTransform, new Vector2(1f, 1f), new Vector2(-Margin, -Margin), new Vector2(480f, 40f));
                back.sprite = BuiltinSprite("UI/Skin/Background.psd");
                back.type = Image.Type.Sliced;

                Image fill = MakeImage(back.transform, "Fill", BarFillColor, layer);
                Stretch(fill.rectTransform, new Vector2(4f, 4f));
                fill.sprite = BuiltinSprite("UI/Skin/UISprite.psd");
                fill.type = Image.Type.Filled;
                fill.fillMethod = Image.FillMethod.Horizontal;
                fill.fillOrigin = (int)Image.OriginHorizontal.Left;
                fill.fillAmount = 1f;

                TextMeshProUGUI label = MakeText(back.transform, "Label", UiStrings.QueenHealth, 24, TextAlignmentOptions.Center, layer);
                Stretch(label.rectTransform, Vector2.zero);
                label.raycastTarget = false;

                bar = back.gameObject.AddComponent<QueenHealthBar>();
                var so = new SerializedObject(bar);
                so.FindProperty("_fill").objectReferenceValue = fill;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static void EnsureNextWaveButton(Scene scene, Canvas canvas, RectTransform hud, int layer)
        {
            if (FindInScene<NextWaveButton>(scene) != null)
            {
                return;
            }

            // The button the gameplay scene used to carry, if it came along; otherwise a new one.
            Button button = null;
            foreach (Button candidate in canvas.GetComponentsInChildren<Button>(true))
            {
                if (candidate.GetComponentInParent<PauseMenu>(true) == null && candidate.GetComponentInParent<ResultPanel>(true) == null)
                {
                    button = candidate;
                    break;
                }
            }

            if (button == null)
            {
                button = MakeButton(hud, "Next Wave Button", UiStrings.NextWave, layer);
            }
            else
            {
                button.gameObject.name = "Next Wave Button";
                button.transform.SetParent(hud, worldPositionStays: false);
                TMP_Text text = button.GetComponentInChildren<TMP_Text>(true);
                if (text != null)
                {
                    text.text = UiStrings.NextWave;
                    text.fontSize = ButtonFontSize;
                }

                // Its old listener pointed into the gameplay scene; NextWaveButton wires it at runtime.
                for (int i = button.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    UnityEventTools.RemovePersistentListener(button.onClick, i);
                }
            }

            Anchor((RectTransform)button.transform, new Vector2(1f, 0f), new Vector2(-Margin, Margin), NextWaveButtonSize);
            button.gameObject.AddComponent<NextWaveButton>();
        }

        private static void EnsurePauseMenu(Scene scene, Transform canvas, int layer)
        {
            PauseMenu menu = FindInScene<PauseMenu>(scene);
            if (menu != null)
            {
                return;
            }

            RectTransform root = EnsureStretchedChild(canvas, "Pause Menu", layer);
            GameObject panel = MakeDimPanel(root, layer);
            TextMeshProUGUI title = MakeText(panel.transform, "Title", UiStrings.Paused, TitleFontSize, TextAlignmentOptions.Center, layer);
            Anchor(title.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, 180f), new Vector2(900f, 90f));

            Button resume = MakeButton(panel.transform, "Resume", UiStrings.Resume, layer);
            Button restart = MakeButton(panel.transform, "Restart", UiStrings.RestartLevel, layer);
            Button home = MakeButton(panel.transform, "Home", UiStrings.Home, layer);
            Anchor((RectTransform)resume.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, 50f), MenuButtonSize);
            Anchor((RectTransform)restart.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -50f), MenuButtonSize);
            Anchor((RectTransform)home.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -150f), MenuButtonSize);

            menu = root.gameObject.AddComponent<PauseMenu>();
            var so = new SerializedObject(menu);
            so.FindProperty("_panel").objectReferenceValue = panel;
            so.FindProperty("_resume").objectReferenceValue = resume;
            so.FindProperty("_restart").objectReferenceValue = restart;
            so.FindProperty("_home").objectReferenceValue = home;
            so.ApplyModifiedPropertiesWithoutUndo();

            panel.SetActive(false);
        }

        private static void EnsureResultPanel(Scene scene, Transform canvas, int layer)
        {
            ResultPanel result = FindInScene<ResultPanel>(scene);
            if (result != null)
            {
                return;
            }

            RectTransform root = EnsureStretchedChild(canvas, "Result", layer);
            GameObject panel = MakeDimPanel(root, layer);
            TextMeshProUGUI title = MakeText(panel.transform, "Title", UiStrings.Victory, TitleFontSize, TextAlignmentOptions.Center, layer);
            Anchor(title.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, 140f), new Vector2(1200f, 90f));

            Button restart = MakeButton(panel.transform, "Restart", UiStrings.RestartLevel, layer);
            Button home = MakeButton(panel.transform, "Home", UiStrings.Home, layer);
            Anchor((RectTransform)restart.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), MenuButtonSize);
            Anchor((RectTransform)home.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -100f), MenuButtonSize);

            result = root.gameObject.AddComponent<ResultPanel>();
            var so = new SerializedObject(result);
            so.FindProperty("_panel").objectReferenceValue = panel;
            so.FindProperty("_title").objectReferenceValue = title;
            so.FindProperty("_restart").objectReferenceValue = restart;
            so.FindProperty("_home").objectReferenceValue = home;
            so.ApplyModifiedPropertiesWithoutUndo();

            panel.SetActive(false);
        }

        // ---- uGUI helpers ---------------------------------------------------------------------

        private static RectTransform EnsureStretchedChild(Transform parent, string name, int layer)
        {
            Transform existing = parent.Find(name);
            if (existing != null)
            {
                return (RectTransform)existing;
            }

            var go = new GameObject(name, typeof(RectTransform));
            go.layer = layer;
            var rect = (RectTransform)go.transform;
            rect.SetParent(parent, worldPositionStays: false);
            Stretch(rect, Vector2.zero);
            return rect;
        }

        private static GameObject MakeDimPanel(Transform parent, int layer)
        {
            Image image = MakeImage(parent, "Panel", DimColor, layer);
            Stretch(image.rectTransform, Vector2.zero);
            return image.gameObject;
        }

        private static Image MakeImage(Transform parent, string name, Color color, int layer)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = layer;
            go.transform.SetParent(parent, worldPositionStays: false);
            var image = go.AddComponent<Image>();
            image.color = color;
            return image;
        }

        private static TextMeshProUGUI MakeText(Transform parent, string name, string content, int fontSize,
            TextAlignmentOptions alignment, int layer)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = layer;
            go.transform.SetParent(parent, worldPositionStays: false);
            var text = go.AddComponent<TextMeshProUGUI>();
            text.text = content;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            text.raycastTarget = false;
            return text;
        }

        private static Button MakeButton(Transform parent, string name, string label, int layer)
        {
            Image image = MakeImage(parent, name, Color.white, layer);
            image.sprite = BuiltinSprite("UI/Skin/UISprite.psd");
            image.type = Image.Type.Sliced;

            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            TextMeshProUGUI text = MakeText(image.transform, "Text", label, ButtonFontSize, TextAlignmentOptions.Center, layer);
            text.color = ButtonTextColor;
            Stretch(text.rectTransform, Vector2.zero);
            return button;
        }

        private static Sprite BuiltinSprite(string path)
        {
            return AssetDatabase.GetBuiltinExtraResource<Sprite>(path);
        }

        // Pins a rect to one anchor point; the position is measured from that corner or edge.
        private static void Anchor(RectTransform rect, Vector2 anchor, Vector2 position, Vector2 size)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        private static void Stretch(RectTransform rect, Vector2 padding)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = padding;
            rect.offsetMax = -padding;
        }

        private static T FindInScene<T>(Scene scene) where T : Component
        {
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                T found = root.GetComponentInChildren<T>(true);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        // ---- Report ---------------------------------------------------------------------------

        // Opens each saved scene again and says what it actually holds, so a batch log shows the truth.
        private static void ReportAfterReload()
        {
            Scene gameplay = EditorSceneManager.OpenScene(GameplayScenePath, OpenSceneMode.Single);
            LevelRun run = FindInScene<LevelRun>(gameplay);
            if (run == null)
            {
                Debug.LogError("Level Run is missing from Hill01_Gameplay after save and reopen.");
            }
            else
            {
                var so = new SerializedObject(run);
                Debug.Log($"Level Run: queen = {Name(so, "_queen")}, spawner = {Name(so, "_spawner")}, wallet = {Name(so, "_wallet")}");
                RunControls controls = run.GetComponent<RunControls>();
                Debug.Log($"Run Controls: actions = {(controls != null ? Name(new SerializedObject(controls), "_actions") : "missing component")}");
            }
            Debug.Log($"Hill01_Gameplay: Queen {(FindInScene<Queen>(gameplay) != null ? "present" : "MISSING")}, "
                      + $"Canvas {(FindInScene<Canvas>(gameplay) == null ? "none (moved)" : "STILL HERE")}, "
                      + $"EventSystem {(FindInScene<EventSystem>(gameplay) == null ? "none (moved)" : "STILL HERE")}");

            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
            Canvas canvas = FindInScene<Canvas>(ui);
            CanvasScaler scaler = canvas != null ? canvas.GetComponent<CanvasScaler>() : null;
            Debug.Log($"UI scene: EventSystem {(FindInScene<EventSystem>(ui) != null ? "present" : "MISSING")}, "
                      + $"Canvas {(canvas != null ? canvas.renderMode.ToString() : "MISSING")}, "
                      + $"scaler {(scaler != null ? $"{scaler.uiScaleMode} {scaler.referenceResolution}" : "MISSING")}");
            Debug.Log($"HUD: Honeydew {(FindInScene<HoneydewLabel>(ui) != null ? "present" : "MISSING")}, "
                      + $"Wave {(FindInScene<WaveLabel>(ui) != null ? "present" : "MISSING")}, "
                      + $"Next Wave button {(FindInScene<NextWaveButton>(ui) != null ? "present" : "MISSING")}");

            QueenHealthBar bar = FindInScene<QueenHealthBar>(ui);
            Debug.Log($"Queen bar: {(bar != null ? "fill = " + Name(new SerializedObject(bar), "_fill") : "MISSING")}");

            PauseMenu pause = FindInScene<PauseMenu>(ui);
            if (pause != null)
            {
                var so = new SerializedObject(pause);
                Debug.Log($"Pause menu: panel = {Name(so, "_panel")}, resume = {Name(so, "_resume")}, restart = {Name(so, "_restart")}, home = {Name(so, "_home")}");
            }
            else
            {
                Debug.LogError("Pause menu is missing from the UI scene.");
            }

            ResultPanel result = FindInScene<ResultPanel>(ui);
            if (result != null)
            {
                var so = new SerializedObject(result);
                Debug.Log($"Result panel: panel = {Name(so, "_panel")}, title = {Name(so, "_title")}, restart = {Name(so, "_restart")}, home = {Name(so, "_home")}");
            }
            else
            {
                Debug.LogError("Result panel is missing from the UI scene.");
            }
        }

        private static string Name(SerializedObject so, string field)
        {
            Object value = so.FindProperty(field).objectReferenceValue;
            return value != null ? value.name : "none";
        }
    }
}
