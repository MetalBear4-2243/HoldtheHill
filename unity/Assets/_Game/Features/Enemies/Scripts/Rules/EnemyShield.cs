using System;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// The Rhino Beetle's shield, as plain C# so EditMode tests can cover it. Damage hits the
    /// shield first and only what is left reaches the enemy's health. Once the shield has gone
    /// untouched for <see cref="RegrowDelay"/> seconds it grows back at <see cref="RegrowPerSecond"/>;
    /// any hit, even one the shield soaks completely, restarts that wait.
    /// </summary>
    public sealed class EnemyShield
    {
        public EnemyShield(double capacity, float regrowDelay, double regrowPerSecond)
        {
            Capacity = Math.Max(0.0, capacity);
            RegrowDelay = Math.Max(0f, regrowDelay);
            RegrowPerSecond = Math.Max(0.0, regrowPerSecond);
            Current = Capacity;
        }

        /// <summary>How much damage a full shield soaks.</summary>
        public double Capacity { get; }

        /// <summary>Seconds without a hit before the shield starts to grow back.</summary>
        public float RegrowDelay { get; }

        /// <summary>Shield points regained per second once regrowing.</summary>
        public double RegrowPerSecond { get; }

        /// <summary>Shield points left right now.</summary>
        public double Current { get; private set; }

        /// <summary>Seconds since the last hit.</summary>
        public float UntouchedFor { get; private set; }

        /// <summary>True while any shield is left.</summary>
        public bool IsUp => Current > 0.0;

        /// <summary>Shield left as a share of the capacity, 0 to 1, for a visual.</summary>
        public float Fraction => Capacity > 0.0 ? (float)(Current / Capacity) : 0f;

        /// <summary>
        /// Takes <paramref name="damage"/> on the shield and returns the part that gets through to
        /// health. Zero or negative damage changes nothing.
        /// </summary>
        public double Soak(double damage)
        {
            if (damage <= 0.0)
            {
                return 0.0;
            }

            UntouchedFor = 0f;
            double absorbed = Math.Min(Current, damage);
            Current -= absorbed;
            return damage - absorbed;
        }

        /// <summary>
        /// Advances time: counts the untouched seconds and regrows the shield once the delay has
        /// passed. Only the part of this tick that lies beyond the delay regrows anything.
        /// </summary>
        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f)
            {
                return;
            }

            UntouchedFor += deltaTime;
            if (Current >= Capacity)
            {
                return;
            }

            float regrowing = Math.Min(deltaTime, UntouchedFor - RegrowDelay);
            if (regrowing <= 0f)
            {
                return;
            }

            Current = Math.Min(Capacity, Current + RegrowPerSecond * regrowing);
        }
    }
}
