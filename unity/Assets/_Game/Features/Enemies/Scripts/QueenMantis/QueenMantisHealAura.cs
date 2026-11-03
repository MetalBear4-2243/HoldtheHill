using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// The white shade: every <see cref="QueenMantisData.HealInterval"/> seconds she heals the hurt
    /// enemies within <see cref="QueenMantisData.HealRadius"/> (never herself, <see cref="QueenMantisHealRule"/>)
    /// with Jason's <see cref="BasicEnemy.Heal"/>, and a <see cref="QueenMantisPulse"/> shows each heal.
    /// Quiet while she prays and in the black shade.
    /// </summary>
    [DisallowMultipleComponent]
    public class QueenMantisHealAura : QueenMantisPart
    {
        [Tooltip("The ring on her child object that grows out to the heal radius at each heal.")]
        [SerializeField] private QueenMantisPulse _pulse;

        private float _timer;

        /// <summary>She healed; the number is how many enemies it reached (0 when none were near and hurt).</summary>
        public event Action<int> Healed;

        protected override void OnShadeChanged(QueenMantisShade shade)
        {
            _timer = 0f;
        }

        private void Update()
        {
            if (Data == null || !Boss.CanAct || Boss.Shade != QueenMantisShade.White)
            {
                return;
            }

            _timer += Time.deltaTime;
            if (_timer < Data.HealInterval)
            {
                return;
            }
            _timer -= Data.HealInterval;
            HealAround();
        }

        private void HealAround()
        {
            int healed = 0;
            Vector3 here = transform.position;
            IReadOnlyList<BasicEnemy> all = EnemyRegistry.All;
            for (int i = 0; i < all.Count; i++)
            {
                BasicEnemy other = all[i];
                if (other == null)
                {
                    continue;
                }

                Vector3 offset = other.transform.position - here;
                if (QueenMantisHealRule.ShouldHeal(other == Boss, other.IsDead, other.CurrentHealth, other.MaxHealth,
                        offset.x, offset.y, Data.HealRadius))
                {
                    other.Heal(Data.HealAmount);
                    healed++;
                }
            }

            // The pulse shows every heal, even an empty one, so the white shade reads on screen.
            if (_pulse != null)
            {
                _pulse.Play(Data.HealRadius, Data.HealPulseColor);
            }
            Healed?.Invoke(healed);
        }
    }
}
