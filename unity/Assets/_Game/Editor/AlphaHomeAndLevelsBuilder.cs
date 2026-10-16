using System.Collections.Generic;
using HoldTheHill.Core;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.GameFlow;
using HoldTheHill.Features.Levels;
using HoldTheHill.Features.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using WaveSpec = HoldTheHill.EditorTools.AlphaCoreLoopBuilder.WaveSpec;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// Alpha step 3, home and levels. The <c>Home</c> scene: a title page (Play, Quit) and a level
    /// select page with one button per <see cref="LevelCatalogue"/> entry. The levels
    /// <c>L00_TutorialHill</c> and <c>L01_Meadow</c>: each a <c>_Terrain</c> + <c>_Gameplay</c> pair
    /// copied from Hill01 the first time, given its own route, camera colour and wave list, then
    /// wired by the step 1 and step 2 builders. Adds every new scene to the Build Settings list.
    /// </summary>
    /// <remarks>
    /// Safe to run again: scenes and assets that exist are kept (so hand edits to a route or a wave
    /// list survive), and only references are refreshed. A level's route and colour are written
    /// once, when its gameplay scene is first copied.
    /// </remarks>
    internal static class AlphaHomeAndLevelsBuilder
    {
        private const string ScenesFolder = "Assets/_Game/Scenes";
        private const string LevelsFolder = ScenesFolder + "/Levels";
        private const string HomeScenePath = ScenesFolder + "/Home.unity";
        private const string BootstrapScenePath = ScenesFolder + "/Bootstrap.unity";
        private const string TemplateTerrainPath = LevelsFolder + "/Hill01/Hill01_Terrain.unity";
        private const string TemplateGameplayPath = LevelsFolder + "/Hill01/Hill01_Gameplay.unity";
        private const string WaveDataFolder = "Assets/_Game/Features/Enemies/Data";

        private const string HomeSceneName = "Home";
        private const int GameTitleFontSize = 120;
        private const int HeadingFontSize = 64;
        private static readonly Color HomeBackground = new Color(0.08f, 0.13f, 0.1f);

        /// <summary>What differs between the placeholder levels: the route, the ground colour, the waves.</summary>
        private sealed class LevelSpec
        {
            public Vector2Int[] Route;
            public Color CameraBackground;
            public WaveSpec[] Waves;
        }

        // Grid cells, cell size 1. The camera is orthographic size 5 at the origin, so x in [-9, 9] and
        // y in [-5, 5] are on screen at 16:9; routes enter and leave off the left and right edges.
        private static readonly Dictionary<string, LevelSpec> Specs = new Dictionary<string, LevelSpec>
        {
            ["L00_TutorialHill"] = new LevelSpec
            {
                Route = new[] { new Vector2Int(-10, 0), new Vector2Int(0, 0), new Vector2Int(0, -3), new Vector2Int(9, -3) },
                CameraBackground = new Color(0.36f, 0.55f, 0.32f),
                Waves = new[]
                {
                    new WaveSpec(4, 1.4f), new WaveSpec(6, 1.2f), new WaveSpec(8, 1.0f),
                    new WaveSpec(10, 0.9f), new WaveSpec(12, 0.8f),
                },
            },
            ["L01_Meadow"] = new LevelSpec
            {
                Route = new[]
                {
                    new Vector2Int(-10, 3), new Vector2Int(-6, 3), new Vector2Int(-6, -3), new Vector2Int(-1, -3),
                    new Vector2Int(-1, 3), new Vector2Int(4, 3), new Vector2Int(4, -3), new Vector2Int(9, -3),
                },
                CameraBackground = new Color(0.56f, 0.66f, 0.3f),
                Waves = new[]
                {
                    new WaveSpec(5, 1.2f), new WaveSpec(7, 1.1f), new WaveSpec(9, 1.0f), new WaveSpec(11, 0.9f),
                    new WaveSpec(13, 0.8f), new WaveSpec(15, 0.7f), new WaveSpec(17, 0.65f), new WaveSpec(19, 0.6f),
                    new WaveSpec(21, 0.55f), new WaveSpec(24, 0.5f),
                },
            },
        };

        [MenuItem("Tools/Hold the Hill/Alpha/Build Home and Levels (all)")]
        private static void BuildAll()
        {
            EnsureCoreLoopAssets();
            foreach (LevelInfo level in LevelCatalogue.All)
            {
                BuildLevel(level);
            }
            BuildHome();
            AssetDatabase.SaveAssets();
            Debug.Log("Alpha home and levels built: open Bootstrap and press Play. Play > pick a hill. Esc > Home comes back here.");
        }

        [MenuItem("Tools/Hold the Hill/Alpha/Build Home Screen")]
        private static void BuildHomeMenu()
        {
            BuildHome();
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Tools/Hold the Hill/Alpha/Build Level L00_TutorialHill")]
        private static void BuildTutorialHill()
        {
            BuildLevelMenu("L00_TutorialHill");
        }

        [MenuItem("Tools/Hold the Hill/Alpha/Build Level L01_Meadow")]
        private static void BuildMeadow()
        {
            BuildLevelMenu("L01_Meadow");
        }

        /// <summary>Entry point for a batch run: <c>-executeMethod HoldTheHill.EditorTools.AlphaHomeAndLevelsBuilder.BuildFromCommandLine</c>.</summary>
        public static void BuildFromCommandLine()
        {
            BuildAll();
        }

        private static void BuildLevelMenu(string levelId)
        {
            if (!LevelCatalogue.TryFind(levelId, out LevelInfo level))
            {
                Debug.LogError($"'{levelId}' is not in the LevelCatalogue.");
                return;
            }

            EnsureCoreLoopAssets();
            BuildLevel(level);
            AssetDatabase.SaveAssets();
        }

        // The levels are copies of Hill01 and their waves use the basic enemy, so step 1 must have run.
        private static void EnsureCoreLoopAssets()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>(AlphaCoreLoopBuilder.EnemyPrefabPath) == null)
            {
                Debug.Log("The core loop assets are missing; building Hill01 first.");
                AlphaCoreLoopBuilder.BuildFromCommandLine();
                AlphaRunFrameBuilder.BuildFromCommandLine();
            }
        }

        // ---- Levels ---------------------------------------------------------------------------

        private static void BuildLevel(LevelInfo level)
        {
            if (!Specs.TryGetValue(level.Id, out LevelSpec spec))
            {
                Debug.LogError($"No placeholder route and waves are written down for '{level.Id}'.");
                return;
            }

            string folder = $"{LevelsFolder}/{level.Id}";
            AlphaCoreLoopBuilder.EnsureFolder(folder);
            string terrainPath = $"{folder}/{LevelScenes.Terrain(level.Id)}.unity";
            string gameplayPath = $"{folder}/{LevelScenes.Gameplay(level.Id)}.unity";
            string wavesPath = $"{WaveDataFolder}/{level.Id}_Waves.asset";

            CopySceneIfMissing(TemplateTerrainPath, terrainPath);
            bool freshGameplay = CopySceneIfMissing(TemplateGameplayPath, gameplayPath);

            var enemy = AssetDatabase.LoadAssetAtPath<GameObject>(AlphaCoreLoopBuilder.EnemyPrefabPath);
            AlphaCoreLoopBuilder.EnsureWaves(wavesPath, level.Id, enemy, spec.Waves);

            if (freshGameplay)
            {
                ShapeNewLevel(gameplayPath, spec);
            }

            AlphaCoreLoopBuilder.WireGameplayScene(gameplayPath, wavesPath);
            AlphaRunFrameBuilder.WireLevelGameplay(gameplayPath);
            EnsureInBuildSettings(terrainPath, gameplayPath);
            Debug.Log($"Level {level.Id} ({level.Title}) built: {LevelScenes.Terrain(level.Id)} + {LevelScenes.Gameplay(level.Id)}, {spec.Waves.Length} waves.");
        }

        // True when the copy was made now, so the caller knows the scene still looks like Hill01.
        private static bool CopySceneIfMissing(string templatePath, string scenePath)
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath) != null)
            {
                return false;
            }

            if (!AssetDatabase.CopyAsset(templatePath, scenePath))
            {
                Debug.LogError($"Could not copy {templatePath} to {scenePath}.");
                return false;
            }

            Debug.Log($"Made {scenePath} from {templatePath}.");
            return true;
        }

        // A level that was just copied from Hill01 gets its own route, Queen spot and ground colour.
        private static void ShapeNewLevel(string gameplayPath, LevelSpec spec)
        {
            Scene scene = EditorSceneManager.OpenScene(gameplayPath, OpenSceneMode.Single);

            EnemyPath path = AlphaRunFrameBuilder.FindInScene<EnemyPath>(scene);
            if (path == null)
            {
                path = HoldTheHillMenu.CreateExamplePathObject().GetComponent<EnemyPath>();
            }
            HoldTheHillMenu.SetRoute(path.transform, spec.Route);

            Queen queen = AlphaRunFrameBuilder.FindInScene<Queen>(scene);
            if (queen != null && path.transform.childCount > 0)
            {
                queen.transform.position = path.transform.GetChild(path.transform.childCount - 1).position;
            }

            Camera camera = AlphaRunFrameBuilder.FindInScene<Camera>(scene);
            if (camera != null)
            {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = spec.CameraBackground;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        // ---- Home -----------------------------------------------------------------------------

        private static void BuildHome()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(HomeScenePath) == null)
            {
                Scene fresh = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(fresh, HomeScenePath);
                Debug.Log($"Made {HomeScenePath}.");
            }

            Scene scene = EditorSceneManager.OpenScene(HomeScenePath, OpenSceneMode.Single);
            int uiLayer = LayerMask.NameToLayer("UI");

            EnsureHomeCamera(scene);
            Canvas canvas = EnsureCanvas(scene, uiLayer);
            EnsureHomeScreen(scene, canvas.transform, uiLayer);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);

            WireBootstrap();
            AlphaRunFrameBuilder.WireUiSceneFile();
            EnsureInBuildSettings(HomeScenePath);
            ReportHomeAfterReload();
        }

        // The home screen is a screen-space canvas, but with no camera at all the Game view says
        // "No cameras rendering". One flat colour, from z = -10 like the level cameras.
        private static void EnsureHomeCamera(Scene scene)
        {
            Camera camera = AlphaRunFrameBuilder.FindInScene<Camera>(scene);
            if (camera == null)
            {
                var go = new GameObject("Home Camera");
                go.tag = "MainCamera";
                camera = go.AddComponent<Camera>();
                go.AddComponent<AudioListener>();
            }

            camera.orthographic = true;
            camera.orthographicSize = 5f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = HomeBackground;
            camera.transform.position = new Vector3(0f, 0f, -10f);
        }

        private static Canvas EnsureCanvas(Scene scene, int uiLayer)
        {
            Canvas canvas = AlphaRunFrameBuilder.FindInScene<Canvas>(scene);
            if (canvas == null)
            {
                var go = new GameObject("Canvas", typeof(RectTransform));
                go.layer = uiLayer;
                canvas = go.AddComponent<Canvas>();
                go.AddComponent<CanvasScaler>();
                go.AddComponent<GraphicRaycaster>();
            }
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            }
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = AlphaRunFrameBuilder.ReferenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        private static void EnsureHomeScreen(Scene scene, Transform canvas, int layer)
        {
            HomeScreen screen = AlphaRunFrameBuilder.FindInScene<HomeScreen>(scene);
            if (screen != null)
            {
                return;
            }

            RectTransform root = AlphaRunFrameBuilder.EnsureStretchedChild(canvas, "Home Screen", layer);

            // Title page: the game's name, Play, Quit.
            RectTransform titlePage = AlphaRunFrameBuilder.EnsureStretchedChild(root, "Title Page", layer);
            TextMeshProUGUI title = AlphaRunFrameBuilder.MakeText(titlePage, "Title", UiStrings.GameTitle, GameTitleFontSize,
                TextAlignmentOptions.Center, layer);
            AlphaRunFrameBuilder.Anchor(title.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, 200f), new Vector2(1400f, 160f));
            Button play = AlphaRunFrameBuilder.MakeButton(titlePage, "Play", UiStrings.Play, layer);
            Button quit = AlphaRunFrameBuilder.MakeButton(titlePage, "Quit", UiStrings.Quit, layer);
            AlphaRunFrameBuilder.Anchor((RectTransform)play.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -20f), AlphaRunFrameBuilder.MenuButtonSize);
            AlphaRunFrameBuilder.Anchor((RectTransform)quit.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -120f), AlphaRunFrameBuilder.MenuButtonSize);

            // Level select page: a heading, one button per level, Back.
            RectTransform levelPage = AlphaRunFrameBuilder.EnsureStretchedChild(root, "Level Select Page", layer);
            TextMeshProUGUI heading = AlphaRunFrameBuilder.MakeText(levelPage, "Heading", UiStrings.ChooseLevel, HeadingFontSize,
                TextAlignmentOptions.Center, layer);
            AlphaRunFrameBuilder.Anchor(heading.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, 200f), new Vector2(1200f, 90f));

            float y = 60f;
            foreach (LevelInfo level in LevelCatalogue.All)
            {
                Button button = AlphaRunFrameBuilder.MakeButton(levelPage, level.Id, level.Title, layer);
                AlphaRunFrameBuilder.Anchor((RectTransform)button.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, y), AlphaRunFrameBuilder.MenuButtonSize);
                var levelButton = button.gameObject.AddComponent<LevelButton>();
                var buttonSo = new SerializedObject(levelButton);
                buttonSo.FindProperty("_levelId").stringValue = level.Id;
                buttonSo.ApplyModifiedPropertiesWithoutUndo();
                y -= 100f;
            }

            Button back = AlphaRunFrameBuilder.MakeButton(levelPage, "Back", UiStrings.Back, layer);
            AlphaRunFrameBuilder.Anchor((RectTransform)back.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, y - 40f), AlphaRunFrameBuilder.MenuButtonSize);

            screen = root.gameObject.AddComponent<HomeScreen>();
            var so = new SerializedObject(screen);
            so.FindProperty("_titlePage").objectReferenceValue = titlePage.gameObject;
            so.FindProperty("_levelSelectPage").objectReferenceValue = levelPage.gameObject;
            so.FindProperty("_play").objectReferenceValue = play;
            so.FindProperty("_quit").objectReferenceValue = quit;
            so.FindProperty("_back").objectReferenceValue = back;
            so.ApplyModifiedPropertiesWithoutUndo();

            levelPage.gameObject.SetActive(false);
        }

        // The loader in Bootstrap learns the home scene's name; its old default-level list is gone.
        private static void WireBootstrap()
        {
            Scene scene = EditorSceneManager.OpenScene(BootstrapScenePath, OpenSceneMode.Single);
            SceneLoader loader = AlphaRunFrameBuilder.FindInScene<SceneLoader>(scene);
            if (loader == null)
            {
                Debug.LogError("Bootstrap has no SceneLoader.");
                return;
            }

            var so = new SerializedObject(loader);
            so.FindProperty("_homeScene").stringValue = HomeSceneName;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        // ---- Build Settings -------------------------------------------------------------------

        // Scenes load by name only when they are on the Build Settings list, in the Editor too.
        // This writes ProjectSettings/EditorBuildSettings.asset; commit that file only with a yes.
        private static void EnsureInBuildSettings(params string[] scenePaths)
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            bool changed = false;
            foreach (string path in scenePaths)
            {
                if (scenes.Exists(s => s.path == path))
                {
                    continue;
                }
                scenes.Add(new EditorBuildSettingsScene(path, true));
                changed = true;
                Debug.Log($"Added {path} to the Build Settings scene list.");
            }

            if (changed)
            {
                EditorBuildSettings.scenes = scenes.ToArray();
            }
        }

        // ---- Report ---------------------------------------------------------------------------

        private static void ReportHomeAfterReload()
        {
            Scene home = EditorSceneManager.OpenScene(HomeScenePath, OpenSceneMode.Single);
            Camera camera = AlphaRunFrameBuilder.FindInScene<Camera>(home);
            Debug.Log($"Home: camera {(camera != null ? $"at z = {camera.transform.position.z}" : "MISSING")}, "
                      + $"Canvas {(AlphaRunFrameBuilder.FindInScene<Canvas>(home) != null ? "present" : "MISSING")}");

            HomeScreen screen = AlphaRunFrameBuilder.FindInScene<HomeScreen>(home);
            if (screen == null)
            {
                Debug.LogError("Home Screen is missing from the Home scene after save and reopen.");
            }
            else
            {
                var so = new SerializedObject(screen);
                Debug.Log($"Home Screen: title page = {AlphaRunFrameBuilder.Name(so, "_titlePage")}, level page = {AlphaRunFrameBuilder.Name(so, "_levelSelectPage")}, "
                          + $"play = {AlphaRunFrameBuilder.Name(so, "_play")}, quit = {AlphaRunFrameBuilder.Name(so, "_quit")}, back = {AlphaRunFrameBuilder.Name(so, "_back")}");
            }

            foreach (LevelButton button in Object.FindObjectsByType<LevelButton>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                Debug.Log($"Level button '{button.name}': level id = {button.LevelId}");
            }

            Scene bootstrap = EditorSceneManager.OpenScene(BootstrapScenePath, OpenSceneMode.Single);
            SceneLoader loader = AlphaRunFrameBuilder.FindInScene<SceneLoader>(bootstrap);
            Debug.Log($"Bootstrap SceneLoader: home scene = {(loader != null ? new SerializedObject(loader).FindProperty("_homeScene").stringValue : "MISSING")}");

            var listed = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                listed.Add(System.IO.Path.GetFileNameWithoutExtension(scene.path) + (scene.enabled ? "" : " (disabled)"));
            }
            Debug.Log($"Build Settings scenes: {string.Join(", ", listed)}");
        }
    }
}
