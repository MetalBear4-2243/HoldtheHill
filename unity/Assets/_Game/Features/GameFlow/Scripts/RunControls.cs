using UnityEngine;
using UnityEngine.InputSystem;

namespace HoldTheHill.Features.GameFlow
{
    /// <summary>
    /// Reads the run keys from the game's actions asset and hands them to the
    /// <see cref="LevelRun"/> beside it: NextWave sends a wave, ToggleSpeed switches 1x / 2x,
    /// Pause opens or closes the pause menu, Upgrade buys the selected tower's next level,
    /// Burrow sends every tower underground or brings them all back up.
    /// Keys keep working while paused because the Input System does not follow
    /// <c>Time.timeScale</c>.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LevelRun))]
    public class RunControls : MonoBehaviour
    {
        [Tooltip("The game's actions asset. Uses its Gameplay map: NextWave, ToggleSpeed, Pause, Upgrade, Burrow.")]
        [SerializeField] private InputActionAsset _actions;

        private LevelRun _run;
        private InputActionMap _gameplay;
        private InputAction _nextWave;
        private InputAction _toggleSpeed;
        private InputAction _pause;
        private InputAction _upgrade;
        private InputAction _burrow;

        private void Awake()
        {
            _run = GetComponent<LevelRun>();

            if (_actions == null)
            {
                Debug.LogError("RunControls has no actions asset, so the run keys do nothing.", this);
                return;
            }

            _gameplay = _actions.FindActionMap("Gameplay", throwIfNotFound: true);
            _nextWave = _gameplay.FindAction("NextWave", throwIfNotFound: true);
            _toggleSpeed = _gameplay.FindAction("ToggleSpeed", throwIfNotFound: true);
            _pause = _gameplay.FindAction("Pause", throwIfNotFound: true);
            _upgrade = _gameplay.FindAction("Upgrade", throwIfNotFound: true);
            _burrow = _gameplay.FindAction("Burrow", throwIfNotFound: true);
        }

        private void OnEnable()
        {
            if (_gameplay != null)
            {
                _gameplay.Enable();
            }
        }

        private void OnDisable()
        {
            if (_gameplay != null)
            {
                _gameplay.Disable();
            }
        }

        private void Update()
        {
            if (_gameplay == null)
            {
                return;
            }

            if (_nextWave.WasPressedThisFrame())
            {
                _run.StartNextWave();
            }

            if (_toggleSpeed.WasPressedThisFrame())
            {
                _run.ToggleSpeed();
            }

            if (_pause.WasPressedThisFrame())
            {
                _run.TogglePause();
            }

            if (_upgrade.WasPressedThisFrame())
            {
                _run.UpgradeSelectedTower();
            }

            if (_burrow.WasPressedThisFrame())
            {
                _run.ToggleBurrowAll();
            }
        }
    }
}
