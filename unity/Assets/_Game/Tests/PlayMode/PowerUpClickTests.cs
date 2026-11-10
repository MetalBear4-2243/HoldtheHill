#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using HoldTheHill.Core;
using HoldTheHill.Features.Economy;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.PowerUps;
using HoldTheHill.Features.Towers;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace HoldTheHill.Tests
{
    /// <summary>
    /// One left click on the map does one thing: a click that collects a power-up drop (or aims a
    /// Royal Gift at a tower) takes the click, so the tower placer neither builds, pays nor changes
    /// the selection.
    /// Not covered: real pointer input. The test asmdef has no Input System reference, so the click is
    /// driven through <see cref="LevelPowerUps.ClickAt"/>, and <see cref="PlacerClick"/> stands in for
    /// the gate in <c>TowerPlacer.Update</c> (it reads the same <see cref="SilverfishClickReader.ClickUsedThisFrame"/>).
    /// The execution order (power-ups before the placer) is checked by reading the attributes.
    /// Editor only: the game's input actions asset is loaded through the AssetDatabase.
    /// </summary>
    public class PowerUpClickTests
    {
        private const string ActionsPath = "Assets/_Game/Input/HoldTheHill.inputactions";
        private const int BuildCost = 100;
        private const int Start = 300;

        // Cell size 1: cell (2, 1) is centred on (2.5, 1.5).
        private static readonly Vector2Int FreeCell = new Vector2Int(2, 1);
        private static readonly Vector3 FreeCellCentre = new Vector3(2.5f, 1.5f, 0f);
        private static readonly Vector2Int TowerCell = new Vector2Int(4, 1);

        private readonly List<Object> _assets = new List<Object>();
        private Scene _scene;
        private TowerData _data;
        private TowerPlacer _placer;
        private LevelPowerUps _powerUps;
        private Honeydew _honeydew;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            _scene = SceneManager.CreateScene("PowerUpClickTests");
            SceneManager.SetActiveScene(_scene);
            yield return null;
            MapClick.Reset();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            for (int i = _assets.Count - 1; i >= 0; i--)
            {
                if (_assets[i] != null)
                {
                    Object.DestroyImmediate(_assets[i]);
                }
            }

            _assets.Clear();
            yield return SceneManager.UnloadSceneAsync(_scene);
        }

        // A wallet, a placer selling one tower (build 100) and the power-ups, wired as in a level.
        private void MakeLevel()
        {
            var walletObject = new GameObject("Wallet");
            walletObject.SetActive(false);
            HoneydewWallet wallet = walletObject.AddComponent<HoneydewWallet>();
            var walletSo = new SerializedObject(wallet);
            walletSo.FindProperty("_startingHoneydew").intValue = Start;
            walletSo.ApplyModifiedPropertiesWithoutUndo();
            walletObject.SetActive(true);
            _honeydew = wallet.Honeydew;

            // Templates under an inactive holder, so only their copies wake up.
            var holder = new GameObject("Templates");
            holder.SetActive(false);
            var towerObject = new GameObject("Test Tower");
            towerObject.transform.SetParent(holder.transform);
            Tower template = towerObject.AddComponent<Tower>();
            var pickupObject = new GameObject("Test Drop");
            pickupObject.transform.SetParent(holder.transform);
            PowerUpPickup pickupTemplate = pickupObject.AddComponent<PowerUpPickup>();

            _data = ScriptableObject.CreateInstance<TowerData>();
            _data.name = "Test Tower";
            _data.towerName = "Test Tower";
            _assets.Add(_data);
            var dataSo = new SerializedObject(_data);
            dataSo.FindProperty("_prefab").objectReferenceValue = template;
            SerializedProperty levels = dataSo.FindProperty("_levels");
            int[] costs = { BuildCost, 60, 90 };
            levels.arraySize = costs.Length;
            for (int i = 0; i < costs.Length; i++)
            {
                SerializedProperty level = levels.GetArrayElementAtIndex(i);
                level.FindPropertyRelative("Cost").intValue = costs[i];
                level.FindPropertyRelative("Damage").floatValue = 10f;
                level.FindPropertyRelative("AttackInterval").floatValue = 1f;
                level.FindPropertyRelative("Range").floatValue = 2.5f;
            }
            dataSo.ApplyModifiedPropertiesWithoutUndo();

            var templateSo = new SerializedObject(template);
            templateSo.FindProperty("_data").objectReferenceValue = _data;
            templateSo.ApplyModifiedPropertiesWithoutUndo();

            var actions = AssetDatabase.LoadMainAssetAtPath(ActionsPath);
            Assert.That(actions, Is.Not.Null, $"{ActionsPath} is missing.");

            var placerObject = new GameObject("Tower Placer");
            placerObject.SetActive(false);
            _placer = placerObject.AddComponent<TowerPlacer>();
            var placerSo = new SerializedObject(_placer);
            placerSo.FindProperty("_wallet").objectReferenceValue = wallet;
            placerSo.FindProperty("_actions").objectReferenceValue = actions;
            SerializedProperty definitions = placerSo.FindProperty("_definitions");
            definitions.arraySize = 1;
            definitions.GetArrayElementAtIndex(0).objectReferenceValue = _data;
            placerSo.ApplyModifiedPropertiesWithoutUndo();
            placerObject.SetActive(true);

            PowerUpSettings settings = ScriptableObject.CreateInstance<PowerUpSettings>();
            settings.DropChance = 0f; // drops only where the test puts them
            _assets.Add(settings);

            var powerUpsObject = new GameObject("Power-Ups");
            powerUpsObject.SetActive(false);
            _powerUps = powerUpsObject.AddComponent<LevelPowerUps>();
            var powerUpsSo = new SerializedObject(_powerUps);
            powerUpsSo.FindProperty("_settings").objectReferenceValue = settings;
            powerUpsSo.FindProperty("_pickupPrefab").objectReferenceValue = pickupTemplate;
            powerUpsSo.FindProperty("_placer").objectReferenceValue = _placer;
            powerUpsSo.FindProperty("_actions").objectReferenceValue = actions;
            powerUpsSo.ApplyModifiedPropertiesWithoutUndo();
            powerUpsObject.SetActive(true);
        }

        // What TowerPlacer.Update does with the same press after the power-ups have run: nothing if
        // the click is taken; otherwise a standing tower is selected, and with a tower in hand a free
        // cell is built on.
        private void PlacerClick(Vector3 world, Vector2Int cell)
        {
            if (SilverfishClickReader.ClickUsedThisFrame)
            {
                return;
            }

            Tower standing = _placer.TowerAt(world);
            if (standing != null)
            {
                _placer.Select(-1);
                _placer.SelectTower(standing);
            }
            else if (_placer.Selected.HasValue)
            {
                _placer.TryBuildSelected(cell);
            }
            else
            {
                _placer.SelectTower(null);
            }
        }

        private Tower BuildAt(Vector2Int cell)
        {
            _placer.Select(0);
            Assert.AreEqual(TowerPlacementRule.Verdict.Ready, _placer.TryBuildSelected(cell));
            return _placer.Towers[cell];
        }

        [Test]
        public void ClickOnADrop_WithATowerInHandOnAFreeCell_CollectsAndBuildsNothing()
        {
            MakeLevel();
            _placer.Select(0);
            _powerUps.Drop(PowerUpKind.SwarmFrenzy, FreeCellCentre);
            Assert.AreEqual(TowerPlacementRule.Verdict.Ready, _placer.CheckPlacement(FreeCell), "the cell under the drop is buildable");

            Assert.IsTrue(_powerUps.ClickAt(FreeCellCentre), "the power-ups take the click");
            PlacerClick(FreeCellCentre, FreeCell);

            Assert.AreEqual(PowerUpKind.SwarmFrenzy, _powerUps.Slot.Held, "the drop is collected");
            Assert.IsNull(_powerUps.PickupAt(FreeCellCentre));
            Assert.AreEqual(Start, _honeydew.Amount, "no Honeydew spent");
            Assert.AreEqual(0, _placer.Towers.Count, "nothing built");
            Assert.AreEqual(_data, _placer.Selected.Value.Data, "the tower stays in hand");
        }

        [Test]
        public void ClickOnADrop_WithATowerCardOpen_LeavesTheCardOpen()
        {
            MakeLevel();
            Tower tower = BuildAt(TowerCell);
            _placer.Select(-1);
            _placer.SelectTower(tower);
            int afterBuild = _honeydew.Amount;
            _powerUps.Drop(PowerUpKind.RoyalGift, FreeCellCentre);

            Assert.IsTrue(_powerUps.ClickAt(FreeCellCentre));
            PlacerClick(FreeCellCentre, FreeCell);

            Assert.AreEqual(PowerUpKind.RoyalGift, _powerUps.Slot.Held);
            Assert.AreSame(tower, _placer.SelectedTower, "the card stays on the selected tower");
            Assert.AreEqual(afterBuild, _honeydew.Amount);
        }

        [Test]
        public void ClickOnADrop_WithTheSlotFull_IsStillAUsedClick()
        {
            MakeLevel();
            _placer.Select(0);
            Assert.IsTrue(_powerUps.Slot.TryStore(PowerUpKind.RoyalGift));
            _powerUps.Drop(PowerUpKind.SwarmFrenzy, FreeCellCentre);
            int slotFull = 0;
            _powerUps.SlotFull += () => slotFull++;

            Assert.IsTrue(_powerUps.ClickAt(FreeCellCentre));
            PlacerClick(FreeCellCentre, FreeCell);

            Assert.AreEqual(1, slotFull);
            Assert.IsNotNull(_powerUps.PickupAt(FreeCellCentre), "the drop stays on the map");
            Assert.AreEqual(PowerUpKind.RoyalGift, _powerUps.Slot.Held);
            Assert.AreEqual(Start, _honeydew.Amount);
            Assert.AreEqual(0, _placer.Towers.Count);
        }

        [Test]
        public void ClickOnEmptyGround_IsNotTaken_SoThePlacerBuilds()
        {
            MakeLevel();
            _placer.Select(0);

            Assert.IsFalse(_powerUps.ClickAt(FreeCellCentre));
            Assert.IsFalse(MapClick.TakenThisFrame);
            PlacerClick(FreeCellCentre, FreeCell);

            Assert.AreEqual(Start - BuildCost, _honeydew.Amount);
            Assert.AreEqual(1, _placer.Towers.Count);
        }

        [Test]
        public void RoyalGiftOnATower_UpgradesItAndLeavesTheSelectionAlone()
        {
            MakeLevel();
            Tower tower = BuildAt(TowerCell);
            _placer.Select(-1);
            int afterBuild = _honeydew.Amount;
            Assert.IsTrue(_powerUps.Slot.TryStore(PowerUpKind.RoyalGift));
            _powerUps.Use();
            Assert.IsTrue(_powerUps.PickingGiftTarget);
            Vector3 towerWorld = tower.transform.position;

            Assert.IsTrue(_powerUps.ClickAt(towerWorld));
            PlacerClick(towerWorld, TowerCell);

            Assert.AreEqual(2, tower.Level, "gifted");
            Assert.IsNull(_powerUps.Slot.Held);
            Assert.IsNull(_placer.SelectedTower, "the gift click does not also open the tower's card");
            Assert.AreEqual(afterBuild, _honeydew.Amount, "the gift is free");
        }

        [Test]
        public void ClaimersRunBeforeThePlacer()
        {
            int silverfish = OrderOf(typeof(SilverfishClickReader));
            int powerUps = OrderOf(typeof(LevelPowerUps));
            int placer = OrderOf(typeof(TowerPlacer));
            Assert.Less(silverfish, powerUps);
            Assert.Less(powerUps, placer);
        }

        private static int OrderOf(System.Type type)
        {
            var order = (DefaultExecutionOrder)System.Attribute.GetCustomAttribute(type, typeof(DefaultExecutionOrder));
            return order != null ? order.order : 0;
        }
    }
}
#endif
