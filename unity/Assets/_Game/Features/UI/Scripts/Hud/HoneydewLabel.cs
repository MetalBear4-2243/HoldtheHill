using HoldTheHill.Features.Economy;
using HoldTheHill.Features.GameFlow;
using TMPro;
using UnityEngine;

namespace HoldTheHill.Features.UI
{
    /// <summary>Shows the run's Honeydew. Updates only when the amount changes.</summary>
    [RequireComponent(typeof(TMP_Text))]
    public class HoneydewLabel : LevelRunListener
    {
        private TMP_Text _label;
        private Honeydew _honeydew;

        private void Awake()
        {
            _label = GetComponent<TMP_Text>();
        }

        protected override void Bind(LevelRun run)
        {
            if (run.Wallet == null)
            {
                return;
            }

            _honeydew = run.Wallet.Honeydew;
            _honeydew.Changed += Show;
            Show(_honeydew.Amount);
        }

        protected override void Unbind(LevelRun run)
        {
            if (_honeydew != null)
            {
                _honeydew.Changed -= Show;
                _honeydew = null;
            }
        }

        private void Show(int amount)
        {
            _label.text = string.Format(UiStrings.HoneydewFormat, amount);
        }
    }
}
