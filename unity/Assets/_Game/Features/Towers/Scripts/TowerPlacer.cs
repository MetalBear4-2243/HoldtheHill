using System;
using System.Collections.Generic;
using HoldTheHill.Core;
using HoldTheHill.Features.Economy;
using HoldTheHill.Features.Enemies;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// Places and selects towers. A tower is picked from the shop (<see cref="Select(TowerData)"/>)
    /// or with the number keys; a ghost then follows the mouse, snapped to the grid. Left-click
    /// builds it on a free cell if the player can afford it, right-click puts the ghost away.
    /// With no ghost, left-click on a standing tower selects it for the tower card,
    /// <see cref="UpgradeSelectedTower"/> buys its next level and <see cref="ToggleBurrowAll"/>
    /// is the B key. A tower a hazard removes frees its cell through its Removed event.
    /// </summary>
    public class TowerPlacer : MonoBehaviour
    {
        /// <summary>A tower the player can buy: the shop entry and the prefab it builds.</summary>
        [Serializable]
        public struct Offer
        {
            [Tooltip("The shop card's data; its Honeydew cost is what the player pays.")]
            public TowerData Data;

            [Tooltip("The tower prefab built for that card.")]
            public Tower Prefab;
        }

        [Header("Grid")]
        [Tooltip("Size of one grid cell in world units.")]
        [SerializeField] private float _cellSize = 1f;

        [Header("Level")]
        [Tooltip("Towers can't be built on this path. Found automatically if left empty.")]
        [SerializeField] private EnemyPath _path;

        [Tooltip("The wallet towers are paid from.")]
        [SerializeField] private HoneydewWallet _wallet;

        [Header("Towers")]
        [Tooltip("Towers for sale, in number-key order (1 is the first).")]
        [SerializeField] private List<Offer> _offers = new List<Offer>();

        [Header("Input")]
        [Tooltip("The game's actions asset. Uses its Gameplay map: Point, Place, Cancel, SelectTower1-6.")]
        [SerializeField] private InputActionAsset _actions;

        [Header("Preview")]
        [SerializeField] private Color _freeCellColor = new Color(1f, 1f, 1f, 0.35f);
        [SerializeField] private Color _blockedCellColor = new Color(1f, 0.3f, 0.3f, 0.35f);
        [SerializeField] private Color _unaffordableColor = new Color(1f, 0.8f, 0.2f, 0.35f);
        [SerializeField] private Color _selectionColor = new Color(0.4f, 0.9f, 1f, 0.45f);

        private const int SelectKeys = 6;

        // Which grid cell holds which tower.
        private readonly Dictionary<Vector2Int, Tower> _towers = new Dictionary<Vector2Int, Tower>();
        private readonly InputAction[] _selectActions = new InputAction[SelectKeys];

        private Camera _camera;
        private SpriteRenderer _preview;
        private SpriteRenderer _selectionMark;
        private InputActionMap _gameplay;
        private InputAction _point;
        private InputAction _place;
        private InputAction _cancel;
        private int _selected = -1;
        private Tower _selectedTower;

        /// <summary>Raised when a tower has been built and paid for.</summary>
        public event Action<Tower> TowerPlaced;

        /// <summary>Raised when the selected tower changes; null means nothing is selected.</summary>
        public event Action<Tower> SelectedTowerChanged;

        /// <summary>The offer the ghost shows, or null when nothing is selected.</summary>
        public Offer? Selected => _selected >= 0 ? _offers[_selected] : (Offer?)null;

        /// <summary>The standing tower the card shows, or null.</summary>
        public Tower SelectedTower => _selectedTower;

        /// <summary>The wallet towers and upgrades are paid from.</summary>
        public HoneydewWallet Wallet => _wallet;

        /// <summary>Towers currently standing, by grid cell.</summary>
        public IReadOnlyDictionary<Vector2Int, Tower> Towers => _towers;

        private void Awake()
        {
            _camera = Camera.main;
            if (_path == null)
            {
                _path = FindAnyObjectByType<EnemyPath>();
            }

            _preview = MakeMarker("Placement Preview", 100);
            _selectionMark = MakeMarker("Selection Mark", 99);
            _selectionMark.color = _selectionColor;

            if (_actions != null)
            {
                _gameplay = _actions.FindActionMap("Gameplay", throwIfNotFound: true);
                _point = _gameplay.FindAction("Point", throwIfNotFound: true);
                _place = _gameplay.FindAction("Place", throwIfNotFound: true);
                _cancel = _gameplay.FindAction("Cancel", throwIfNotFound: true);
                for (int i = 0; i < SelectKeys; i++)
                {
                    _selectActions[i] = _gameplay.FindAction($"SelectTower{i + 1}", throwIfNotFound: true);
                }
            }
            else
            {
                Debug.LogError("TowerPlacer has no actions asset, so nothing can be placed.", this);
            }
        }

        private SpriteRenderer MakeMarker(string name, int sortingOrder)
        {
            var marker = new GameObject(name).AddComponent<SpriteRenderer>();
            marker.sprite = SpriteUtil.Square;
            marker.sortingOrder = sortingOrder;
            marker.transform.localScale = Vector3.one * _cellSize;
            marker.enabled = false;
            return marker;
        }

        private void OnEnable()
        {
            if (_gameplay != null)
            {
                _gameplay.Enable();
            }
        }

        private void OnDisable()
        {
            if (_gameplay != null)
            {
                _gameplay.Disable();
            }
        }

        /// <summary>Starts placing the tower sold by <paramref name="data"/>. The shop card calls this.</summary>
        public void Select(TowerData data)
        {
            for (int i = 0; i < _offers.Count; i++)
            {
                if (_offers[i].Data == data)
                {
                    Select(i);
                    return;
                }
            }

            Debug.LogWarning($"No tower is on offer for '{(data != null ? data.towerName : "null")}'.", this);
        }

        /// <summary>Starts placing offer <paramref name="index"/>; an index out of range cancels.</summary>
        public void Select(int index)
        {
            bool valid = index >= 0 && index < _offers.Count && _offers[index].Prefab != null;
            _selected = valid ? index : -1;
            _preview.enabled = valid;
            if (valid)
            {
                SelectTower(null);
            }
        }

        /// <summary>Puts the ghost away without building anything, and drops the tower selection.</summary>
        public void Cancel()
        {
            Select(-1);
            SelectTower(null);
        }

        /// <summary>Makes <paramref name="tower"/> the one the card shows; null clears it.</summary>
        public void SelectTower(Tower tower)
        {
            if (_selectedTower == tower)
            {
                return;
            }

            _selectedTower = tower;
            _selectionMark.enabled = tower != null;
            if (tower != null)
            {
                _selectionMark.transform.position = CellToWorld(tower.Cell);
            }

            SelectedTowerChanged?.Invoke(tower);
        }

        /// <summary>
        /// Buys the selected tower's next level if it has one and the wallet covers it.
        /// The card and the U key both come here.
        /// </summary>
        public TowerUpgradeRule.Verdict UpgradeSelectedTower()
        {
            if (_selectedTower == null)
            {
                return TowerUpgradeRule.Verdict.AtMaxLevel;
            }

            int honeydew = _wallet != null ? _wallet.Honeydew.Amount : int.MaxValue;
            TowerUpgradeRule.Verdict verdict = TowerUpgradeRule.Check(
                _selectedTower.Level, _selectedTower.MaxLevel, _selectedTower.NextUpgradeCost, honeydew);
            if (verdict != TowerUpgradeRule.Verdict.Ready)
            {
                return verdict;
            }

            if (_wallet == null || _wallet.Honeydew.TrySpend(_selectedTower.NextUpgradeCost))
            {
                _selectedTower.Upgrade();
            }

            return verdict;
        }

        /// <summary>
        /// Burrows every tower, or brings every tower up when all of them are already down.
        /// The B key comes here.
        /// </summary>
        public void ToggleBurrowAll()
        {
            int burrowed = 0;
            foreach (Tower tower in _towers.Values)
            {
                if (tower.Burrowed)
                {
                    burrowed++;
                }
            }

            bool burrow = TowerCondition.BurrowAllNext(burrowed, _towers.Count);
            foreach (Tower tower in _towers.Values)
            {
                tower.Burrowed = burrow;
            }
        }

        private void HandleTowerRemoved(Tower tower)
        {
            tower.Removed -= HandleTowerRemoved;
            _towers.Remove(tower.Cell);
            if (_selectedTower == tower)
            {
                SelectTower(null);
            }
        }

        private void Update()
        {
            if (_gameplay == null || _camera == null)
            {
                return;
            }

            for (int i = 0; i < SelectKeys; i++)
            {
                if (_selectActions[i].WasPressedThisFrame())
                {
                    Select(i);
                }
            }

            if (_cancel.WasPressedThisFrame())
            {
                Cancel();
            }

            // A click on the HUD (the shop, a button, the card) is not a click on the map.
            bool overUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            bool clicked = _place.WasPressedThisFrame() && !overUi;
            Vector3 mouseWorld = _camera.ScreenToWorldPoint(_point.ReadValue<Vector2>());
            Vector2Int cell = WorldToCell(mouseWorld);

            if (_selected < 0)
            {
                if (clicked)
                {
                    SelectTower(_towers.TryGetValue(cell, out Tower hit) ? hit : null);
                }
                return;
            }

            Offer offer = _offers[_selected];
            int cost = offer.Data != null ? offer.Data.honeydewCost : 0;
            bool blocked = _towers.ContainsKey(cell) || IsOnPath(cell);
            bool affordable = _wallet == null || _wallet.Honeydew.CanAfford(cost);

            _preview.transform.position = CellToWorld(cell);
            _preview.color = blocked ? _blockedCellColor : affordable ? _freeCellColor : _unaffordableColor;

            if (clicked && !blocked && affordable)
            {
                if (_wallet == null || _wallet.Honeydew.TrySpend(cost))
                {
                    PlaceTower(offer, cell);
                }
            }
        }

        private bool IsOnPath(Vector2Int cell)
        {
            return _path != null && _path.Blocks(CellToWorld(cell), _cellSize);
        }

        private void PlaceTower(Offer offer, Vector2Int cell)
        {
            Tower tower = Instantiate(offer.Prefab, CellToWorld(cell), Quaternion.identity, transform);
            tower.Cell = cell;
            tower.ShopData = offer.Data;
            tower.Removed += HandleTowerRemoved;
            _towers.Add(cell, tower);
            TowerPlaced?.Invoke(tower);
            SelectTower(tower);
        }

        private Vector2Int WorldToCell(Vector3 world)
        {
            return new Vector2Int(Mathf.FloorToInt(world.x / _cellSize), Mathf.FloorToInt(world.y / _cellSize));
        }

        private Vector3 CellToWorld(Vector2Int cell)
        {
            return new Vector3((cell.x + 0.5f) * _cellSize, (cell.y + 0.5f) * _cellSize, 0f);
        }
    }
}
