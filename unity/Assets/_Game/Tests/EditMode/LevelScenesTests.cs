using HoldTheHill.Core;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class LevelScenesTests
    {
        [Test]
        public void SceneNames_AreTheIdPlusASuffix()
        {
            Assert.AreEqual("L00_TutorialHill_Terrain", LevelScenes.Terrain("L00_TutorialHill"));
            Assert.AreEqual("L00_TutorialHill_Gameplay", LevelScenes.Gameplay("L00_TutorialHill"));
        }

        [Test]
        public void TryGetLevelId_ReadsTheIdBackFromEitherScene()
        {
            Assert.IsTrue(LevelScenes.TryGetLevelId("L01_Meadow_Terrain", out string fromTerrain));
            Assert.IsTrue(LevelScenes.TryGetLevelId("L01_Meadow_Gameplay", out string fromGameplay));
            Assert.AreEqual("L01_Meadow", fromTerrain);
            Assert.AreEqual("L01_Meadow", fromGameplay);
        }

        [Test]
        public void TryGetLevelId_StillFitsHill01()
        {
            Assert.IsTrue(LevelScenes.TryGetLevelId("Hill01_Gameplay", out string levelId));
            Assert.AreEqual("Hill01", levelId);
        }

        [TestCase("Bootstrap")]
        [TestCase("UI")]
        [TestCase("Home")]
        [TestCase("AdamShopTest")]
        [TestCase("_Terrain")]
        [TestCase("_Gameplay")]
        [TestCase("")]
        [TestCase(null)]
        public void TryGetLevelId_RejectsEveryOtherScene(string sceneName)
        {
            Assert.IsFalse(LevelScenes.TryGetLevelId(sceneName, out string levelId));
            Assert.IsNull(levelId);
            Assert.IsFalse(LevelScenes.IsLevelScene(sceneName));
        }

        [Test]
        public void TryGetLevelId_IsCaseSensitive()
        {
            Assert.IsFalse(LevelScenes.TryGetLevelId("L00_TutorialHill_terrain", out _));
        }
    }
}
