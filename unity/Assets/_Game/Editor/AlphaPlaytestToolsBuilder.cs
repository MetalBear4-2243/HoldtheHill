using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.GameFlow;
using HoldTheHill.Features.Levels;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoldTheHill.EditorTools
{
    /// <summary>
    /// What the 10-31 play pass asked for in the scenes: <see cref="DevKeys"/> in the UI scene, with
    /// the centipede its F5 key sends, and a <see cref="LevelCameraFit"/> on each level's camera so
    /// the map clears the HUD. Leaves the Bootstrap scene open, ready for Play.
    /// </summary>
    /// <remarks>Safe to run again: each piece is added only when missing.</remarks>
    internal static class AlphaPlaytestToolsBuilder
    {
        private const string UiScenePath = "Assets/_Game/Scenes/UI.unity";
        private const string BootstrapScenePath = "Assets/_Game/Scenes/Bootstrap.unity";
        private const string CentipedePrefabPath = "Assets/_Game/Features/Enemies/Prefabs/Enemy_Centipede.prefab";
        private const string DevKeysName = "Dev Keys";

        private static readonly string[] LevelGameplayScenes =
        {
            "Assets/_Game/Scenes/Levels/L00_TutorialHill/L00_TutorialHill_Gameplay.unity",
            "Assets/_Game/Scenes/Levels/L01_Meadow/L01_Meadow_Gameplay.unity",
        };

        [MenuItem("Tools/Hold the Hill/Alpha/Add Dev Keys and Camera Fit")]
        private static void Build()
        {
            // From the menu, the open scenes are about to be replaced: offer to save them first.
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            AddDevKeys();
            foreach (string path in LevelGameplayScenes)
            {
                Scene level = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                Camera camera = AlphaRunFrameBuilder.FindInScene<Camera>(level);
                if (camera == null)
                {
                    Debug.LogError($"{path} has no camera to fit.");
                    continue;
                }

                EnsureCameraFit(camera);
                EditorSceneManager.MarkSceneDirty(level);
                EditorSceneManager.SaveScene(level);
            }

            ReportAfterReload();
            EditorSceneManager.OpenScene(BootstrapScenePath, OpenSceneMode.Single);
        }

        /// <summary>Entry point for a batch run: <c>-executeMethod HoldTheHill.EditorTools.AlphaPlaytestToolsBuilder.BuildFromCommandLine</c>.</summary>
        public static void BuildFromCommandLine()
        {
            Build();
        }

        /// <summary>Puts a <see cref="LevelCameraFit"/> on a level camera that has none. The level builder calls it too.</summary>
        internal static void EnsureCameraFit(Camera camera)
        {
            if (camera.GetComponent<LevelCameraFit>() == null)
            {
                camera.gameObject.AddComponent<LevelCameraFit>();
            }
        }

        private static void AddDevKeys()
        {
            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
            DevKeys keys = AlphaRunFrameBuilder.FindInScene<DevKeys>(ui);
            if (keys == null)
            {
                var holder = new GameObject(DevKeysName);
                SceneManager.MoveGameObjectToScene(holder, ui);
                keys = holder.AddComponent<DevKeys>();
            }

            // Loaded after OpenScene: opening a scene unloads loose assets loaded before it.
            var centipede = AssetDatabase.LoadAssetAtPath<GameObject>(CentipedePrefabPath);
            if (centipede == null)
            {
                Debug.LogError($"No centipede prefab at {CentipedePrefabPath}; F5 will do nothing.");
            }

            var so = new SerializedObject(keys);
            so.FindProperty("_centipedePrefab").objectReferenceValue = centipede != null ? centipede.GetComponent<BasicEnemy>() : null;
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(ui);
            EditorSceneManager.SaveScene(ui);
        }

        // Opens the saved scenes again and says what they hold, so a batch log shows the truth.
        private static void ReportAfterReload()
        {
            Scene ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Single);
            DevKeys keys = AlphaRunFrameBuilder.FindInScene<DevKeys>(ui);
            Object centipede = keys != null ? new SerializedObject(keys).FindProperty("_centipedePrefab").objectReferenceValue : null;
            string report = $"Play-test tools: dev keys = {(keys != null ? "yes" : "MISSING")}, centipede = {(centipede != null ? centipede.name : "none")}";

            foreach (string path in LevelGameplayScenes)
            {
                Scene level = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                Camera camera = AlphaRunFrameBuilder.FindInScene<Camera>(level);
                bool fitted = camera != null && camera.GetComponent<LevelCameraFit>() != null;
                report += $", {level.name} camera fit = {(fitted ? "yes" : "MISSING")}";
            }

            Debug.Log(report);
        }
    }
}
