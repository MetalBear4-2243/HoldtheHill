using HoldTheHill.Features.GameFlow;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace HoldTheHill.Features.UI
{
    /// <summary>
    /// Shows the upcoming wave's description under the wave counter once a wave is cleared,
    /// and hides it when the next wave starts. Stays hidden when there is no next wave or
    /// the upcoming wave description is blank or still the default placeholder.
    /// Before the first wave is sent it shows the first wave's description the same way, so a
    /// level can open with a line of its own (Tutorial Hill uses it to say how to build and start).
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class WaveDescriptionLabel : LevelRunListener
    {
        private TMP_Text _label;
        private EnemySpawner _spawner;

        private void Awake()
        {
            EnsureLabel();
            Hide();
        }

        protected override void Bind(LevelRun run)
        {
            BindSpawner(run != null ? run.Spawner : null);
        }

        protected override void Unbind(LevelRun run)
        {
            UnbindSpawner();
        }

        /// <summary>
        /// Subscribes to <paramref name="spawner"/>'s wave events and clears any previous text.
        /// </summary>
        public void BindSpawner(EnemySpawner spawner)
        {
            UnbindSpawner();

            _spawner = spawner;
            if (_spawner == null)
            {
                return;
            }

            if (_spawner.onWaveStarted == null)
            {
                _spawner.onWaveStarted = new UnityEvent<int, int>();
            }

            if (_spawner.onWaveCleared == null)
            {
                _spawner.onWaveCleared = new UnityEvent<int>();
            }

            if (_spawner.onMapChanged == null)
            {
                _spawner.onMapChanged = new UnityEvent<string>();
            }

            _spawner.onWaveStarted.AddListener(HandleWaveStarted);
            _spawner.onWaveCleared.AddListener(HandleWaveCleared);
            _spawner.onMapChanged.AddListener(HandleMapChanged);
            ShowFirstWaveIfNotStarted();
        }

        /// <summary>
        /// Unsubscribes from the bound spawner and hides the description label.
        /// </summary>
        public void UnbindSpawner()
        {
            if (_spawner != null)
            {
                _spawner.onWaveStarted?.RemoveListener(HandleWaveStarted);
                _spawner.onWaveCleared?.RemoveListener(HandleWaveCleared);
                _spawner.onMapChanged?.RemoveListener(HandleMapChanged);
                _spawner = null;
            }

            Hide();
        }

        private void HandleWaveStarted(int wave, int total)
        {
            Hide();
        }

        private void HandleWaveCleared(int clearedWaveNumber)
        {
            if (_spawner == null || (Run != null && Run.Outcome != RunOutcome.Playing))
            {
                Hide();
                return;
            }

            Show(_spawner.GetUpcomingWaveDescription(clearedWaveNumber));
        }

        private void HandleMapChanged(string mapId)
        {
            ShowFirstWaveIfNotStarted();
        }

        // Shows wave 1's description while no wave has been sent yet; otherwise leaves the label alone.
        private void ShowFirstWaveIfNotStarted()
        {
            if (_spawner == null
                || _spawner.CurrentState != EnemySpawner.SpawnerState.Idle
                || _spawner.CurrentWaveIndex != 0)
            {
                return;
            }

            Show(EnemySpawner.GetDisplayableWaveDescription(_spawner.GetWave(0)));
        }

        private void Show(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                Hide();
                return;
            }

            TMP_Text label = EnsureLabel();
            if (label != null)
            {
                label.text = description;
                label.enabled = true;
            }
        }

        private void Hide()
        {
            TMP_Text label = EnsureLabel();
            if (label != null)
            {
                label.text = string.Empty;
                label.enabled = false;
            }
        }

        private TMP_Text EnsureLabel()
        {
            if (_label == null)
            {
                _label = GetComponent<TMP_Text>();
            }

            return _label;
        }
    }
}
