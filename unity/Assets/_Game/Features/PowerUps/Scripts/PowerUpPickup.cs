using UnityEngine;

namespace HoldTheHill.Features.PowerUps
{
    /// <summary>
    /// A power-up lying on the map where an enemy died. Bobs so it is seen, fades away after its
    /// lifetime, and says whether a click lands on it. <see cref="LevelPowerUps"/> makes and collects them.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PowerUpPickup : MonoBehaviour
    {
        [Tooltip("A click within this distance of the centre, in world units, collects the drop.")]
        [SerializeField, Min(0.05f)] private float _pickRadius = 0.45f;

        [Tooltip("How far the drop bobs up and down, in world units.")]
        [SerializeField, Min(0f)] private float _bobHeight = 0.12f;

        [Tooltip("Bobs per second.")]
        [SerializeField, Min(0f)] private float _bobRate = 1.5f;

        private SpriteRenderer _sprite;
        private Vector3 _restPosition;
        private float _lifetime;
        private float _age;

        /// <summary>Which power-up this drop gives.</summary>
        public PowerUpKind Kind { get; private set; }

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _restPosition = transform.position;
        }

        /// <summary>Sets what the drop is and how it looks. Called right after it is spawned.</summary>
        public void Init(PowerUpKind kind, Sprite icon, Color color, float lifetime)
        {
            Kind = kind;
            _lifetime = lifetime;
            _age = 0f;
            _restPosition = transform.position;
            if (icon != null)
            {
                _sprite.sprite = icon;
            }
            _sprite.color = color;
        }

        private void Update()
        {
            _age += Time.deltaTime;
            if (_lifetime > 0f && _age >= _lifetime)
            {
                Destroy(gameObject);
                return;
            }

            float bob = Mathf.Sin(_age * _bobRate * Mathf.PI * 2f) * _bobHeight;
            transform.position = _restPosition + Vector3.up * bob;

            // The last quarter of its life it fades, as a warning that it is about to go.
            if (_lifetime > 0f)
            {
                float left = _lifetime - _age;
                float fade = Mathf.Clamp01(left / (_lifetime * 0.25f));
                Color color = _sprite.color;
                color.a = Mathf.Lerp(0.2f, 1f, fade);
                _sprite.color = color;
            }
        }

        /// <summary>True if a click at <paramref name="world"/> lands on this drop.</summary>
        public bool Covers(Vector3 world)
        {
            Vector3 centre = _restPosition;
            centre.z = world.z;
            return (centre - world).sqrMagnitude <= _pickRadius * _pickRadius;
        }
    }
}
