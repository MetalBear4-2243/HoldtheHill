using HoldTheHill.Features.GameFlow;
using UnityEngine;
using UnityEngine.UI;

namespace HoldTheHill.Features.UI
{
    /// <summary>A bar that follows the Queen's health through her health-changed event.</summary>
    public class QueenHealthBar : LevelRunListener
    {
        [Tooltip("A filled Image; its fill amount is health divided by max health.")]
        [SerializeField] private Image _fill;

        private Queen _queen;

        protected override void Bind(LevelRun run)
        {
            _queen = run.Queen;
            if (_queen == null)
            {
                return;
            }

            _queen.OnHealthChanged += Show;
            Show(_queen.CurrentHealth, _queen.MaxHealth);
        }

        protected override void Unbind(LevelRun run)
        {
            if (_queen != null)
            {
                _queen.OnHealthChanged -= Show;
                _queen = null;
            }
        }

        private void Show(double current, double max)
        {
            if (_fill != null)
            {
                _fill.fillAmount = max > 0.0 ? (float)(current / max) : 0f;
            }
        }
    }
}
