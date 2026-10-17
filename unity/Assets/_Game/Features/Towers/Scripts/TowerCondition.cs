using System;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// What is being done to a tower, and what that allows: burrowed (the player's order), stunned
    /// (a hazard, for a while) or missing (a hazard, for a while). Plain C#, so the rules can be
    /// tested without a scene; <see cref="Tower"/> owns one and ticks it. This is the whole
    /// "affliction system" of the Alpha.
    /// </summary>
    public sealed class TowerCondition
    {
        /// <summary>The player's order: a burrowed tower neither fires nor can be hit by a bird or pesticide.</summary>
        public bool Burrowed { get; set; }

        /// <summary>Seconds of stun left. A stunned tower does nothing.</summary>
        public float StunnedFor { get; private set; }

        /// <summary>Seconds of missing left. A missing tower fires, but its shots and hits do nothing.</summary>
        public float MissingFor { get; private set; }

        public bool IsStunned => StunnedFor > 0f;

        public bool IsMissing => MissingFor > 0f;

        /// <summary>True when the tower may aim, fire, slow or lay mines.</summary>
        public bool CanAct => !Burrowed && !IsStunned;

        /// <summary>True when a hit or shot from the tower should deal its damage.</summary>
        public bool HitsLand => !IsMissing;

        /// <summary>Stuns for <paramref name="seconds"/>. A shorter stun never cuts a longer one short.</summary>
        public void Stun(float seconds)
        {
            StunnedFor = Math.Max(StunnedFor, seconds);
        }

        /// <summary>Makes the tower miss for <paramref name="seconds"/>. Never shortens a longer spell.</summary>
        public void Miss(float seconds)
        {
            MissingFor = Math.Max(MissingFor, seconds);
        }

        /// <summary>
        /// Lets <paramref name="deltaTime"/> pass. True when a stun or a missing spell ended this tick,
        /// so the owner can refresh what it shows.
        /// </summary>
        public bool Tick(float deltaTime)
        {
            bool wasStunned = IsStunned;
            bool wasMissing = IsMissing;
            StunnedFor = Math.Max(0f, StunnedFor - deltaTime);
            MissingFor = Math.Max(0f, MissingFor - deltaTime);
            return (wasStunned && !IsStunned) || (wasMissing && !IsMissing);
        }

        /// <summary>
        /// The B key: if any tower is still up, burrow them all; only when every tower is already
        /// burrowed does the key bring them all back up.
        /// </summary>
        public static bool BurrowAllNext(int burrowedCount, int towerCount)
        {
            return burrowedCount < towerCount;
        }
    }
}
