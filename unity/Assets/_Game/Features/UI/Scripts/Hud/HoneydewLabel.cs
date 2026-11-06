using HoldTheHill.Features.Economy;
using HoldTheHill.Features.GameFlow;
using TMPro;
using UnityEngine;

namespace HoldTheHill.Features.UI
{
    /// <summary>
    /// Shows the run's Honeydew. Updates only when the amount changes. Sits on the balance box of
    /// Adam's shop list, which shows the number alone; the HUD's top-left Honeydew line that also
    /// used it was removed on 2026-11-05.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class HoneydewLabel : LevelRunListener
    {
        [Tooltip("Show only the number, for a box that already says what it holds.")]
        [SerializeField] private bool _amountOnly;

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
            _label.text = _amountOnly ? amount.ToString() : string.Format(UiStrings.HoneydewFormat, amount);
        }
    }
}
