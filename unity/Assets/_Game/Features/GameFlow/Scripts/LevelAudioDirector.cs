using System.Collections.Generic;
using HoldTheHill.Core.Audio;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.Towers;
using UnityEngine;

namespace HoldTheHill.Features.GameFlow
{
    /// <summary>
    /// Macro-level audio director for a level run.
    /// Observes high-level gameplay events (waves starting and completing, Queen damage,
    /// run victory/defeat, boss entrances, and tower placements) and triggers the corresponding
    /// <see cref="SoundEffectSO"/> assets. Operates completely decoupled from gameplay simulation logic.
    /// </summary>
    [DisallowMultipleComponent]
    public class LevelAudioDirector : MonoBehaviour
    {
        [Header("Run Reference")]
        [Tooltip("Optional direct reference to the LevelRun. If unassigned, automatically subscribes to LevelRun.Started.")]
        [SerializeField] private LevelRun _run;

        [Header("Wave Audio")]
        [Tooltip("Played when a wave begins.")]
        [SerializeField] private SoundEffectSO _waveStartedSfx;

        [Tooltip("Played when a wave is completed.")]
        [SerializeField] private SoundEffectSO _waveCompletedSfx;

        [Header("Queen / Base Audio")]
        [Tooltip("Played when the Queen takes damage.")]
        [SerializeField] private SoundEffectSO _queenDamagedSfx;

        [Header("Run Outcome Audio")]
        [Tooltip("Played on run victory.")]
        [SerializeField] private SoundEffectSO _victorySfx;

        [Tooltip("Played on run defeat.")]
        [SerializeField] private SoundEffectSO _defeatSfx;

        [Header("Boss & Special Enemies")]
        [Tooltip("Played when a boss appears (e.g. Queen Mantis).")]
        [SerializeField] private SoundEffectSO _bossAppearedSfx;

        [Tooltip("Played when a Centipede splits into segments.")]
        [SerializeField] private SoundEffectSO _centipedeSplitSfx;

        [Header("Towers")]
        [Tooltip("Played when a tower is successfully placed on the grid.")]
        [SerializeField] private SoundEffectSO _towerPlacedSfx;

        private LevelRun _boundRun;
        private double _lastQueenHealth = -1;

        /// <summary>Optional direct reference to the LevelRun.</summary>
        public LevelRun Run
        {
            get => _run;
            set => _run = value;
        }

        public SoundEffectSO WaveStartedSfx { get => _waveStartedSfx; set => _waveStartedSfx = value; }
        public SoundEffectSO WaveCompletedSfx { get => _waveCompletedSfx; set => _waveCompletedSfx = value; }
        public SoundEffectSO QueenDamagedSfx { get => _queenDamagedSfx; set => _queenDamagedSfx = value; }
        public SoundEffectSO VictorySfx { get => _victorySfx; set => _victorySfx = value; }
        public SoundEffectSO DefeatSfx { get => _defeatSfx; set => _defeatSfx = value; }
        public SoundEffectSO BossAppearedSfx { get => _bossAppearedSfx; set => _bossAppearedSfx = value; }
        public SoundEffectSO CentipedeSplitSfx { get => _centipedeSplitSfx; set => _centipedeSplitSfx = value; }
        public SoundEffectSO TowerPlacedSfx { get => _towerPlacedSfx; set => _towerPlacedSfx = value; }

        /// <summary>The LevelRun currently bound to this director.</summary>
        public LevelRun BoundRun => _boundRun;

        private void OnEnable()
        {
            CentipedeEnemy.OnAnyCentipedeSplit += HandleCentipedeSplit;
            QueenMantisBoss.Appeared += HandleBossAppeared;

            if (_run != null)
            {
                BindToRun(_run);
            }
            else
            {
                LevelRun.Started += BindToRun;
                LevelRun.Ended += HandleRunEnded;

                if (LevelRun.Current != null)
                {
                    BindToRun(LevelRun.Current);
                }
            }
        }

        private void OnDisable()
        {
            CentipedeEnemy.OnAnyCentipedeSplit -= HandleCentipedeSplit;
            QueenMantisBoss.Appeared -= HandleBossAppeared;

            if (_run == null)
            {
                LevelRun.Started -= BindToRun;
                LevelRun.Ended -= HandleRunEnded;
            }

            if (_boundRun != null)
            {
                UnbindFromRun(_boundRun);
                _boundRun = null;
            }
        }

        /// <summary>Binds to a LevelRun instance and subscribes to its child systems.</summary>
        public void BindToRun(LevelRun run)
        {
            if (run == null || run == _boundRun)
            {
                return;
            }

            if (_boundRun != null)
            {
                UnbindFromRun(_boundRun);
            }

            _boundRun = run;
            _lastQueenHealth = run.Queen != null ? run.Queen.CurrentHealth : -1;

            if (run.Spawner != null)
            {
                run.Spawner.onWaveStarted.AddListener(HandleWaveStarted);
                run.Spawner.onWaveCompleted.AddListener(HandleWaveCompleted);
            }

            if (run.Queen != null)
            {
                run.Queen.OnHealthChanged += HandleQueenHealthChanged;
            }

            if (run.Placer != null)
            {
                run.Placer.TowerPlaced += HandleTowerPlaced;
            }

            run.Finished += HandleRunFinished;
        }

        private void HandleRunEnded(LevelRun run)
        {
            if (run == _boundRun)
            {
                UnbindFromRun(run);
                _boundRun = null;
            }
        }

        /// <summary>Unbinds from a LevelRun instance to prevent memory leaks across scene unloads.</summary>
        public void UnbindFromRun(LevelRun run)
        {
            if (run == null)
            {
                return;
            }

            if (run.Spawner != null)
            {
                run.Spawner.onWaveStarted.RemoveListener(HandleWaveStarted);
                run.Spawner.onWaveCompleted.RemoveListener(HandleWaveCompleted);
            }

            if (run.Queen != null)
            {
                run.Queen.OnHealthChanged -= HandleQueenHealthChanged;
            }

            if (run.Placer != null)
            {
                run.Placer.TowerPlaced -= HandleTowerPlaced;
            }

            run.Finished -= HandleRunFinished;
        }

        private void HandleWaveStarted(int waveNumber, int totalWaves)
        {
            _waveStartedSfx?.Play();
        }

        private void HandleWaveCompleted(int completedWaveNumber)
        {
            _waveCompletedSfx?.Play();
        }

        private void HandleQueenHealthChanged(double currentHealth, double maxHealth)
        {
            if (_lastQueenHealth > 0 && currentHealth < _lastQueenHealth)
            {
                _queenDamagedSfx?.Play();
            }
            _lastQueenHealth = currentHealth;
        }

        private void HandleRunFinished(RunOutcome outcome)
        {
            if (outcome == RunOutcome.Victory)
            {
                _victorySfx?.Play();
            }
            else if (outcome == RunOutcome.Defeat)
            {
                _defeatSfx?.Play();
            }
        }

        private void HandleBossAppeared(QueenMantisBoss boss)
        {
            _bossAppearedSfx?.Play();
        }

        private void HandleCentipedeSplit(CentipedeEnemy parent, List<CentipedeEnemy> children)
        {
            if (parent != null)
            {
                _centipedeSplitSfx?.Play(parent.transform.position);
            }
            else
            {
                _centipedeSplitSfx?.Play();
            }
        }

        private void HandleTowerPlaced(Tower tower)
        {
            if (tower != null)
            {
                _towerPlacedSfx?.Play(tower.transform.position);
            }
            else
            {
                _towerPlacedSfx?.Play();
            }
        }
    }
}

