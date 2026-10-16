namespace HoldTheHill.Core
{
    /// <summary>
    /// The scene-name rule for levels: a level with id <c>L00_TutorialHill</c> is the scene pair
    /// <c>L00_TutorialHill_Terrain</c> + <c>L00_TutorialHill_Gameplay</c>. Plain C#, so the rule
    /// is tested in EditMode and the loader never guesses.
    /// </summary>
    public static class LevelScenes
    {
        public const string TerrainSuffix = "_Terrain";
        public const string GameplaySuffix = "_Gameplay";

        /// <summary>The tilemap scene of a level.</summary>
        public static string Terrain(string levelId) => levelId + TerrainSuffix;

        /// <summary>The camera, spawner and run scene of a level. It becomes the active scene.</summary>
        public static string Gameplay(string levelId) => levelId + GameplaySuffix;

        /// <summary>True for either scene of a level.</summary>
        public static bool IsLevelScene(string sceneName) => TryGetLevelId(sceneName, out _);

        /// <summary>
        /// Reads the level id back from one of its scene names. False for every other scene
        /// (Bootstrap, UI, Home, a sandbox scene) and for a bare suffix with no id in front.
        /// </summary>
        public static bool TryGetLevelId(string sceneName, out string levelId)
        {
            levelId = null;
            if (string.IsNullOrEmpty(sceneName))
            {
                return false;
            }

            if (EndsWith(sceneName, TerrainSuffix, out string id) || EndsWith(sceneName, GameplaySuffix, out id))
            {
                levelId = id;
                return true;
            }

            return false;
        }

        private static bool EndsWith(string sceneName, string suffix, out string head)
        {
            head = null;
            if (sceneName.Length <= suffix.Length || !sceneName.EndsWith(suffix, System.StringComparison.Ordinal))
            {
                return false;
            }

            head = sceneName.Substring(0, sceneName.Length - suffix.Length);
            return true;
        }
    }
}
