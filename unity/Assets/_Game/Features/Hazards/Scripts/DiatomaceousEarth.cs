using System.Collections.Generic;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.Towers;
using UnityEngine;

namespace HoldTheHill.Features.Hazards
{
    /// <summary>
    /// A patch of diatomaceous earth on the map. It first settles: the dust fades in and does
    /// nothing yet, which is the warning (time to burrow, or to see which towers it will cover).
    /// Then, while it lasts, it hurts every enemy on the ground inside it on a tick, and stuns every
    /// tower inside it (the stun is renewed each tick, so a tower is free again right after the patch
    /// is gone or it is no longer inside).
    /// </summary>
    [DisallowMultipleComponent]
    public class DiatomaceousEarth : MonoBehaviour
    {
        [Tooltip("Radius of the patch in world units. The sprite is scaled to match.")]
        [SerializeField, Min(0.1f)] private float _radius = 1.2f;

        [Tooltip("Seconds the dust takes to settle, fading in, before it hurts or stuns anything. The warning.")]
        [SerializeField, Min(0f)] private float _settleSeconds = 1.5f;

        [Tooltip("Seconds the patch hurts and stuns once it has settled.")]
        [SerializeField, Min(0.1f)] private float _duration = 12f;

        [Tooltip("Seconds between ticks of damage and stun.")]
        [SerializeField, Min(0.05f)] private float _tickInterval = 0.5f;

        [Tooltip("Damage dealt to each enemy inside on every tick.")]
        [SerializeField, Min(0f)] private float _damagePerTick = 4f;

        private readonly List<BasicEnemy> _hit = new List<BasicEnemy>();
        private SpriteRenderer _sprite;
        private Color _settledColor;
        private float _age;
        private float _untilNextTick;
        private bool _settledShown;

        /// <summary>Radius of the patch in world units.</summary>
        public float Radius => _radius;

        /// <summary>True once the dust has settled and hurts and stuns.</summary>
        public bool Settled => _age >= _settleSeconds;

        private void Start()
        {
            transform.localScale = Vector3.one * (_radius * 2f);
            _sprite = GetComponentInChildren<SpriteRenderer>();
            if (_sprite != null)
            {
                _settledColor = _sprite.color;
                ShowSettling();
            }
        }

        private void Update()
        {
            _age += Time.deltaTime;
            if (_age >= _settleSeconds + _duration)
            {
                Destroy(gameObject);
                return;
            }

            // Every settling frame, and once more on the first settled one so the dust ends solid.
            if (!_settledShown)
            {
                ShowSettling();
                _settledShown = Settled;
            }

            if (!Settled)
            {
                return;
            }

            _untilNextTick -= Time.deltaTime;
            if (_untilNextTick > 0f)
            {
                return;
            }

            _untilNextTick = _tickInterval;
            Tick();
        }

        /// <summary>
        /// How opaque the dust is while it settles, as a share of its settled look: it fades in from a
        /// faint haze to full over the settle time, so the player sees where it will land.
        /// </summary>
        public static float SettleShare(float age, float settleSeconds)
        {
            const float Haze = 0.2f;
            if (settleSeconds <= 0f || age >= settleSeconds)
            {
                return 1f;
            }

            return Mathf.Lerp(Haze, 1f, Mathf.Clamp01(age / settleSeconds));
        }

        // A settling patch shows its full size but only part of its colour; once settled it is solid.
        private void ShowSettling()
        {
            if (_sprite == null)
            {
                return;
            }

            Color color = _settledColor;
            color.a *= SettleShare(_age, _settleSeconds);
            _sprite.color = color;
        }

        private void Tick()
        {
            Vector3 here = transform.position;
            float radiusSqr = _radius * _radius;

            // Collect first: a death removes the enemy from the registry while it is read.
            _hit.Clear();
            IReadOnlyList<BasicEnemy> enemies = EnemyRegistry.All;
            for (int i = 0; i < enemies.Count; i++)
            {
                BasicEnemy enemy = enemies[i];
                if (enemy == null || enemy.IsDead || (enemy.transform.position - here).sqrMagnitude > radiusSqr)
                {
                    continue;
                }

                // Dust on the ground does nothing to a flyer.
                if (enemy is IFlyingEnemy flyer && flyer.IsFlying)
                {
                    continue;
                }

                _hit.Add(enemy);
            }

            for (int i = 0; i < _hit.Count; i++)
            {
                _hit[i].TakeDamage(_damagePerTick);
            }

            // Twice the tick, so the stun never flickers off between two ticks.
            IReadOnlyList<Tower> towers = TowerRegistry.All;
            for (int i = 0; i < towers.Count; i++)
            {
                Tower tower = towers[i];
                if (tower != null && (tower.transform.position - here).sqrMagnitude <= radiusSqr)
                {
                    tower.Stun(_tickInterval * 2f);
                }
            }
        }
    }
}
