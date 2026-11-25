using HoldTheHill.Core;
using HoldTheHill.Core.Save;
using HoldTheHill.Features.GameFlow;
using UnityEngine;

namespace HoldTheHill.Features.Progression
{
    /// <summary>
    /// Writes a won level into the save file: completed, and the most hill health ever left.
    /// Records only; nothing reads it to lock levels yet. Hooks itself up when the game starts,
    /// so no scene holds it.
    /// </summary>
    public static class LevelProgressRecorder
    {
        private static LevelRun s_run;

        // After LevelRun clears its events (SubsystemRegistration), before any scene wakes up.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Install()
        {
            s_run = null;
            LevelRun.Started -= HandleRunStarted;
            LevelRun.Ended -= HandleRunEnded;
            LevelRun.Started += HandleRunStarted;
            LevelRun.Ended += HandleRunEnded;
        }

        private static void HandleRunStarted(LevelRun run)
        {
            HandleRunEnded(s_run);
            s_run = run;
            run.Finished += HandleRunFinished;
        }

        private static void HandleRunEnded(LevelRun run)
        {
            if (run != null && run == s_run)
            {
                run.Finished -= HandleRunFinished;
                s_run = null;
            }
        }

        private static void HandleRunFinished(RunOutcome outcome)
        {
            if (outcome != RunOutcome.Won || s_run == null)
            {
                return;
            }

            string levelId = SceneLoader.Current != null ? SceneLoader.Current.OpenLevelId : null;
            if (string.IsNullOrEmpty(levelId))
            {
                Debug.LogWarning("A level was won, but no SceneLoader knows which one, so it was not saved.");
                return;
            }

            Queen queen = s_run.Queen;
            float hillLeft = queen != null && queen.MaxHealth > 0 ? (float)(queen.CurrentHealth / queen.MaxHealth) : 0f;
            if (SaveStore.Data.RecordWin(levelId, hillLeft))
            {
                SaveStore.Save();
            }
        }
    }
}
