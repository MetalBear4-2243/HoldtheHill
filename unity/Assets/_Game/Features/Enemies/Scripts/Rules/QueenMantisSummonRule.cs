using System;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>Where her summoned Mantis Spawn appear, as plain C# so EditMode tests can cover it.</summary>
    public static class QueenMantisSummonRule
    {
        /// <summary>
        /// The offset from her of escort <paramref name="index"/> of <paramref name="count"/>, spread
        /// evenly on a ring of <paramref name="radius"/>. The first one stands straight above her.
        /// </summary>
        public static void RingOffset(int index, int count, float radius, out float x, out float y)
        {
            int slots = count < 1 ? 1 : count;
            double angle = Math.PI * 0.5 + 2.0 * Math.PI * index / slots;
            x = (float)(radius * Math.Cos(angle));
            y = (float)(radius * Math.Sin(angle));
        }
    }
}
