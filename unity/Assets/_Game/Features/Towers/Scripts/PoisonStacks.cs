using HoldTheHill.Features.Combat;
using HoldTheHill.Features.Enemies;
using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// Poison stacks on one enemy (the Crazy Ant's). Added to the enemy on its first stack. Each stack
    /// adds to the damage of one "Poison" status effect on the enemy, and every new stack restarts the
    /// clock; when the clock runs out the effect ends and the stacks go back to none.
    /// </summary>
    [DisallowMultipleComponent]
    public class PoisonStacks : MonoBehaviour
    {
        private const string EffectName = "Poison";
        private const string OverlayKind = "Poison";

        private BasicEnemy _enemy;
        private float _remaining;

        /// <summary>How many stacks the enemy carries now.</summary>
        public int Stacks { get; private set; }

        /// <summary>
        /// Adds one stack to <paramref name="enemy"/>, up to <paramref name="maxStacks"/>, and starts the
        /// poison over for <paramref name="seconds"/>: <paramref name="damagePerStack"/> per stack every
        /// <paramref name="tickInterval"/> seconds. Shows <paramref name="overlayFrames"/> on it meanwhile.
        /// </summary>
        public static void AddStack(BasicEnemy enemy, float damagePerStack, float tickInterval, float seconds, int maxStacks,
            Sprite[] overlayFrames, float overlayFrameSeconds)
        {
            if (enemy == null || !enemy.IsActive)
            {
                return;
            }

            PoisonStacks stacks = enemy.GetComponent<PoisonStacks>();
            if (stacks == null)
            {
                stacks = enemy.gameObject.AddComponent<PoisonStacks>();
                stacks._enemy = enemy;
            }

            stacks.Stacks = NextStacks(stacks._remaining > 0f ? stacks.Stacks : 0, maxStacks);
            stacks._remaining = seconds;
            enemy.ApplyStatusEffect(new StatusEffectData
            {
                EffectName = EffectName,
                DamagePerTick = damagePerStack * stacks.Stacks,
                TickInterval = tickInterval,
                Duration = seconds,
                SlowMultiplier = 1f,
            });
            StatusOverlay.Show(enemy, OverlayKind, overlayFrames, overlayFrameSeconds, seconds);
        }

        /// <summary>The stack count after one more stack: one up, never past <paramref name="maxStacks"/> (at least one).</summary>
        public static int NextStacks(int stacks, int maxStacks)
        {
            return Mathf.Clamp(stacks + 1, 1, Mathf.Max(1, maxStacks));
        }

        private void Update()
        {
            if (_remaining <= 0f)
            {
                return;
            }

            // The enemy's own effect ends at the same moment (same duration, restarted together).
            _remaining -= Time.deltaTime;
            if (_remaining <= 0f)
            {
                Stacks = 0;
            }
        }
    }
}
