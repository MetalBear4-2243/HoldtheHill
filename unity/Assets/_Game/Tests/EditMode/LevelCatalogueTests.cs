using System.Collections.Generic;
using HoldTheHill.Core;
using NUnit.Framework;

namespace HoldTheHill.Tests
{
    public class LevelCatalogueTests
    {
        [Test]
        public void HasTheTwoAlphaLevels_InOrder()
        {
            Assert.AreEqual(2, LevelCatalogue.All.Count);
            Assert.AreEqual("L00_TutorialHill", LevelCatalogue.All[0].Id);
            Assert.AreEqual("L01_Meadow", LevelCatalogue.All[1].Id);
        }

        [Test]
        public void EveryLevel_HasAUniqueIdAndATitle()
        {
            var ids = new HashSet<string>();
            foreach (LevelInfo level in LevelCatalogue.All)
            {
                Assert.IsTrue(ids.Add(level.Id), $"Duplicate level id '{level.Id}'.");
                Assert.IsFalse(string.IsNullOrWhiteSpace(level.Title), $"Level '{level.Id}' has no title.");
            }
        }

        [Test]
        public void EveryLevelId_RoundTripsThroughTheSceneNameRule()
        {
            foreach (LevelInfo level in LevelCatalogue.All)
            {
                Assert.IsTrue(LevelScenes.TryGetLevelId(LevelScenes.Gameplay(level.Id), out string fromGameplay));
                Assert.IsTrue(LevelScenes.TryGetLevelId(LevelScenes.Terrain(level.Id), out string fromTerrain));
                Assert.AreEqual(level.Id, fromGameplay);
                Assert.AreEqual(level.Id, fromTerrain);
            }
        }

        [Test]
        public void TryFind_FindsListedIds_AndRejectsOthers()
        {
            Assert.IsTrue(LevelCatalogue.TryFind("L01_Meadow", out LevelInfo meadow));
            Assert.AreEqual("Meadow", meadow.Title);

            Assert.IsFalse(LevelCatalogue.TryFind("Hill01", out LevelInfo missing));
            Assert.IsNull(missing.Id);
            Assert.IsFalse(LevelCatalogue.TryFind(null, out _));
        }
    }
}
