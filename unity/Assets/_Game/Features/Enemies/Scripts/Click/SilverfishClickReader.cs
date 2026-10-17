using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// Kills a <see cref="SilverfishEnemy"/> with one click, read from the game's actions asset.
    /// Jason's class does this with <c>OnMouseDown</c>, which only the old Input Manager drives;
    /// this project runs the Input System alone, so one reader per level scene checks the
    /// <see cref="EnemyRegistry"/> under the pointer on the Place action and calls the same kill
    /// (all remaining health as damage). One of the two should go once Jason has picked.
    /// </summary>
    [DisallowMultipleComponent]
    public class SilverfishClickReader : MonoBehaviour
    {
        [Tooltip("The game's actions asset. Uses its Gameplay map: Point and Place.")]
        [SerializeField] private InputActionAsset _actions;

        [Tooltip("A silverfish with no collider counts as clicked within this distance of its centre, in world units.")]
        [SerializeField, Min(0.05f)] private float _fallbackRadius = 0.4f;

        private Camera _camera;
        private InputActionMap _gameplay;
        private InputAction _point;
        private InputAction _place;

        /// <summary>Raised after a click has killed a silverfish.</summary>
        public event System.Action<SilverfishEnemy> SilverfishClicked;

        private void Awake()
        {
            _camera = Camera.main;
            if (_actions == null)
            {
                Debug.LogError("SilverfishClickReader has no actions asset, so clicks do nothing.", this);
                return;
            }

            _gameplay = _actions.FindActionMap("Gameplay", throwIfNotFound: true);
            _point = _gameplay.FindAction("Point", throwIfNotFound: true);
            _place = _gameplay.FindAction("Place", throwIfNotFound: true);
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

        private void Update()
        {
            if (_gameplay == null || _camera == null || !_place.WasPressedThisFrame())
            {
                return;
            }

            // A click on the HUD is not a click on the map.
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Vector3 world = _camera.ScreenToWorldPoint(_point.ReadValue<Vector2>());
            world.z = 0f;
            SilverfishEnemy hit = FindSilverfishAt(world);
            if (hit == null)
            {
                return;
            }

            // The same kill Jason's OnMouseDown does.
            hit.TakeDamage(hit.CurrentHealth);
            SilverfishClicked?.Invoke(hit);
        }

        /// <summary>The first living silverfish under <paramref name="world"/>, or null.</summary>
        public SilverfishEnemy FindSilverfishAt(Vector3 world)
        {
            IReadOnlyList<BasicEnemy> enemies = EnemyRegistry.All;
            for (int i = 0; i < enemies.Count; i++)
            {
                if (!(enemies[i] is SilverfishEnemy silverfish) || silverfish.IsDead)
                {
                    continue;
                }

                if (Covers(silverfish, world))
                {
                    return silverfish;
                }
            }

            return null;
        }

        private bool Covers(SilverfishEnemy silverfish, Vector3 world)
        {
            Collider2D collider = silverfish.GetComponent<Collider2D>();
            if (collider != null)
            {
                return collider.OverlapPoint(world);
            }

            Vector3 centre = silverfish.transform.position;
            centre.z = 0f;
            return (centre - world).sqrMagnitude <= _fallbackRadius * _fallbackRadius;
        }
    }
}
