using System.Collections.Generic;
using HoldTheHill.Features.Combat;
using HoldTheHill.Features.Enemies;
using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// The Sap Ant: slows every enemy inside the tower's range. Uses Jason's status effects on
    /// <see cref="BasicEnemy"/>, re-applied each tick for a little longer than the tick, so an
    /// enemy that walks out of range, or outlives the tower, gets its speed back on its own.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Tower))]
    public class SlowAura : MonoBehaviour
    {
        private const string EffectName = "Sap";

        [Tooltip("Movement speed while inside the aura. 0.5 is half speed.")]
        [SerializeField, Range(0.05f, 1f)] private float _slowMultiplier = 0.5f;

        [Tooltip("Seconds between re-applying the slow to everyone in range.")]
        [SerializeField, Min(0.05f)] private float _tickInterval = 0.25f;

        private Tower _tower;
        private float _untilNextTick;

        /// <summary>Movement speed multiplier applied inside the aura.</summary>
        public float SlowMultiplier => _slowMultiplier;

        private void Awake()
        {
            _tower = GetComponent<Tower>();
        }

        private void Update()
        {
            _untilNextTick -= Time.deltaTime;
            if (_untilNextTick > 0f)
            {
                return;
            }

            _untilNextTick = _tickInterval;
            ApplyToEnemiesInRange();
        }

        private void ApplyToEnemiesInRange()
        {
            // Twice the tick, so the slow never flickers off between two applications.
            var effect = new StatusEffectData
            {
                EffectName = EffectName,
                Duration = _tickInterval * 2f,
                SlowMultiplier = _slowMultiplier,
            };

            Vector3 here = transform.position;
            float rangeSqr = _tower.Range * _tower.Range;
            IReadOnlyList<BasicEnemy> enemies = EnemyRegistry.All;

            for (int i = 0; i < enemies.Count; i++)
            {
                BasicEnemy enemy = enemies[i];
                if (enemy != null && !enemy.IsDead && (enemy.transform.position - here).sqrMagnitude <= rangeSqr)
                {
                    enemy.ApplyStatusEffect(effect);
                }
            }
        }
    }
}
