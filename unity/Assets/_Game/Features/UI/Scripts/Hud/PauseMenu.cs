using HoldTheHill.Features.GameFlow;
using UnityEngine;
using UnityEngine.UI;

namespace HoldTheHill.Features.UI
{
    /// <summary>
    /// The pause panel: shown while the run's clock is paused and the run is still going.
    /// Its buttons work at time scale zero because uGUI buttons ignore <c>Time.timeScale</c>.
    /// </summary>
    public class PauseMenu : LevelRunListener
    {
        [Tooltip("The panel to show while paused. Inactive when the game starts.")]
        [SerializeField] private GameObject _panel;

        [SerializeField] private Button _resume;
        [SerializeField] private Button _restart;
        [SerializeField] private Button _home;

        protected override void OnEnable()
        {
            _resume.onClick.AddListener(Resume);
            _restart.onClick.AddListener(Restart);
            _home.onClick.AddListener(Home);
            Hide();
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _resume.onClick.RemoveListener(Resume);
            _restart.onClick.RemoveListener(Restart);
            _home.onClick.RemoveListener(Home);
        }

        protected override void Bind(LevelRun run)
        {
            run.Clock.Changed += Refresh;
            run.Finished += HandleFinished;
            Refresh();
        }

        protected override void Unbind(LevelRun run)
        {
            run.Clock.Changed -= Refresh;
            run.Finished -= HandleFinished;
            Hide();
        }

        private void Refresh()
        {
            bool paused = Run != null && Run.Clock.Paused && Run.Outcome == RunOutcome.Playing;
            _panel.SetActive(paused);
        }

        private void HandleFinished(RunOutcome outcome)
        {
            // The result panel takes over.
            Hide();
        }

        private void Hide()
        {
            _panel.SetActive(false);
        }

        private void Resume()
        {
            if (Run != null)
            {
                Run.Resume();
            }
        }

        private void Restart()
        {
            if (Run != null)
            {
                Run.RestartLevel();
            }
        }

        private void Home()
        {
            if (Run != null)
            {
                Run.GoHome();
            }
        }
    }
}
