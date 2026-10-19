using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoldTheHill.Core
{
    /// <summary>
    /// Lives in the Bootstrap scene. Loads the persistent scenes (UI), then either the home screen or
    /// a level's scene pair additively, so pressing Play in ANY scene (Bootstrap, UI, Home, a Terrain
    /// or a Gameplay scene) starts the full game. Menus reach it through <see cref="Current"/>.
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public const string BootstrapSceneName = "Bootstrap";

        [Tooltip("Scenes that stay loaded for the whole game.")]
        [SerializeField] private string[] _persistentScenes = { "UI" };

        [Tooltip("The home screen scene, shown when no level is open.")]
        [SerializeField] private string _homeScene = "Home";

        private static SceneLoader s_current;

        private bool _busy;

        /// <summary>The loader in the Bootstrap scene, for menus that live in other scenes.</summary>
        public static SceneLoader Current => s_current;

        /// <summary>True while scenes are being swapped; calls made meanwhile are ignored.</summary>
        public bool Busy => _busy;

        /// <summary>The id of the level whose scenes are open, or null on the home screen.</summary>
        public string OpenLevelId
        {
            get
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    if (scene.isLoaded && LevelScenes.TryGetLevelId(scene.name, out string levelId))
                    {
                        return levelId;
                    }
                }
                return null;
            }
        }

        private void OnEnable()
        {
            s_current = this;
        }

        private void OnDisable()
        {
            if (s_current == this)
            {
                s_current = null;
            }
        }

        /// <summary>Leaves whatever is open (home screen or level) and opens the level's scene pair.</summary>
        public void LoadLevel(string levelId)
        {
            if (string.IsNullOrEmpty(levelId))
            {
                Debug.LogError("LoadLevel needs a level id.", this);
                return;
            }

            if (!_busy)
            {
                StartCoroutine(SwitchRoutine(levelId));
            }
        }

        /// <summary>Unloads the level that is open and loads the same one again.</summary>
        public void RestartLevel()
        {
            string levelId = OpenLevelId;
            if (levelId == null)
            {
                Debug.LogWarning("No level is open, so there is nothing to restart.", this);
                return;
            }

            LoadLevel(levelId);
        }

        /// <summary>Leaves the level for the home screen.</summary>
        public void GoHome()
        {
            if (!_busy)
            {
                StartCoroutine(SwitchRoutine(null));
            }
        }

        // Unloads every scene that is not Bootstrap or persistent, then loads the level pair, or the
        // home screen when levelId is null.
        private IEnumerator SwitchRoutine(string levelId)
        {
            _busy = true;

            // Last loaded goes first: a level's Gameplay scene before its Terrain, so the run never
            // plays on for a few frames on a map that has already gone.
            List<string> loaded = LoadedSwappableScenes();
            for (int i = loaded.Count - 1; i >= 0; i--)
            {
                yield return SceneManager.UnloadSceneAsync(loaded[i]);
            }

            if (levelId == null)
            {
                yield return LoadIfMissing(_homeScene);
                Activate(_homeScene);
            }
            else
            {
                yield return LoadLevelScenes(levelId);
            }

            _busy = false;
        }

        private static IEnumerator LoadLevelScenes(string levelId)
        {
            yield return LoadIfMissing(LevelScenes.Terrain(levelId));
            yield return LoadIfMissing(LevelScenes.Gameplay(levelId));
            Activate(LevelScenes.Gameplay(levelId));
        }

        // Domain reload is on, but clearing keeps Play mode safe if that ever changes.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            s_current = null;
        }

        // Runs after the first scene loads, in the Editor and in builds.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureBootstrapLoaded()
        {
            if (!IsLoaded(BootstrapSceneName))
            {
                SceneManager.LoadScene(BootstrapSceneName, LoadSceneMode.Additive);
            }
        }

        private IEnumerator Start()
        {
            foreach (string sceneName in _persistentScenes)
            {
                yield return LoadIfMissing(sceneName);
            }

            // Play pressed in one scene of a level (e.g. Hill01_Gameplay): open the rest of that level.
            string openLevel = OpenLevelId;
            if (openLevel != null)
            {
                yield return LoadLevelScenes(openLevel);
                yield break;
            }

            // Play pressed in Bootstrap, UI or Home: show the home screen. Any other scene (a sandbox
            // scene, say) is left alone, so a teammate can play their own scene with just the UI on top.
            if (LoadedSwappableScenes().Count == 0)
            {
                yield return LoadIfMissing(_homeScene);
                Activate(_homeScene);
            }
        }

        // Every loaded scene that is not Bootstrap or persistent: the home screen, level scenes,
        // or a scene someone pressed Play in.
        private List<string> LoadedSwappableScenes()
        {
            var keep = new HashSet<string>(_persistentScenes) { BootstrapSceneName };
            var result = new List<string>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && !keep.Contains(scene.name))
                {
                    result.Add(scene.name);
                }
            }
            return result;
        }

        private static IEnumerator LoadIfMissing(string sceneName)
        {
            if (!IsLoaded(sceneName))
            {
                yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
        }

        private static void Activate(string sceneName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene.isLoaded)
            {
                SceneManager.SetActiveScene(scene);
            }
        }

        private static bool IsLoaded(string sceneName)
        {
            return SceneManager.GetSceneByName(sceneName).isLoaded;
        }
    }
}
