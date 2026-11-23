using System;
using System.Collections.Generic;

namespace HoldTheHill.Core.Save
{
    /// <summary>
    /// Everything the game keeps between sessions, as written to the save file by
    /// <see cref="SaveFile"/>. Plain fields so <c>JsonUtility</c> can read and write it.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        /// <summary>
        /// The layout this data follows; <see cref="SaveMigrations"/> upgrades older ones. Left at 0
        /// here so a file with no version field reads as 0; <see cref="CreateNew"/> sets the current one.
        /// </summary>
        public int version;

        /// <summary>Volume sliders.</summary>
        public AudioSettingsData audio = new AudioSettingsData();

        /// <summary>One entry per level the player has finished at least once.</summary>
        public List<LevelRecord> levels = new List<LevelRecord>();

        /// <summary>Fresh data for a new player, at the current layout version.</summary>
        public static SaveData CreateNew() => new SaveData { version = SaveMigrations.CurrentVersion };

        /// <summary>The record for a level, or null if it has none yet.</summary>
        public LevelRecord FindLevel(string levelId)
        {
            foreach (LevelRecord record in levels)
            {
                if (record.id == levelId)
                {
                    return record;
                }
            }

            return null;
        }

        /// <summary>
        /// Marks a level as won and keeps the best share of hill health left (0..1).
        /// Returns true when anything changed, so the caller knows to save.
        /// </summary>
        public bool RecordWin(string levelId, float hillHealthLeft)
        {
            if (string.IsNullOrEmpty(levelId))
            {
                return false;
            }

            float clamped = Math.Max(0f, Math.Min(1f, hillHealthLeft));
            LevelRecord record = FindLevel(levelId);
            if (record == null)
            {
                levels.Add(new LevelRecord { id = levelId, completed = true, bestHillHealth = clamped });
                return true;
            }

            bool changed = !record.completed || clamped > record.bestHillHealth;
            record.completed = true;
            record.bestHillHealth = Math.Max(record.bestHillHealth, clamped);
            return changed;
        }
    }

    /// <summary>Volume sliders, linear 0..1.</summary>
    [Serializable]
    public class AudioSettingsData
    {
        /// <summary>
        /// False until the player moves a slider; until then the mixer keeps its authored levels.
        /// </summary>
        public bool set;

        public float master = 1f;
        public float sfx = 1f;
        public float music = 0.5f;
    }

    /// <summary>How the player has done on one level.</summary>
    [Serializable]
    public class LevelRecord
    {
        /// <summary>The level's id from the LevelCatalogue, e.g. L00_TutorialHill.</summary>
        public string id;

        /// <summary>Won at least once.</summary>
        public bool completed;

        /// <summary>The most hill health ever left at a win, 0..1.</summary>
        public float bestHillHealth;
    }
}
