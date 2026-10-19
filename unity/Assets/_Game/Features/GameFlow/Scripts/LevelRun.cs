using System;
using HoldTheHill.Core;
using HoldTheHill.Features.Economy;
using HoldTheHill.Features.Enemies;
using HoldTheHill.Features.Towers;
using UnityEngine;

namespace HoldTheHill.Features.GameFlow
{
    /// <summary>
    /// Where a level's run comes together: the Queen, the spawner and the wallet, the pause and
    /// speed <see cref="RunClock"/>, and the win or lose decision. Lives in the level's gameplay
    /// scene. The HUD and menus live in the persistent UI scene, so they reach it through
    /// <see cref="Current"/> and the <see cref="Started"/> / <see cref="Ended"/> events instead of
    /// a serialized reference.
    /// </summary>
    // After the spawner (default order 0), so its wave list is loaded when Started is raised.
    [DefaultExecutionOrder(100)]
    [DisallowMultipleComponent]
    public class LevelRun : MonoBehaviour
    {
        [Header("Level")]
        [Tooltip("The hill's health pool. Its Game Over loses the run.")]
        [SerializeField] private Queen _queen;

        [Tooltip("The level's spawner. The run is won when it has sent every wave and no enemy is left.")]
        [SerializeField] private EnemySpawner _spawner;

        [Tooltip("The run's Honeydew, shown on the HUD.")]
        [SerializeField] private HoneydewWallet _wallet;

        [Tooltip("Places and selects towers; the tower card and the U key reach it through the run.")]
        [SerializeField] private TowerPlacer _placer;

        [Tooltip("Drops, the slot and the two power-ups; the HUD slot reaches it through the run.")]
        [SerializeField] private PowerUps.LevelPowerUps _powerUps;

        [Header("Speed")]
        [Tooltip("Time scale while the fast speed is on.")]
        [SerializeField, Min(1f)] private float _fastTimeScale = 2f;

        private static LevelRun s_current;

        private bool _queenDefeated;

        /// <summary>The run in progress, or null between levels.</summary>
        public static LevelRun Current => s_current;

        /// <summary>Raised when a run is ready: its scene objects exist and their Awake has run.</summary>
        public static event Action<LevelRun> Started;

        /// <summary>Raised when the current run goes away (the level scene unloads).</summary>
        public static event Action<LevelRun> Ended;

        /// <summary>Raised once, when the run is won or lost.</summary>
        public event Action<RunOutcome> Finished;

        /// <summary>The hill's health pool.</summary>
        public Queen Queen => _queen;

        /// <summary>The level's spawner.</summary>
        public EnemySpawner Spawner => _spawner;

        /// <summary>The run's Honeydew.</summary>
        public HoneydewWallet Wallet => _wallet;

        /// <summary>The level's tower placer, for the tower card. May be null in a scene without one.</summary>
        public TowerPlacer Placer => _placer;

        /// <summary>The level's power-ups, for the HUD slot. May be null in a scene without them.</summary>
        public PowerUps.LevelPowerUps PowerUps => _powerUps;

        /// <summary>Pause and speed. Created in Awake.</summary>
        public RunClock Clock { get; private set; }

        /// <summary>Playing until the run is won or lost.</summary>
        public RunOutcome Outcome { get; private set; }

        private void Awake()
        {
            Clock = new RunClock(_fastTimeScale);
        }

        private void OnEnable()
        {
            Clock.Changed += ApplyTimeScale;
            if (_queen != null)
            {
                _queen.OnGameOver += HandleQueenDefeated;
            }
        }

        // Start, not OnEnable: the wallet makes its Honeydew in Awake and the spawner loads its waves
        // in Start, and listeners read both as soon as they hear the run started.
        private void Start()
        {
            s_current = this;
            Started?.Invoke(this);
        }

        private void OnDisable()
        {
            Clock.Changed -= ApplyTimeScale;
            if (_queen != null)
            {
                _queen.OnGameOver -= HandleQueenDefeated;
            }

            // The level is going away; whatever it did to time ends with it.
            Time.timeScale = 1f;

            if (s_current == this)
            {
                s_current = null;
                Ended?.Invoke(this);
            }
        }

        private void Update()
        {
            // A run is won when the last enemy dies after the last wave; neither the registry nor the
            // spawner announces that moment, so it is checked each frame. Two reads, no allocation.
            if (Outcome == RunOutcome.Playing)
            {
                Decide();
            }
        }

        /// <summary>Sends the next wave. Does nothing while paused or once the run is over.</summary>
        public void StartNextWave()
        {
            if (Outcome == RunOutcome.Playing && !Clock.Paused && _spawner != null)
            {
                _spawner.StartNextWave();
            }
        }

        /// <summary>Pauses, or resumes when paused. Does nothing once the run is over.</summary>
        public void TogglePause()
        {
            if (Outcome == RunOutcome.Playing)
            {
                Clock.TogglePause();
            }
        }

        /// <summary>Resumes from the pause menu.</summary>
        public void Resume()
        {
            if (Outcome == RunOutcome.Playing)
            {
                Clock.SetPaused(false);
            }
        }

        /// <summary>Switches between normal and fast speed.</summary>
        public void ToggleSpeed()
        {
            if (Outcome == RunOutcome.Playing)
            {
                Clock.ToggleSpeed();
            }
        }

        /// <summary>Buys the next level of the selected tower, if any. The U key comes here.</summary>
        public void UpgradeSelectedTower()
        {
            if (Outcome == RunOutcome.Playing && !Clock.Paused && _placer != null)
            {
                _placer.UpgradeSelectedTower();
            }
        }

        /// <summary>
        /// The B key: burrows every tower, or brings them all up when every one is already down.
        /// </summary>
        public void ToggleBurrowAll()
        {
            if (Outcome == RunOutcome.Playing && !Clock.Paused && _placer != null)
            {
                _placer.ToggleBurrowAll();
            }
        }

        /// <summary>Uses the power-up in the HUD slot, if any. The slot button comes here.</summary>
        public void UsePowerUp()
        {
            if (Outcome == RunOutcome.Playing && !Clock.Paused && _powerUps != null)
            {
                _powerUps.Use();
            }
        }

        /// <summary>Reloads this level from the start.</summary>
        public void RestartLevel()
        {
            SceneLoader loader = SceneLoader.Current;
            if (loader != null)
            {
                loader.RestartLevel();
            }
            else
            {
                Debug.LogError("No SceneLoader is loaded, so the level can't be restarted.", this);
            }
        }

        /// <summary>Leaves the level for the home screen.</summary>
        public void GoHome()
        {
            SceneLoader loader = SceneLoader.Current;
            if (loader != null)
            {
                loader.GoHome();
            }
            else
            {
                Debug.LogError("No SceneLoader is loaded, so there is nowhere to go home to.", this);
            }
        }

        private void HandleQueenDefeated()
        {
            _queenDefeated = true;
            Decide();
        }

        private void Decide()
        {
            if (Outcome != RunOutcome.Playing)
            {
                return;
            }

            bool allWavesSent = _spawner != null && _spawner.CurrentState == EnemySpawner.SpawnerState.Completed;
            RunOutcome outcome = RunOutcomeRule.Decide(_queenDefeated, allWavesSent, EnemyRegistry.All.Count);
            if (outcome == RunOutcome.Playing)
            {
                return;
            }

            Outcome = outcome;
            // The map freezes under the result panel; its buttons run on unscaled time.
            Clock.SetPaused(true);
            Finished?.Invoke(outcome);
        }

        private void ApplyTimeScale()
        {
            Time.timeScale = Clock.TimeScale;

            // The Input System doesn't follow the time scale, so the placer is told: no building,
            // picking or cancelling under the pause menu or the result panel.
            if (_placer != null)
            {
                _placer.AcceptsInput = !Clock.Paused;
            }
        }

        // Domain reload is on, but clearing keeps Play mode safe if that ever changes.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            s_current = null;
            Started = null;
            Ended = null;
        }
    }
}
