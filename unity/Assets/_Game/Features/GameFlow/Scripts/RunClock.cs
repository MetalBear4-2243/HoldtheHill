using System;

namespace HoldTheHill.Features.GameFlow
{
    /// <summary>
    /// Pause and speed of a run, as plain C#. <see cref="TimeScale"/> is what the scene
    /// writes to <c>Time.timeScale</c>: zero while paused, otherwise the normal or fast speed.
    /// Pausing keeps the chosen speed, so resuming goes back to it.
    /// </summary>
    public sealed class RunClock
    {
        /// <summary>Raised when pause or speed changes.</summary>
        public event Action Changed;

        /// <param name="fastScale">Time scale of the fast speed. Anything up to 1 is treated as 2.</param>
        public RunClock(float fastScale)
        {
            FastScale = fastScale > 1f ? fastScale : 2f;
        }

        /// <summary>Time scale of the fast speed.</summary>
        public float FastScale { get; }

        /// <summary>True while the run is paused.</summary>
        public bool Paused { get; private set; }

        /// <summary>True while the run plays at the fast speed.</summary>
        public bool Fast { get; private set; }

        /// <summary>Zero while paused, otherwise 1 or <see cref="FastScale"/>.</summary>
        public float TimeScale => Paused ? 0f : Fast ? FastScale : 1f;

        /// <summary>Pauses or resumes.</summary>
        public void SetPaused(bool paused)
        {
            if (paused == Paused)
            {
                return;
            }

            Paused = paused;
            Changed?.Invoke();
        }

        /// <summary>Resumes when paused, pauses when running.</summary>
        public void TogglePause()
        {
            SetPaused(!Paused);
        }

        /// <summary>Switches between normal and fast speed. Works while paused; it shows on resume.</summary>
        public void ToggleSpeed()
        {
            Fast = !Fast;
            Changed?.Invoke();
        }
    }
}
