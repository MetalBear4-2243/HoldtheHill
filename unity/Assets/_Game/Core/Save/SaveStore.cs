using System.IO;
using UnityEngine;

namespace HoldTheHill.Core.Save
{
    /// <summary>
    /// The game's one save file, <c>persistentDataPath/save.json</c>, loaded the first time
    /// anything asks for it. Settings and progress code read <see cref="Data"/>, change it, and
    /// call <see cref="Save"/>. Static because the file is one per player, like PlayerPrefs; the
    /// reading and writing itself is in <see cref="SaveFile"/>, which the tests use directly.
    /// </summary>
    public static class SaveStore
    {
        public const string FileName = "save.json";

        private static SaveFile s_file;

        /// <summary>The save file, loaded on first use.</summary>
        public static SaveFile File
        {
            get
            {
                if (s_file == null)
                {
                    s_file = new SaveFile(Path.Combine(Application.persistentDataPath, FileName));
                    s_file.Load();
                }

                return s_file;
            }
        }

        /// <summary>The data in memory.</summary>
        public static SaveData Data => File.Data;

        /// <summary>Writes the data to disk.</summary>
        public static bool Save() => File.Save();

        // Domain reload is on, but clearing keeps Play mode safe if that ever changes, and makes
        // each Play read the file again.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            s_file = null;
        }
    }
}
