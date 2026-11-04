using UnityEngine;

namespace HoldTheHill.Features.Towers
{
    /// <summary>
    /// The timing of a tower's attack animation, as pure functions so EditMode tests cover it. The
    /// art's attack row is wind-up frames, then the hit frame, then follow-through and recovery
    /// (Assets/Ants: frames 0-1 wind up, 2 hits). The animation starts so that its hit frame shows the
    /// moment the tower attacks, and it is shortened to fit when the tower fires faster than the row
    /// plays (a fast tower, an upgrade, Swarm Frenzy).
    /// </summary>
    public static class TowerAttackTiming
    {
        /// <summary>Never shorter than this per frame, so a very fast tower still shows each frame.</summary>
        public const float MinFrameSeconds = 0.02f;

        /// <summary>
        /// Seconds per attack frame: the sheet's time, or less, so the whole row fits in the time
        /// between two attacks.
        /// </summary>
        public static float FrameSeconds(float sheetFrameSeconds, int frameCount, float fireInterval, float fireRateFactor)
        {
            float frame = Mathf.Max(MinFrameSeconds, sheetFrameSeconds);
            if (frameCount <= 0 || fireInterval <= 0f)
            {
                return frame;
            }

            float between = fireInterval / Mathf.Max(1f, fireRateFactor);
            return Mathf.Max(MinFrameSeconds, Mathf.Min(frame, between / frameCount));
        }

        /// <summary>Seconds of wind-up before the hit frame.</summary>
        public static float WindUpSeconds(int hitFrame, float frameSeconds)
        {
            return Mathf.Max(0, hitFrame) * frameSeconds;
        }

        /// <summary>
        /// True when the tower should start winding up now: it attacks on its own, can act, has a
        /// target, and will be ready to attack within the wind-up.
        /// </summary>
        public static bool ShouldWindUp(bool attacksOnItsOwn, bool canAct, bool hasTarget, float cooldownRemaining, float windUpSeconds)
        {
            return attacksOnItsOwn && canAct && hasTarget && cooldownRemaining <= windUpSeconds;
        }

        /// <summary>The frame to show <paramref name="clock"/> seconds into the attack, or -1 once it is over.</summary>
        public static int FrameAt(float clock, float frameSeconds, int frameCount)
        {
            if (frameCount <= 0 || frameSeconds <= 0f || clock < 0f)
            {
                return -1;
            }

            int index = (int)(clock / frameSeconds);
            return index < frameCount ? index : -1;
        }
    }
}
