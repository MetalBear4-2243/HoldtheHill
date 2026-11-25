using HoldTheHill.Core.Save;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class SaveDataTests
    {
        [Test]
        public void RecordWin_AddsACompletedLevel()
        {
            SaveData data = SaveData.CreateNew();

            Assert.IsTrue(data.RecordWin("L00_TutorialHill", 0.6f));

            LevelRecord record = data.FindLevel("L00_TutorialHill");
            Assert.IsTrue(record.completed);
            Assert.AreEqual(0.6f, record.bestHillHealth, 1e-5f);
        }

        [Test]
        public void RecordWin_KeepsTheBestHillHealth()
        {
            SaveData data = SaveData.CreateNew();
            data.RecordWin("L01_Meadow", 0.8f);

            Assert.IsFalse(data.RecordWin("L01_Meadow", 0.3f), "A worse win changes nothing.");
            Assert.AreEqual(0.8f, data.FindLevel("L01_Meadow").bestHillHealth, 1e-5f);

            Assert.IsTrue(data.RecordWin("L01_Meadow", 0.9f));
            Assert.AreEqual(0.9f, data.FindLevel("L01_Meadow").bestHillHealth, 1e-5f);
            Assert.AreEqual(1, data.levels.Count);
        }

        [Test]
        public void RecordWin_ClampsHillHealth_AndIgnoresAMissingId()
        {
            SaveData data = SaveData.CreateNew();

            data.RecordWin("L01_Meadow", 3f);
            Assert.AreEqual(1f, data.FindLevel("L01_Meadow").bestHillHealth, 1e-5f);

            Assert.IsFalse(data.RecordWin(null, 0.5f));
            Assert.IsFalse(data.RecordWin("", 0.5f));
            Assert.AreEqual(1, data.levels.Count);
        }

        [Test]
        public void Upgrade_RefusesANewerVersion_AndLeavesItAlone()
        {
            var data = new SaveData { version = SaveMigrations.CurrentVersion + 1 };

            Assert.IsFalse(SaveMigrations.Upgrade(data));
            Assert.AreEqual(SaveMigrations.CurrentVersion + 1, data.version);
        }

        [Test]
        public void Upgrade_FillsAMissingLevelList()
        {
            var data = new SaveData { version = 0, levels = null };

            Assert.IsTrue(SaveMigrations.Upgrade(data));
            Assert.AreEqual(SaveMigrations.CurrentVersion, data.version);
            Assert.IsNotNull(data.levels);
        }
    }
}
