using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoldTheHill.Core
{
    /// <summary>
    /// "This frame's click on the map is taken": one left click on the map does one thing.
    /// Whoever acts on the click first calls <see cref="Claim"/>; everyone later in the frame reads
    /// <see cref="TakenThisFrame"/> and leaves the click alone. The order is set with
    /// <c>[DefaultExecutionOrder]</c>: the silverfish reader (-10), then the power-ups (-5), then
    /// the tower placer (0), which only reads.
    /// </summary>
    public static class MapClick
    {
        private static int s_takenFrame = -1;

        /// <summary>True once something has used this frame's map click.</summary>
        public static bool TakenThisFrame => s_takenFrame == Time.frameCount;

        /// <summary>Marks this frame's map click as used.</summary>
        public static void Claim()
        {
            s_takenFrame = Time.frameCount;
        }

        /// <summary>Forgets any claim. Runs when Play starts and whenever a scene unloads.</summary>
        public static void Reset()
        {
            s_takenFrame = -1;
        }

        // Statics survive a level restart and a whole PlayMode test run, so clear on both.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnPlay()
        {
            Reset();
            SceneManager.sceneUnloaded -= ResetOnUnload;
            SceneManager.sceneUnloaded += ResetOnUnload;
        }

        private static void ResetOnUnload(Scene scene)
        {
            Reset();
        }
    }
}
