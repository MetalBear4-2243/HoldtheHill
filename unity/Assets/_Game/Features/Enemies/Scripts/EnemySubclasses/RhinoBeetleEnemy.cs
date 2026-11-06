using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// The Rhino Beetle: a <see cref="BasicEnemy"/> behind an <see cref="EnemyShield"/>. Damage
    /// hits the shield first; what gets through reaches its health. The shield grows back after a
    /// few seconds without a hit. Both of Jason's damage entry points end in
    /// <see cref="TakeDamage(double)"/>, so overriding that one covers them.
    /// </summary>
    public class RhinoBeetleEnemy : BasicEnemy
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

        private EnemyShield _shield;
        private float _shieldVisualAlpha;

        /// <summary>The shield rule, for tests and the visual.</summary>
        public EnemyShield Shield => _shield;

        protected override void Awake()
        {
            base.Awake();
            _shield = new EnemyShield(_shieldCapacity, _shieldRegrowDelay, _shieldRegrowPerSecond);
            if (_shieldVisual != null)
            {
                _shieldVisualAlpha = _shieldVisual.color.a;
            }
            RefreshShieldVisual();
        }

        protected override void Update()
        {
            if (!IsDead)
            {
                _shield.Tick(Time.deltaTime);
                RefreshShieldVisual();
            }

            base.Update();
        }

        /// <inheritdoc />
        public override void TakeDamage(double damageAmount)
        {
            if (damageAmount <= 0.0 || IsDead)
            {
                return;
            }

            double through = _shield.Soak(damageAmount);
            RefreshShieldVisual();
            if (through > 0.0)
            {
                base.TakeDamage(through);
            }
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
