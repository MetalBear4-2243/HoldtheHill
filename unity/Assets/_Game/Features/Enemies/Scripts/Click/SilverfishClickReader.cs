using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// Kills a silverfish (an enemy whose <see cref="EnemyData.DiesToClick"/> is on) with one
    /// click, read from the game's actions asset.
    /// Jason's #43 took out his <c>OnMouseDown</c> in favour of this reader, so it is the only kill
    /// path: one reader per level scene checks the <see cref="EnemyRegistry"/> under the pointer on
    /// the Place action and deals all remaining health as damage. A click that kills is used up:
    /// <see cref="ClickUsedThisFrame"/> tells the placer and the power-ups to leave it alone.
    /// </summary>
    // Before the placer and the power-ups (default order 0), so they can see the click was used.
    [DefaultExecutionOrder(-10)]
    [DisallowMultipleComponent]
    public class SilverfishClickReader : MonoBehaviour
    {
        private static int s_usedFrame = -1;
        [Tooltip("The game's actions asset. Uses its Gameplay map: Point and Place.")]
        [SerializeField] private InputActionAsset _actions;

        [Tooltip("A silverfish with no collider counts as clicked within this distance of its centre, in world units.")]
        [SerializeField, Min(0.05f)] private float _fallbackRadius = 0.4f;

        private Camera _camera;
        private InputActionMap _gameplay;
        private InputAction _point;
        private InputAction _place;

        /// <summary>Raised after a click has killed a silverfish.</summary>
        public event System.Action<BasicEnemy> SilverfishClicked;

        /// <summary>True on the frame a click killed a silverfish: that click selects, builds and collects nothing.</summary>
        public static bool ClickUsedThisFrame => s_usedFrame == Time.frameCount;

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
            BasicEnemy hit = FindSilverfishAt(world);
            if (hit == null)
            {
                return;
            }

            // Marked before the kill: the kill can drop a power-up under the cursor this frame.
            s_usedFrame = Time.frameCount;

            // The same kill Jason's OnMouseDown did.
            hit.TakeDamage(hit.CurrentHealth);
            SilverfishClicked?.Invoke(hit);
        }

        /// <summary>The first living silverfish under <paramref name="world"/>, or null.</summary>
        public BasicEnemy FindSilverfishAt(Vector3 world)
        {
            IReadOnlyList<BasicEnemy> enemies = EnemyRegistry.All;
            for (int i = 0; i < enemies.Count; i++)
            {
                BasicEnemy silverfish = enemies[i];
                if (silverfish.Data == null || !silverfish.Data.DiesToClick || silverfish.IsDead)
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

        private bool Covers(BasicEnemy silverfish, Vector3 world)
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

        // Domain reload is on, but clearing keeps Play mode safe if that ever changes.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            s_usedFrame = -1;
        }
    }
}
