using HoldTheHill.Features.GameFlow;
using TMPro;
using UnityEngine;

namespace HoldTheHill.Features.UI
{
    /// <summary>Shows "Wave 3 / 10". Reads 0 before the first wave is sent.</summary>
    [RequireComponent(typeof(TMP_Text))]
    public class WaveLabel : LevelRunListener
    {
        private TMP_Text _label;
        private EnemySpawner _spawner;

        private void Awake()
        {
            _label = GetComponent<TMP_Text>();
        }

        protected override void Bind(LevelRun run)
        {
            _spawner = run.Spawner;
            if (_spawner == null)
            {
                return;
            }

            _spawner.onWaveStarted.AddListener(Show);
            int sent = _spawner.CurrentState == EnemySpawner.SpawnerState.Idle ? 0 : _spawner.CurrentWaveNumber;
            Show(sent, _spawner.TotalWaves);
        }

        protected override void Unbind(LevelRun run)
        {
            if (_spawner != null)
            {
                _spawner.onWaveStarted.RemoveListener(Show);
                _spawner = null;
            }
        }

        private void Show(int wave, int total)
        {
            _label.text = string.Format(UiStrings.WaveFormat, wave, total);
        }
    }
}
