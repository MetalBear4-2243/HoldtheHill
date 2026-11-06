using System.Linq;
using HoldTheHill.Features.Towers;
using HoldTheHill.Features.UI;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// Checks the UI scene, not code: the HUD's top-left Honeydew line is gone (removed 2026-11-05),
    /// and the run's Honeydew still shows in the balance box of Adam's shop list.
    /// </summary>
    public class HudHoneydewDataTests
    {
        private const string UiScenePath = "Assets/_Game/Scenes/UI.unity";

        private Scene _ui;
        private bool _openedHere;

        [OneTimeSetUp]
        public void OpenUiScene()
        {
            // Additive, and closed again only if this fixture opened it, so a scene someone has open stays open.
            _ui = SceneManager.GetSceneByPath(UiScenePath);
            if (!_ui.isLoaded)
            {
                _ui = EditorSceneManager.OpenScene(UiScenePath, OpenSceneMode.Additive);
                _openedHere = true;
            }
        }

        [OneTimeTearDown]
        public void CloseUiScene()
        {
            if (_openedHere && _ui.IsValid())
            {
                EditorSceneManager.CloseScene(_ui, removeScene: true);
            }
        }

        [Test]
        public void TheHud_HasNoTopLeftHoneydewLine()
        {
            HoneydewLabel[] onTheHud = Labels().Where(l => l.transform.parent != null && l.transform.parent.name == "HUD").ToArray();
            Assert.That(onTheHud, Is.Empty, "The HUD should have no Honeydew line of its own; the shop list's balance box shows it.");
        }

        [Test]
        public void TheShopBalanceBox_IsTheOnlyHoneydewDisplay()
        {
            HoneydewLabel[] labels = Labels();
            Assert.That(labels.Length, Is.EqualTo(1), "Exactly one HoneydewLabel should be in the UI scene.");
            Assert.That(labels[0].name, Is.EqualTo("BalanceAmount"));
            Assert.That(labels[0].GetComponentInParent<TowerShopManager>(true), Is.Not.Null,
                "The Honeydew label should sit inside Adam's shop list.");
        }

        private HoneydewLabel[] Labels()
        {
            Assert.That(_ui.IsValid() && _ui.isLoaded, Is.True, $"{UiScenePath} did not open.");
            return _ui.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<HoneydewLabel>(true)).ToArray();
        }
    }
}
