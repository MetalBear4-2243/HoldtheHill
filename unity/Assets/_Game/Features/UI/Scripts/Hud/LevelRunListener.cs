using HoldTheHill.Features.GameFlow;
using UnityEngine;

namespace HoldTheHill.Features.UI
{
    /// <summary>
    /// Base for HUD and menu pieces in the persistent UI scene. They cannot hold a serialized
    /// reference into a level scene, so this follows <see cref="LevelRun.Started"/> and
    /// <see cref="LevelRun.Ended"/> and hands each run to <see cref="Bind"/> and <see cref="Unbind"/>.
    /// </summary>
    public abstract class LevelRunListener : MonoBehaviour
    {
        private LevelRun _run;

        /// <summary>The run this piece shows, or null between levels.</summary>
        protected LevelRun Run => _run;

        protected virtual void OnEnable()
        {
            LevelRun.Started += Attach;
            LevelRun.Ended += Detach;

            // The level may have started before this scene loaded.
            if (LevelRun.Current != null)
            {
                Attach(LevelRun.Current);
            }
        }

        protected virtual void OnDisable()
        {
            LevelRun.Started -= Attach;
            LevelRun.Ended -= Detach;

            if (_run != null)
            {
                Detach(_run);
            }
        }

        /// <summary>Subscribe to <paramref name="run"/> and show its current state.</summary>
        protected abstract void Bind(LevelRun run);

        /// <summary>Undo <see cref="Bind"/>. The run's scene objects may already be destroyed.</summary>
        protected abstract void Unbind(LevelRun run);

        private void Attach(LevelRun run)
        {
            if (_run == run)
            {
                return;
            }

            if (_run != null)
            {
                Unbind(_run);
            }

            _run = run;
            Bind(run);
        }

        private void Detach(LevelRun run)
        {
            if (_run != run)
            {
                return;
            }

            Unbind(run);
            _run = null;
        }
    }
}
