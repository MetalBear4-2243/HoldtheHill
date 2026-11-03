using System.Collections.Generic;

namespace HoldTheHill.Core
{
    /// <summary>One level the player can pick: its id (the scene pair's name) and its title.</summary>
    public readonly struct LevelInfo
    {
        public LevelInfo(string id, string title, bool showsControlsHelp = false)
        {
            Id = id;
            Title = title;
            ShowsControlsHelp = showsControlsHelp;
        }

        /// <summary>The scene pair's name, see <see cref="LevelScenes"/>.</summary>
        public string Id { get; }

        /// <summary>What the level select shows.</summary>
        public string Title { get; }

        /// <summary>
        /// The controls help box opens with the level. Only on the tutorial: on later maps it would
        /// cover the road, and F1 still brings it up.
        /// </summary>
        public bool ShowsControlsHelp { get; }
    }

    /// <summary>The levels the home screen offers, in order. The Alpha has two.</summary>
    public static class LevelCatalogue
    {
        public static readonly IReadOnlyList<LevelInfo> All = new[]
        {
            new LevelInfo("L00_TutorialHill", "Tutorial Hill", showsControlsHelp: true),
            new LevelInfo("L01_Meadow", "Meadow"),
        };

        /// <summary>Finds a level by id. False for an id that is not on the list.</summary>
        public static bool TryFind(string levelId, out LevelInfo level)
        {
            foreach (LevelInfo candidate in All)
            {
                if (candidate.Id == levelId)
                {
                    level = candidate;
                    return true;
                }
            }

            level = default;
            return false;
        }
    }
}
