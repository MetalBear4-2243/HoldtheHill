namespace HoldTheHill.Core.Animation
{
    /// <summary>
    /// Timing for a row of sprites played once (a hurt flash, a death), as pure functions so EditMode
    /// tests cover it.
    /// </summary>
    public static class SpriteRow
    {
        /// <summary>The frame to show <paramref name="clock"/> seconds in, or -1 once the row is over.</summary>
        public static int FrameAt(float clock, float frameSeconds, int frameCount)
        {
            if (frameCount <= 0 || frameSeconds <= 0f || clock < 0f)
            {
                return -1;
            }

            int index = (int)(clock / frameSeconds);
            return index < frameCount ? index : -1;
        }

        /// <summary>How long the row lasts, played once.</summary>
        public static float Length(float frameSeconds, int frameCount)
        {
            return frameCount > 0 && frameSeconds > 0f ? frameSeconds * frameCount : 0f;
        }
    }
}
