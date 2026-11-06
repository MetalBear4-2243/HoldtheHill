using System;

namespace HoldTheHill.Features.Enemies
{
    /// <summary>
    /// The Rolly Polly's curl, as plain C# so EditMode tests can cover it. A hit on an uncurled
    /// Rolly Polly lands in full and makes it curl; while curled it takes only a share of each
    /// further hit and rolls faster, until it has covered <see cref="RollDistance"/> of path.
    /// It can curl at most <see cref="MaxCurls"/> times in its life; after that every hit lands in full.
    /// </summary>
    public sealed class RollyPollyCurl
    {
        public RollyPollyCurl(int maxCurls, float rollDistance, double curledDamageShare, float curledSpeedMultiplier)
        {
            MaxCurls = Math.Max(0, maxCurls);
            RollDistance = Math.Max(0f, rollDistance);
            CurledDamageShare = Math.Min(1.0, Math.Max(0.0, curledDamageShare));
            CurledSpeedMultiplier = Math.Max(0f, curledSpeedMultiplier);
            CurlsLeft = MaxCurls;
        }

        /// <summary>How many times it can curl in one life.</summary>
        public int MaxCurls { get; }

        /// <summary>Path distance rolled while curled before it uncurls, in world units (one tile is one unit).</summary>
        public float RollDistance { get; }

        /// <summary>The share of each hit that lands while curled; 0.4 means 40 percent.</summary>
        public double CurledDamageShare { get; }

        /// <summary>Movement speed multiplier while curled.</summary>
        public float CurledSpeedMultiplier { get; }

        /// <summary>Curls still available.</summary>
        public int CurlsLeft { get; private set; }

        /// <summary>True while curled up and rolling.</summary>
        public bool IsCurled { get; private set; }

        /// <summary>Path distance still to roll before uncurling; zero when not curled.</summary>
        public float DistanceLeft { get; private set; }

        /// <summary>The speed multiplier to apply right now: the curled one while rolling, otherwise 1.</summary>
        public float SpeedMultiplier => IsCurled ? CurledSpeedMultiplier : 1f;

        /// <summary>
        /// Takes a hit of <paramref name="damage"/> and returns how much of it lands. The hit that
        /// makes it curl lands in full; hits while curled land at <see cref="CurledDamageShare"/>.
        /// </summary>
        public double Hit(double damage)
        {
            if (damage <= 0.0)
            {
                return 0.0;
            }

            if (IsCurled)
            {
                return damage * CurledDamageShare;
            }

            if (CurlsLeft > 0)
            {
                CurlsLeft--;
                IsCurled = true;
                DistanceLeft = RollDistance;
            }

            return damage;
        }

        /// <summary>
        /// Records <paramref name="distance"/> of path rolled. Returns true on the call that
        /// finishes the roll and uncurls; false otherwise, including while not curled.
        /// </summary>
        public bool Advance(float distance)
        {
            if (!IsCurled || distance <= 0f)
            {
                return false;
            }

            DistanceLeft -= distance;
            if (DistanceLeft > 0f)
            {
                return false;
            }

            DistanceLeft = 0f;
            IsCurled = false;
            return true;
        }
    }
}
