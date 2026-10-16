using HoldTheHill.Features.GameFlow;
using UnityEngine;
using UnityEngine.UI;

namespace HoldTheHill.Features.UI
{
    /// <summary>The Next Wave button. Sends the current run's next wave, the same as the Space key.</summary>
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
            _button.interactable = true;
        }

        protected override void Unbind(LevelRun run)
        {
            _button.onClick.RemoveListener(run.StartNextWave);
            _button.interactable = false;
        }
    }
}
