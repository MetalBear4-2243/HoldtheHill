using System;
using System.IO;
using UnityEngine;

namespace HoldTheHill.Core.Save
{
    /// <summary>
    /// Reads and writes one save file as JSON. Writes go to a temporary file that then replaces
    /// the old one (which is kept as <c>.bak</c>), so a crash mid-write never leaves half a file.
    /// A missing file starts fresh; an unreadable one is set aside as <c>.corrupt.json</c> and
    /// starts fresh; one from a newer build is loaded read-only so this build never overwrites it.
    /// </summary>
    public class SaveFile
    {
        private readonly string _path;

        /// <param name="path">Full path of the save file, e.g. persistentDataPath/save.json.</param>
        public SaveFile(string path)
        {
            _path = path;
            Data = SaveData.CreateNew();
        }

        /// <summary>Where the file lives.</summary>
        public string Path => _path;

        /// <summary>The data in memory. Change it, then call <see cref="Save"/>.</summary>
        public SaveData Data { get; private set; }

        /// <summary>
        /// True when the file on disk came from a newer build: <see cref="Save"/> does nothing,
        /// so playing an older build can't wipe newer progress.
        /// </summary>
        public bool ReadOnly { get; private set; }

        /// <summary>Where an unreadable file is moved to before starting fresh.</summary>
        public string CorruptPath => System.IO.Path.ChangeExtension(_path, ".corrupt.json");

        /// <summary>Where the previous good file is kept after each save.</summary>
        public string BackupPath => _path + ".bak";

        private string TempPath => _path + ".tmp";

        /// <summary>Reads the file. Never throws: every failure falls back to fresh data and logs why.</summary>
        public void Load()
        {
            ReadOnly = false;
            Data = SaveData.CreateNew();

            if (!File.Exists(_path))
            {
                return;
            }

            string json;
            try
            {
                json = File.ReadAllText(_path);
            }
            catch (Exception e)
            {
                // Can't even read it (locked, no permission): don't overwrite what we couldn't see.
                ReadOnly = true;
                Debug.LogWarning($"Save file {_path} could not be read, so progress won't be saved this session: {e.Message}");
                return;
            }

            SaveData loaded = null;
            try
            {
                loaded = JsonUtility.FromJson<SaveData>(json);
            }
            catch (Exception)
            {
                // Handled below with the empty-file case.
            }

            if (loaded == null)
            {
                SetAsideCorrupt();
                return;
            }

            if (!SaveMigrations.Upgrade(loaded))
            {
                ReadOnly = true;
                Debug.LogWarning($"Save file {_path} is version {loaded.version}, newer than this build reads ({SaveMigrations.CurrentVersion}). Playing without saving so it isn't overwritten.");
                return;
            }

            Data = loaded;
        }

        /// <summary>Writes <see cref="Data"/> to disk. Returns false if it was not written.</summary>
        public bool Save()
        {
            if (ReadOnly)
            {
                return false;
            }

            try
            {
                string folder = System.IO.Path.GetDirectoryName(_path);
                if (!string.IsNullOrEmpty(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                File.WriteAllText(TempPath, JsonUtility.ToJson(Data, prettyPrint: true));
                if (File.Exists(_path))
                {
                    File.Replace(TempPath, _path, BackupPath);
                }
                else
                {
                    File.Move(TempPath, _path);
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Save file {_path} could not be written: {e.Message}");
                return false;
            }
        }

        /// <summary>Throws away all progress and settings, on disk too.</summary>
        public void ResetToNew()
        {
            Data = SaveData.CreateNew();
            ReadOnly = false;
            Save();
        }

        private void SetAsideCorrupt()
        {
            try
            {
                File.Copy(_path, CorruptPath, overwrite: true);
                Debug.LogWarning($"Save file {_path} could not be understood. A copy is at {CorruptPath}; starting fresh.");
            }
            catch (Exception e)
            {
                // Keep the bad file where it is rather than overwrite it.
                ReadOnly = true;
                Debug.LogWarning($"Save file {_path} could not be understood or copied aside ({e.Message}). Playing without saving.");
            }
        }
    }
}
