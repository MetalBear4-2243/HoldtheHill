using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// The Rhino Beetle's shield, an ability beside <see cref="BasicEnemy"/> around an
    /// <see cref="EnemyShield"/>. Damage hits the shield first; what gets through reaches the
    /// enemy's health. The shield grows back after a few seconds without a hit. An optional sprite
    /// over the beetle fades with it.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BasicEnemy))]
    public class RhinoBeetleShield : MonoBehaviour, IDamageModifier
    {
        [Header("Shield")]
        [Tooltip("How much damage a full shield soaks before anything reaches health.")]
        [SerializeField, Min(0f)] private float _shieldCapacity = 40f;

        [Tooltip("Seconds without a hit before the shield starts to grow back.")]
        [SerializeField, Min(0f)] private float _shieldRegrowDelay = 3f;

        [Tooltip("Shield points regained per second once it is growing back.")]
        [SerializeField, Min(0f)] private float _shieldRegrowPerSecond = 25f;

        [Tooltip("Optional: a sprite drawn over the beetle whose alpha follows the shield. Leave empty for none.")]
        [SerializeField] private SpriteRenderer _shieldVisual;

        private BasicEnemy _enemy;
        private EnemyShield _shield;
        private float _shieldVisualAlpha;

        /// <summary>The shield rule, for tests and the visual.</summary>
        public EnemyShield Shield
        {
            get
            {
                EnsureShield();
                return _shield;
            }
        }

        private void Awake()
        {
            _enemy = GetComponent<BasicEnemy>();
            EnsureShield();
        }

        private void Update()
        {
            if (_enemy.IsDead)
            {
                return;
            }

            _shield.Tick(Time.deltaTime);
            RefreshShieldVisual();
        }

        /// <summary>The shield soaks what it can; the rest reaches health.</summary>
        public double ModifyDamage(double damage)
        {
            EnsureShield();
            double through = _shield.Soak(damage);
            RefreshShieldVisual();
            return through;
        }

        // Made on demand as well as in Awake, so EditMode tests, which never run Awake, work too.
        private void EnsureShield()
        {
            if (_shield != null)
            {
                return;
            }

            _shield = new EnemyShield(_shieldCapacity, _shieldRegrowDelay, _shieldRegrowPerSecond);
            if (_shieldVisual != null)
            {
                _shieldVisualAlpha = _shieldVisual.color.a;
            }
            RefreshShieldVisual();
        }

        private void RefreshShieldVisual()
        {
            if (_shieldVisual == null)
            {
                return;
            }

            Color color = _shieldVisual.color;
            color.a = _shieldVisualAlpha * _shield.Fraction;
            _shieldVisual.color = color;
        }
    }
}
