using System;

namespace HoldTheHill.Features.PowerUps
{
    /// <summary>
    /// The one HUD slot: holds at most one collected power-up until it is used. Plain C#, so the
    /// rule can be tested; <see cref="LevelPowerUps"/> owns one. A drop cannot be collected while the slot
    /// is full; it waits on the map instead.
    /// </summary>
    public sealed class PowerUpSlot
    {
        /// <summary>Raised when the slot fills or empties.</summary>
        public event Action Changed;

        /// <summary>What the slot holds, or null when empty.</summary>
        public PowerUpKind? Held { get; private set; }

        public bool IsEmpty => Held == null;

        /// <summary>Puts <paramref name="kind"/> in the slot. False, and nothing changes, when it is full.</summary>
        public bool TryStore(PowerUpKind kind)
        {
            if (Held != null)
            {
                return false;
            }

            Held = kind;
            Changed?.Invoke();
            return true;
        }

        /// <summary>Empties the slot and hands back what it held, or null when it was empty.</summary>
        public PowerUpKind? Take()
        {
            PowerUpKind? held = Held;
            if (held != null)
            {
                Held = null;
                Changed?.Invoke();
            }

            return held;
        }
    }
}
