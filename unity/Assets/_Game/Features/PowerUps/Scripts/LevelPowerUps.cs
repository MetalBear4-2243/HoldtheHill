using System;
using System.Collections.Generic;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.Towers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace HoldTheHill.Features.PowerUps
{
    /// <summary>
    /// The run's power-ups, in the level's gameplay scene. Rolls a drop when an enemy dies (through
    /// <see cref="EnemyRegistry.EnemyDied"/>, so Jason's enemy class is untouched), lets a click on
    /// the map collect a drop into the one <see cref="PowerUpSlot"/>, and carries out the slot when
    /// the HUD asks: Swarm Frenzy speeds every tower up through <see cref="TowerTempo"/> for a while;
    /// Royal Gift waits for a click on a standing tower and upgrades it for free.
    /// </summary>
    [DisallowMultipleComponent]
    public class LevelPowerUps : MonoBehaviour
    {
        [Tooltip("Drop chance, lifetimes, what each power-up does, and how they look.")]
        [SerializeField] private PowerUpSettings _settings;

        [Tooltip("The drop that appears where an enemy died.")]
        [SerializeField] private PowerUpPickup _pickupPrefab;

        [Tooltip("The level's placer, to find the tower a Royal Gift is aimed at.")]
        [SerializeField] private TowerPlacer _placer;

        [Tooltip("The game's actions asset. Uses its Gameplay map: Point, Place, Cancel.")]
        [SerializeField] private InputActionAsset _actions;

        private readonly List<PowerUpPickup> _pickups = new List<PowerUpPickup>();
        private readonly PowerUpSlot _slot = new PowerUpSlot();
        private System.Random _random;
        private Camera _camera;
        private InputActionMap _gameplay;
        private InputAction _point;
        private InputAction _place;
        private InputAction _cancel;
        private float _frenzyLeft;

        /// <summary>Raised when the slot, the gift targeting or the frenzy state changes. The HUD listens.</summary>
        public event Action Changed;

        /// <summary>Raised when a Royal Gift was aimed at a tower already at its last level, so the HUD can say so.</summary>
        public event Action GiftRefused;

        /// <summary>The numbers and looks. The HUD reads icons from here.</summary>
        public PowerUpSettings Settings => _settings;

        /// <summary>The one slot.</summary>
        public PowerUpSlot Slot => _slot;

        /// <summary>True while a used Royal Gift waits for the player to click a tower.</summary>
        public bool PickingGiftTarget { get; private set; }

        /// <summary>Seconds of Swarm Frenzy left, or 0.</summary>
        public float FrenzySecondsLeft => _frenzyLeft;

        public bool FrenzyActive => _frenzyLeft > 0f;

        private void Awake()
        {
            _random = new System.Random();
            _camera = Camera.main;

            if (_actions == null)
            {
                Debug.LogError("PowerUps has no actions asset, so drops cannot be collected.", this);
                return;
            }

            _gameplay = _actions.FindActionMap("Gameplay", throwIfNotFound: true);
            _point = _gameplay.FindAction("Point", throwIfNotFound: true);
            _place = _gameplay.FindAction("Place", throwIfNotFound: true);
            _cancel = _gameplay.FindAction("Cancel", throwIfNotFound: true);
        }

        private void OnEnable()
        {
            EnemyRegistry.EnemyDied += HandleEnemyDied;
            if (_gameplay != null)
            {
                _gameplay.Enable();
            }
        }

        private void OnDisable()
        {
            EnemyRegistry.EnemyDied -= HandleEnemyDied;
            if (_gameplay != null)
            {
                _gameplay.Disable();
            }

            // The level is going away; towers must not stay fast into the next one.
            if (_frenzyLeft > 0f)
            {
                _frenzyLeft = 0f;
                TowerTempo.FireRateFactor = 1f;
            }
        }

        private void HandleEnemyDied(BasicEnemy enemy)
        {
            if (enemy == null || _settings == null || _pickupPrefab == null)
            {
                return;
            }

            if (!PowerUpRule.RollDrop(_random.NextDouble(), _settings.DropChance, out PowerUpKind kind))
            {
                return;
            }

            Vector3 where = enemy.transform.position;
            where.z = 0f;
            PowerUpPickup pickup = Instantiate(_pickupPrefab, where, Quaternion.identity, transform);
            pickup.Init(kind, _settings.IconFor(kind), _settings.ColorFor(kind), _settings.DropLifetime);
            _pickups.Add(pickup);
        }

        private void Update()
        {
            TickFrenzy();

            if (_gameplay == null || _camera == null)
            {
                return;
            }

            if (_cancel.WasPressedThisFrame() && PickingGiftTarget)
            {
                PickingGiftTarget = false;
                Changed?.Invoke();
            }

            // A click on the HUD (the slot itself, the card) is not a click on the map.
            bool overUi = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            if (!_place.WasPressedThisFrame() || overUi)
            {
                return;
            }

            Vector3 world = _camera.ScreenToWorldPoint(_point.ReadValue<Vector2>());
            world.z = 0f;

            if (PickingGiftTarget)
            {
                GiftTowerAt(world);
                return;
            }

            CollectAt(world);
        }

        private void TickFrenzy()
        {
            if (_frenzyLeft <= 0f)
            {
                return;
            }

            _frenzyLeft -= Time.deltaTime;
            if (_frenzyLeft <= 0f)
            {
                _frenzyLeft = 0f;
                TowerTempo.FireRateFactor = 1f;
                Changed?.Invoke();
            }
        }

        /// <summary>
        /// Collects the drop under <paramref name="world"/> into the slot. A full slot leaves the drop
        /// where it is. True if something was collected.
        /// </summary>
        public bool CollectAt(Vector3 world)
        {
            PowerUpPickup pickup = PickupAt(world);
            if (pickup == null || !_slot.TryStore(pickup.Kind))
            {
                return false;
            }

            _pickups.Remove(pickup);
            Destroy(pickup.gameObject);
            Changed?.Invoke();
            return true;
        }

        /// <summary>The first living drop a click at <paramref name="world"/> lands on, or null.</summary>
        public PowerUpPickup PickupAt(Vector3 world)
        {
            // Expired drops destroy themselves; drop their dead entries as they are met.
            for (int i = _pickups.Count - 1; i >= 0; i--)
            {
                if (_pickups[i] == null)
                {
                    _pickups.RemoveAt(i);
                }
                else if (_pickups[i].Covers(world))
                {
                    return _pickups[i];
                }
            }

            return null;
        }

        /// <summary>
        /// The HUD slot was clicked. Swarm Frenzy starts at once; Royal Gift starts (or stops) waiting
        /// for a tower to be clicked. Nothing happens with an empty slot.
        /// </summary>
        public void Use()
        {
            if (_slot.Held == null)
            {
                return;
            }

            if (_slot.Held == PowerUpKind.RoyalGift)
            {
                PickingGiftTarget = !PickingGiftTarget;
                Changed?.Invoke();
                return;
            }

            _slot.Take();
            StartFrenzy();
            Changed?.Invoke();
        }

        private void StartFrenzy()
        {
            if (_settings == null)
            {
                return;
            }

            // A second frenzy during the first restarts the clock rather than stacking the factor.
            _frenzyLeft = _settings.FrenzyDuration;
            TowerTempo.FireRateFactor = _settings.FrenzyFireRateFactor;
        }

        private void GiftTowerAt(Vector3 world)
        {
            Tower tower = _placer != null ? _placer.TowerAt(world) : null;
            if (tower == null)
            {
                // A click on empty ground keeps waiting; right-click (Cancel) gives up.
                return;
            }

            if (!PowerUpRule.CanGift(tower.Level, tower.MaxLevel))
            {
                GiftRefused?.Invoke();
                return;
            }

            tower.Upgrade();
            _slot.Take();
            PickingGiftTarget = false;
            Changed?.Invoke();
        }
    }
}
