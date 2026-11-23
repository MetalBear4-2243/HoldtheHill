using System.IO;
using HoldTheHill.Core.Save;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HoldTheHill.Tests
{
    public class SaveFileTests
    {
        private string _folder;
        private string _path;

        [SetUp]
        public void SetUp()
        {
            _folder = Path.Combine(Path.GetTempPath(), "hth-save-tests-" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_folder);
            _path = Path.Combine(_folder, "save.json");
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_folder))
            {
                Directory.Delete(_folder, recursive: true);
            }
        }

        [Test]
        public void MissingFile_StartsFresh_AtCurrentVersion()
        {
            var save = new SaveFile(_path);
            save.Load();

            Assert.AreEqual(SaveMigrations.CurrentVersion, save.Data.version);
            Assert.IsEmpty(save.Data.levels);
            Assert.IsFalse(save.ReadOnly);
        }

        [Test]
        public void SaveThenLoad_RoundTripsLevels()
        {
            var save = new SaveFile(_path);
            save.Load();
            save.Data.RecordWin("L01_Meadow", 0.75f);
            Assert.IsTrue(save.Save());

            var again = new SaveFile(_path);
            again.Load();

            Assert.AreEqual(SaveMigrations.CurrentVersion, again.Data.version);
            LevelRecord meadow = again.Data.FindLevel("L01_Meadow");
            Assert.IsNotNull(meadow);
            Assert.IsTrue(meadow.completed);
            Assert.AreEqual(0.75f, meadow.bestHillHealth, 1e-5f);
        }

        [Test]
        public void SecondSave_KeepsThePreviousFileAsBackup()
        {
            var save = new SaveFile(_path);
            save.Load();
            save.Data.RecordWin("L00_TutorialHill", 0.5f);
            save.Save();
            save.Data.RecordWin("L01_Meadow", 0.5f);
            save.Save();

            Assert.IsTrue(File.Exists(save.BackupPath));
            StringAssert.Contains("L00_TutorialHill", File.ReadAllText(save.BackupPath));
            StringAssert.DoesNotContain("L01_Meadow", File.ReadAllText(save.BackupPath));
            Assert.IsFalse(File.Exists(_path + ".tmp"));
        }

        [Test]
        public void CorruptFile_IsSetAside_AndPlayStartsFresh()
        {
            File.WriteAllText(_path, "{ this is not json");
            var save = new SaveFile(_path);

            LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("could not be understood"));
            save.Load();

            Assert.IsEmpty(save.Data.levels);
            Assert.IsFalse(save.ReadOnly);
            Assert.AreEqual("{ this is not json", File.ReadAllText(save.CorruptPath));

            // The next save replaces the bad file with good data.
            Assert.IsTrue(save.Save());
            var again = new SaveFile(_path);
            again.Load();
            Assert.AreEqual(SaveMigrations.CurrentVersion, again.Data.version);
        }

        [Test]
        public void EmptyFile_CountsAsCorrupt()
        {
            File.WriteAllText(_path, "");
            var save = new SaveFile(_path);

            LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("could not be understood"));
            save.Load();

            Assert.IsTrue(File.Exists(save.CorruptPath));
            Assert.IsFalse(save.ReadOnly);
        }

        [Test]
        public void FileWithNoVersion_IsUpgradedToCurrent_KeepingItsData()
        {
            File.WriteAllText(_path, "{\"levels\":[{\"id\":\"L00_TutorialHill\",\"completed\":true,\"bestHillHealth\":0.4}]}");
            var save = new SaveFile(_path);
            save.Load();

            Assert.AreEqual(SaveMigrations.CurrentVersion, save.Data.version);
            Assert.IsTrue(save.Data.FindLevel("L00_TutorialHill").completed);
            Assert.IsFalse(save.ReadOnly);
        }

        [Test]
        public void FileFromANewerBuild_IsReadOnly_AndNeverOverwritten()
        {
            string newer = "{\"version\":" + (SaveMigrations.CurrentVersion + 1) + ",\"levels\":[]}";
            File.WriteAllText(_path, newer);
            var save = new SaveFile(_path);

            LogAssert.Expect(LogType.Warning, new System.Text.RegularExpressions.Regex("newer than this build"));
            save.Load();

            Assert.IsTrue(save.ReadOnly);
            save.Data.RecordWin("L01_Meadow", 1f);
            Assert.IsFalse(save.Save());
            Assert.AreEqual(newer, File.ReadAllText(_path));
        }

        [Test]
        public void ResetToNew_ClearsProgressOnDisk()
        {
            var save = new SaveFile(_path);
            save.Load();
            save.Data.RecordWin("L01_Meadow", 1f);
            save.Save();

            save.ResetToNew();
            var again = new SaveFile(_path);
            again.Load();

            Assert.IsEmpty(again.Data.levels);
        }
    }
}
