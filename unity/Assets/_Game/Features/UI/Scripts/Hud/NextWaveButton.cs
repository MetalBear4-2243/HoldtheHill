using HoldTheHill.Features.GameFlow;
using UnityEngine;
using UnityEngine.UI;

namespace HoldTheHill.Features.UI
{
    /// <summary>
    /// The Next Wave button. Sends the current run's next wave, the same as the Space key, and is
    /// greyed out whenever that would do nothing: while a wave is still coming out, after the last
    /// wave, under the pause menu and once the run is over (<see cref="NextWaveRule"/>).
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class NextWaveButton : LevelRunListener
    {
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.interactable = false;
        }

        protected override void Bind(LevelRun run)
        {
            _button.onClick.AddListener(run.StartNextWave);
            Refresh();
        }

        protected override void Unbind(LevelRun run)
        {
            _button.onClick.RemoveListener(run.StartNextWave);
            _button.interactable = false;
        }

        // The spawner raises nothing when a wave finishes coming out on its own, so the state is
        // read each frame: one property read and a bool compare.
        private void Update()
        {
            Refresh();
        }

        private void Refresh()
        {
            bool canSend = Run != null && Run.CanSendNextWave;
            if (_button.interactable != canSend)
            {
                _button.interactable = canSend;
            }
        }
    }
}
