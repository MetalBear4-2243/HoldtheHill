using HoldTheHill.Features.GameFlow;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HoldTheHill.Features.UI
{
    /// <summary>The one result panel for both outcomes: a title that says won or lost, Restart and Home.</summary>
    public class ResultPanel : LevelRunListener
    {
        [Tooltip("The panel to show when the run ends. Inactive when the game starts.")]
        [SerializeField] private GameObject _panel;

        [Tooltip("Reads the victory or defeat line.")]
        [SerializeField] private TMP_Text _title;

        [SerializeField] private Button _restart;
        [SerializeField] private Button _home;

        protected override void OnEnable()
        {
            _restart.onClick.AddListener(Restart);
            _home.onClick.AddListener(Home);
            _panel.SetActive(false);
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _restart.onClick.RemoveListener(Restart);
            _home.onClick.RemoveListener(Home);
        }

        protected override void Bind(LevelRun run)
        {
            run.Finished += Show;
            if (run.Outcome != RunOutcome.Playing)
            {
                Show(run.Outcome);
            }
            else
            {
                _panel.SetActive(false);
            }
        }

        protected override void Unbind(LevelRun run)
        {
            run.Finished -= Show;
            _panel.SetActive(false);
        }

        private void Show(RunOutcome outcome)
        {
            _title.text = outcome == RunOutcome.Won ? UiStrings.Victory : UiStrings.Defeat;
            _panel.SetActive(true);

            // Take the keyboard focus from whatever HUD button had it, so Enter means Restart, not Sell.
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(_restart.gameObject);
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
